using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Data;

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
        float increaseAtk = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Atk] * Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Atk].Value;
        Atk = (Data.Atk + increaseAtk);

        float increaseMaxHp = Managers.Hero.HeroGrowthUpgradeLevelDic[EHeroUpgradeType.Growth_Hp] * Managers.Data.HeroUpgradeInfoDataDic[EHeroUpgradeType.Growth_Hp].Value;
        MaxHp = (Data.MaxHp + increaseMaxHp);

        AttackRange = Data.AttackRange;
        AttackDelay = Data.AttackDelay;
        AttackSpeedRate = Data.AttackSpeedRate;
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
        
        Managers.Event.TriggerEvent(EEventType.UpdateHeroUpgrade);
    }
    #endregion
}
