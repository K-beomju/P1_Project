using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using System;
using Data;

public class HeroInfo
{
    public int Level { get; set; }
    public float CurrentTotalPower { get; private set; }
    public float AdjustTotalPower { get; private set; }

    #region Stat
    public float Atk { get; private set; }
    public float MaxHp { get; private set; }
    public float Recovery { get; private set; }
    public float CriRate { get; private set; }
    public float CriDmg { get; private set; }
    public float GoldIncreaseRate { get; private set; }
    public float ExpIncreaseRate { get; private set; }

    public float AttackRange { get; private set; }
    public float AttackDelay { get; private set; }
    public float AttackSpeedRate { get; private set; }
    #endregion

    #region Attribute
    public float AtkAttr { get; private set; }
    public float MaxHpAttr { get; private set; }
    public float CriRateAttr { get; private set; }
    public float CriDmgAttr { get; private set; }
    public float SkillTimeAttr { get; private set; }
    public float SkillDmgAttr { get; private set; }
    #endregion

    #region Relic
    public float AtkRelic { get; private set; }
    public float MaxHpRelic { get; private set; }
    public float RecoveryRelic { get; private set; }
    public float MonsterDmgRelic { get; private set; }
    public float BossMonsterDmgRelic { get; private set; }
    public float ExpRateRelic { get; private set; }
    public float GoldRateRelic { get; private set; }
    #endregion

    #region Ad Buff
    public float AtkBuff { get; private set; }
    public float GoldRateBuff { get; private set; }
    public float ExpRateBuff { get; private set; }
    #endregion

    #region RankUp
    public float AtkRankUp { get; private set; }
    public float MaxHpRankUp { get; private set; }
    public float RecoveryRankUp { get; private set; }
    public float CriDmgRankUp { get; private set; }
    public float GoldRateRankUp { get; private set; }
    public float ExpRateRankUp { get; private set; }
    #endregion

    public int DataTemplateID { get; private set; }
    public HeroInfoData Data { get; private set; }

    public HeroInfo(int dataTemplateID)
    {
        Level = Managers.Backend.GameData.CharacterData.Level;
        Debug.Log($"현재 레벨 : {Level}");
        DataTemplateID = dataTemplateID;
        Data = Managers.Data.HeroChart[dataTemplateID];
    }


