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

    public void CalculateInfoStat()
    {
        float baseAtkValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Atk].Value;
        float increaseAtkValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Atk].IncreaseValue;
        int currentAtkLevel = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Atk];
        Atk = baseAtkValue + (increaseAtkValue * (currentAtkLevel - 1)); ; //* levelMultiplierAtk;


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

        float baseMaxHpValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Hp].Value;
        float increaseMaxHpValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Hp].IncreaseValue;
        int currentMaxHpLevel = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Hp];
        MaxHp = baseMaxHpValue + (increaseMaxHpValue * (currentMaxHpLevel -1));

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

        float baseRecoveryValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Recovery].Value;
        float increaseRecoveryValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Recovery].IncreaseValue;
        int currentRecoveryLevel = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Recovery];
        Recovery = baseRecoveryValue + (increaseRecoveryValue * (currentRecoveryLevel -1));

        float baseCriRateValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_CriRate].Value;
        float increaseCriRateValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_CriRate].IncreaseValue;
        int currentCriRateLevel = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_CriRate];
        CriRate = baseCriRateValue + (increaseCriRateValue * (currentCriRateLevel - 1));

        float baseCriDmgValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_CriDmg].Value;
        float increaseCriDmgValue = Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_CriDmg].IncreaseValue;
        int currentCriDmgLevel = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_CriDmg];
        CriDmg = baseCriDmgValue + (increaseCriDmgValue * (currentCriDmgLevel - 1));


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
