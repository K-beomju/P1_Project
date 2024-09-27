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

    public void CalculateInfoStat()
    {
        // 업그레이드에 의한 공격력 증가 
        float increaseAtk = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Atk] * Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Atk].Value;
        // 레벨에 따른 공격력 증가 (2% 증가)
        float levelMultiplierAtk = 1 + (Managers.Purse._currentLevel * 0.02f);
        // 최종공격력 = (기본 공격력 + 업그레이드 공격력) * 레벨 %
        Atk = (Data.Atk + increaseAtk) * levelMultiplierAtk;
        // 보유한 무기의 데이터 가져오기 (보유효과)
        float ownedWeaponValues = Managers.Equipment.OwnedEquipmentValues(EEquipmentType.Weapon);
        // 장착된 무기의 데이터 가져오기 (장착효과)
        float equipWeaponValue = Managers.Equipment.EquipEquipmentValue(EEquipmentType.Weapon);
        float totalWeaponEffect = ownedWeaponValues + equipWeaponValue;
        if (totalWeaponEffect != 0)
        {
            // 무기의 총 효과 (보유 + 장착) 적용
            Atk *= 1 + (totalWeaponEffect / 100f); // 합산된 값을 퍼센트로 반영
        }

        // 업그레이드에 의한 체력 증가 
        float increaseMaxHp = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Hp] * Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Hp].Value;
        // 레벨에 따른 체력 증가 (5% 증가)
        float levelMultiplierHp = 1 + (Managers.Purse._currentLevel * 0.05f);
        // 최종최대체력 = (기본 체력 + 업그레이드 체력) * 레벨 %
        MaxHp = (Data.MaxHp + increaseMaxHp) * levelMultiplierHp;

        // 보유한 갑옷의 데이터 가져오기 (보유효과)
        float ownedArmorValues = Managers.Equipment.OwnedEquipmentValues(EEquipmentType.Armor);
        // 장착된 갑옷의 데이터 가져오기 (장착효과)
        float equipArmorValue = Managers.Equipment.EquipEquipmentValue(EEquipmentType.Armor);
        float totalArmorEffect = ownedArmorValues + equipArmorValue;
        if (totalArmorEffect != 0)
        {
            // 무기의 총 효과 (보유 + 장착) 적용
            MaxHp *= 1 + (totalArmorEffect / 100f); // 합산된 값을 퍼센트로 반영
        }

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
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Atk, 0);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Hp, 0);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Recovery, 0);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_CriRate, 0);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_CriDmg, 0);

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
        Debug.LogWarning("LevelUpHeroUpgrade");
        Managers.Event.TriggerEvent(EEventType.HeroUpgradeUpdated);
    }
    #endregion
}