    public void CalculateInfoStat(Action<bool> totalPowerChanged = null)
    {
        float previousTotalPower = CurrentTotalPower;

        // 스탯 레벨 계산
        Atk = CalculateStat(EHeroUpgradeType.Growth_Atk);
        MaxHp = CalculateStat(EHeroUpgradeType.Growth_Hp);
        Recovery = CalculateStat(EHeroUpgradeType.Growth_Recovery);
        CriRate = CalculateStat(EHeroUpgradeType.Growth_CriRate);
        CriDmg = CalculateStat(EHeroUpgradeType.Growth_CriDmg);

        // 특성 레벨 계산 
        AtkAttr = CaculateAttribute(EHeroAttrType.Attribute_Atk);
        MaxHpAttr = CaculateAttribute(EHeroAttrType.Attribute_MaxHp);
        CriRateAttr = CaculateAttribute(EHeroAttrType.Attribute_CriRate);
        CriDmgAttr = CaculateAttribute(EHeroAttrType.Attribute_CriDmg);
        SkillTimeAttr = CaculateAttribute(EHeroAttrType.Attribute_SkillTime);
        SkillDmgAttr = CaculateAttribute(EHeroAttrType.Attribute_SkillDmg);

        // 유물 레벨 계산 
        AtkRelic = CaculateRelic(EHeroRelicType.Relic_Atk);
        MaxHpRelic = CaculateRelic(EHeroRelicType.Relic_MaxHp);
        RecoveryRelic = CaculateRelic(EHeroRelicType.Relic_Recovery);
        MonsterDmgRelic = CaculateRelic(EHeroRelicType.Relic_MonsterDmg);
        BossMonsterDmgRelic = CaculateRelic(EHeroRelicType.Relic_BossMonsterDmg);
        ExpRateRelic = CaculateRelic(EHeroRelicType.Relic_ExpRate);
        GoldRateRelic = CaculateRelic(EHeroRelicType.Relic_GoldRate);

        // 승급 계산 
        AtkRankUp = CaculateRankUp(EHeroRankUpStatType.RankUp_Atk);
        MaxHpRankUp = CaculateRankUp(EHeroRankUpStatType.RankUp_MaxHp);
        RecoveryRankUp = CaculateRankUp(EHeroRankUpStatType.RankUp_Recovery);
        CriDmgRankUp = CaculateRankUp(EHeroRankUpStatType.RankUp_CriDmg);
        GoldRateRankUp = CaculateRankUp(EHeroRankUpStatType.RankUp_GoldRate);
        ExpRateRankUp = CaculateRankUp(EHeroRankUpStatType.RankUp_ExpRate);

        // 장비 보너스 퍼센트 합산
        float AtkEquipmentPer = GetTotalEquipmentBonusPercentage(EEquipmentType.Weapon);
        float MaxHpEquipmentPer = GetTotalEquipmentBonusPercentage(EEquipmentType.Armor);

        // 총 보너스 퍼센트 합산 (장비 + 특성)
        float totalAtkPer = AtkEquipmentPer + AtkAttr + AtkRelic + AtkBuff;
        float totalMaxHpPer = MaxHpEquipmentPer + MaxHpAttr + MaxHpRelic;
        float totalRecoveryPer = RecoveryRelic;
        float totalCriRatePer = CriRateAttr;
        float totalCriDmgPer = CriDmgAttr;
        float totalGoldRate = GoldRateRelic + GoldRateBuff;
        float totalExpRate = ExpRateRelic + ExpRateBuff;

        // 계산 전 Base Stat 저장
        float baseAtk = Atk;
        float baseMaxHp = MaxHp;
        float baseRecovery = Recovery;
        float baseCriRate = CriRate;
        float baseCriDmg = CriDmg;
        float baseGoldIncreaseRate = GoldIncreaseRate; // 기본값은 1
        float baseExpIncreaseRate = ExpIncreaseRate; // 기본값은 1

        // 최종 스탯 계산
        Atk = Atk * (1 + totalAtkPer / 100f);
        MaxHp = MaxHp * (1 + totalMaxHpPer / 100f);
        Recovery = Recovery * (1 + totalRecoveryPer / 100f);
        CriRate = CriRate * (1 + totalCriRatePer / 100f);
        CriDmg = CriDmg * (1 + totalCriDmgPer / 100f);
        GoldIncreaseRate = 1 + totalGoldRate / 100f;
        ExpIncreaseRate = 1 + totalExpRate / 100f;

        // 디버그 정보 요약
        string debugMessage =
        $"<color=yellow>[Hero Stats Summary]</color>\n" +
        $"<color=cyan>Base Stats:</color> Atk: {baseAtk}, MaxHp: {baseMaxHp}, Recovery: {baseRecovery}, CriRate: {baseCriRate}, CriDmg: {baseCriDmg}\n" +
        $"<color=green>Attribute Stats:</color> AtkAttr: {AtkAttr}, MaxHpAttr: {MaxHpAttr}, CriRateAttr: {CriRateAttr}, CriDmgAttr: {CriDmgAttr}, SkillTimeAttr: {SkillTimeAttr}, SkillDmgAttr: {SkillDmgAttr}\n" +
        $"<color=blue>Relic Stats:</color> AtkRelic: {AtkRelic}, MaxHpRelic: {MaxHpRelic}, RecoveryRelic: {RecoveryRelic}, MonsterDmgRelic: {MonsterDmgRelic}, BossMonsterDmgRelic: {BossMonsterDmgRelic}, ExpRateRelic: {ExpRateRelic}, GoldRateRelic: {GoldRateRelic}\n" +
        $"<color=magenta>RankUp Stats:</color> Atk: {AtkRankUp}, MaxHp: {MaxHpRankUp}, Recovery: {RecoveryRankUp}, CriDmg: {CriDmgRankUp}, GoldRate: {GoldRateRankUp}, ExpRate: {ExpRateRankUp}\n" +
        $"<color=orange>Equipment Bonus:</color> AtkEquipmentPer: {AtkEquipmentPer}, MaxHpEquipmentPer: {MaxHpEquipmentPer}\n" +
        $"<color=red>Final Stats:</color> Atk: {Atk}, MaxHp: {MaxHp}, Recovery: {Recovery}, CriRate: {CriRate}, CriDmg: {CriDmg}, GoldIncreaseRate: {GoldIncreaseRate}, ExpIncreaseRate: {ExpIncreaseRate}";

        Debug.Log(debugMessage);

        // 기타 스탯 설정
        AttackRange = Data.AttackRange;
        AttackDelay = Data.AttackDelay;

        if (Managers.Object.Hero != null)
            Managers.Object.Hero.ReSetStats(false);


        // 전투력 변화 계산
        if (CurrentTotalPower == 0)
        {
            CurrentTotalPower = CalculateTotalCombatPower(this);
            //Debug.Log($"[Initial Combat Power] Total: {CurrentTotalPower}");
            return;
        }

        CurrentTotalPower = CalculateTotalCombatPower(this);
        AdjustTotalPower = CurrentTotalPower - previousTotalPower;
        //Debug.Log($"[Total Combat Power] Total: {CurrentTotalPower}, Change: {AdjustTotalPower}");

        // 전투력 변화가 있는지 확인 후 콜백 실행
        bool isPowerChanged = AdjustTotalPower != 0;
        totalPowerChanged?.Invoke(isPowerChanged);
    }

