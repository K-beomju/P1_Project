using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Data;
using System;

public class HeroInfo
{
    #region Stat
    public float Atk { get; private set; }
    public float MaxHp { get; private set; }
    public float AttackRange { get; private set; }
    public float AttackDelay { get; private set; }
    public float AttackSpeedRate { get; private set; }
    #endregion

    public int DataTemplateID { get; private set; }
    public HeroInfoData Data  { get; private set; }

    public HeroInfo(int dataTemplateID)
    {
        DataTemplateID = dataTemplateID;
        Data = Managers.Data.HeroInfoDataDic[dataTemplateID];

        CalculateInfoStat();
    }

    public void CalculateInfoStat()
    {
        // 업그레이드에 의한 공격력 증가 
        float increaseAtk = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Atk] * Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Atk].Value;
        // 레벨에 따른 공격력 증가 (2% 증가)
        float levelMultiplierAtk = 1 + (Managers.Purse._currentLevel * 0.2f);
        // 최종공격력 = (기본 공격력 + 업그레이드 공격력) * 레벨 %
        Atk = (Data.Atk + increaseAtk) * levelMultiplierAtk;

        // 업그레이드에 의한 체력 증가 
        float increaseMaxHp = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Hp] * Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Hp].Value;
        // 레벨에 따른 체력 증가 (5% 증가)
        float levelMultiplierHp = 1 + (Managers.Purse._currentLevel * 5f);
        // 최종최대체력 = (기본 체력 + 업그레이드 체력) * 레벨 %
        MaxHp = (Data.MaxHp + increaseMaxHp) * levelMultiplierHp;

        AttackRange = Data.AttackRange;
        AttackDelay = Data.AttackDelay;
        AttackSpeedRate = Data.AttackSpeedRate;

        if( Managers.Object.Hero != null)
        Managers.Object.Hero.ReSetStats();
    }
}

public class HeroManager
{
    #region Hero
    public HeroInfo PlayerHeroInfo { get; private set;}
    #endregion

    #region Upgrade (Growth)
    public Dictionary<EHeroUpgradeType, int> HeroUpgradeLevelDic { get; private set; } = new Dictionary<EHeroUpgradeType, int>();

    public Dictionary<EHeroUpgradeType, int> HeroGrowthUpgradeLevelDic
    {
        get { return HeroUpgradeLevelDic.Where(pair => pair.Key == EHeroUpgradeType.Growth_Atk || pair.Key == EHeroUpgradeType.Growth_Hp).ToDictionary(pair => pair.Key, pair => pair.Value); }
    }
    #endregion

    public void Init()
    {
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Atk, 0);
        HeroUpgradeLevelDic.Add(EHeroUpgradeType.Growth_Hp, 0);

        HeroInfo heroInfo = new HeroInfo(11000);
        PlayerHeroInfo = heroInfo;
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
