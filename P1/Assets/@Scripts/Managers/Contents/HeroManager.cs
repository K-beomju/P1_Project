using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Data;
using System;

public class HeroInfo
{
    public int Level { get; set; }
    #region Stat
    public float Atk { get; private set; }
    public float MaxHp { get; private set; }
    public float Recovery { get; private set; } 
    public float CriRate { get; private set; }
    public float CriDmg { get; private set; }

    public float AttackRange { get; private set; }
    public float AttackDelay { get; private set; }
    public float AttackSpeedRate { get; private set; }
    #endregion

    public int DataTemplateID { get; private set; }
    public HeroInfoData Data { get; private set; }

    public HeroInfo(int dataTemplateID)
    {
        Level = 1;
        DataTemplateID = dataTemplateID;
        Data = Managers.Data.HeroInfoDataDic[dataTemplateID];
    }


    // 스탯 계산을 위한 공통 함수
    private float CalculateStat(EHeroUpgradeType upgradeType)
    {
        // 기본 값 및 증가 값 가져오기
        var upgradeData = Managers.Data.HeroUpgradeInfoDataDic[upgradeType];
        float baseValue = upgradeData.Value;
        float increaseValue = upgradeData.IncreaseValue;
        int currentLevel = Managers.Hero.HeroGrowthUpgradeLevelDic[upgradeType];

        // 최종 값 계산
        return baseValue + (increaseValue * (currentLevel - 1));
    }

    // 장비 효과 적용을 위한 공통 함수
    private float ApplyEquipmentEffect(EEquipmentType equipmentType, float baseStat)
    {
        // 보유한 장비 효과 및 장착된 장비 효과 가져오기
        float ownedValue = Managers.Equipment.OwnedEquipmentValues(equipmentType);
        float equipValue = Managers.Equipment.EquipEquipmentValue(equipmentType);

        // 보유 효과가 존재하면 먼저 적용
        if (ownedValue != 0)
        {
            baseStat *= (1 + ownedValue / 100f);
        }

        // 장착 효과가 존재하면 추가로 적용
        if (equipValue != 0)
        {
            baseStat *= (1 + equipValue / 100f);
        }

        return baseStat; // 최종 스탯 반환
    }


    public void CalculateInfoStat()
    {
        // 각 스탯을 공통 함수를 통해 계산
        Atk = CalculateStat(EHeroUpgradeType.Growth_Atk);
        MaxHp = CalculateStat(EHeroUpgradeType.Growth_Hp);
        Recovery = CalculateStat(EHeroUpgradeType.Growth_Recovery);
        CriRate = CalculateStat(EHeroUpgradeType.Growth_CriRate);
        CriDmg = CalculateStat(EHeroUpgradeType.Growth_CriDmg);

        // 장비 효과 적용
        Atk = ApplyEquipmentEffect(EEquipmentType.Weapon, Atk);
        MaxHp = ApplyEquipmentEffect(EEquipmentType.Armor, MaxHp);

        AttackRange = Data.AttackRange;
        AttackDelay = Data.AttackDelay;
        AttackSpeedRate = Data.AttackSpeedRate;

        if (Managers.Object.Hero != null)
            Managers.Object.Hero.ReSetStats();
    }
}

public class HeroManager
{
    #region Hero
    public HeroInfo PlayerHeroInfo { get; private set; }
    #endregion

    #region Upgrade (Growth)
    public Dictionary<EHeroUpgradeType, int> HeroUpgradeLevelDic { get; private set; } = new Dictionary<EHeroUpgradeType, int>();

    public Dictionary<EHeroUpgradeType, int> HeroGrowthUpgradeLevelDic
    {
        get { return HeroUpgradeLevelDic.Where(pair => pair.Key == EHeroUpgradeType.Growth_Atk || pair.Key == 
        EHeroUpgradeType.Growth_Hp || pair.Key == EHeroUpgradeType.Growth_Recovery || pair.Key == 
        EHeroUpgradeType.Growth_CriRate || pair.Key == EHeroUpgradeType.Growth_CriDmg).ToDictionary(pair => pair.Key, pair => pair.Value); }
    }
    #endregion

    public void Init()
    {
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Atk, 1);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Hp, 1);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Recovery, 1);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_CriRate, 1);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_CriDmg, 1);

        HeroInfo heroInfo = new HeroInfo(11000);
        PlayerHeroInfo = heroInfo;

        PlayerHeroInfo.CalculateInfoStat();
    }

    #region Upgrade(Growth)
    public void LevelUpHeroUpgrade(EHeroUpgradeType upgradeType)
    {
        if (HeroUpgradeLevelDic.ContainsKey(upgradeType))
        {
            HeroUpgradeLevelDic[upgradeType]++;
        }
        else
        {
            HeroUpgradeLevelDic.Add(upgradeType, 1);
        }

        PlayerHeroInfo.CalculateInfoStat();
        Managers.Event.TriggerEvent(EEventType.HeroUpgradeUpdated);
    }
    #endregion
}