    // 스탯 계산
    private float CalculateStat(EHeroUpgradeType upgradeType)
    {
        // 기본 값 및 증가 값 가져오기
        var upgradeData = Managers.Data.HeroUpgradeChart[upgradeType];
        float baseValue = upgradeData.Value;
        float increaseValue = upgradeData.IncreaseValue;
        int currentLevel = Managers.Backend.GameData.CharacterData.UpgradeStatDic[upgradeType.ToString()];

        // 최종 값 계산
        return baseValue + (increaseValue * (currentLevel - 1));
    }

    // 특성 계산 
    private float CaculateAttribute(EHeroAttrType attrType)
    {
        var attributeData = Managers.Data.HeroAttributeChart[attrType];
        float increaseValue = attributeData.IncreaseValue;
        int currentLevel = Managers.Backend.GameData.CharacterData.UpgradeAttrDic[attrType.ToString()];

        return increaseValue * currentLevel;
    }

    // 유물 계산 
    private float CaculateRelic(EHeroRelicType relicType)
    {
        var relicData = Managers.Data.RelicChart[relicType];
        float increaseValue = relicData.IncreaseValue;
        int currentLevel = Managers.Backend.GameData.CharacterData.OwnedRelicDic[relicType.ToString()];

        return increaseValue * currentLevel;
    }

    // 광고 버프 아이템 
    public void ApplyAdBuff(EAdBuffType buffType)
    {
        switch (buffType)
        {
            case EAdBuffType.Atk:
                AtkBuff += 100;
                break;
            case EAdBuffType.IncreaseGold:
                GoldRateBuff += 100;
                break;
            case EAdBuffType.IncreaseExp:
                ExpRateBuff += 100;
                break;
        }
    }

    public void RemoveAdBuff(EAdBuffType buffType)
    {
        switch (buffType)
        {
            case EAdBuffType.Atk:
                AtkBuff = 0;
                break;
            case EAdBuffType.IncreaseGold:
                GoldRateBuff = 0;
                break;
            case EAdBuffType.IncreaseExp:
                ExpRateBuff = 0;
                break;
        }
    }

    // 장비 보너스 퍼센트 합산 함수
    private float GetTotalEquipmentBonusPercentage(EEquipmentType equipmentType)
    {
        // 보유한 장비 효과 및 장착된 장비 효과 가져오기
        float ownedValue = Managers.Equipment.OwnedEquipmentValues(equipmentType);
        float equipValue = Managers.Equipment.EquipEquipmentValue(equipmentType);

        // 보유 효과와 장착 효과의 퍼센트를 합산
        return ownedValue + equipValue;
    }

    // RankUp 스탯별 계산 함수
    private float CaculateRankUp(EHeroRankUpStatType statType)
    {
        float totalValue = 0;

        foreach (var item in Managers.Backend.GameData.RankUpData.RankUpDic)
        {
            if (item.Value.RankStatType == statType)
            {
                totalValue += item.Value.Value; // 누적 계산
            }
        }

        return totalValue;
    }


    // 총전투력 계산 함수
    private float CalculateTotalCombatPower(HeroInfo hero)
    {
        float AtkBonusValue = 2f;

        // 모든 스탯을 단순히 합산하여 총 전투력을 계산
        float totalCombatPower = 0.0f;

        totalCombatPower += hero.Atk * AtkBonusValue;
        totalCombatPower += hero.MaxHp;
        return totalCombatPower;
    }
}

public class HeroManager
{
    public HeroInfo PlayerHeroInfo { get; private set; }

    public void Init()
    {
        HeroInfo heroInfo = new HeroInfo(11000);
        PlayerHeroInfo = heroInfo;

        Managers.Event.AddEvent(EEventType.PlayerLevelUp, new Action<int>(level =>
        {
            // 히어로의 레벨 업데이트
            PlayerHeroInfo.Level = level;
            PlayerHeroInfo.CalculateInfoStat();

            Managers.UI.ShowBaseUI<UI_LevelUpBase>().ShowLevelUpUI(level);
        }));

        PlayerHeroInfo.CalculateInfoStat();
    }


}
