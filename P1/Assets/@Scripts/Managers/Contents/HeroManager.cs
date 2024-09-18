using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class HeroManager
{
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
    }

    #region Upgrade(Growth)
    public void LevelUpHeroUpgrade(EHeroUpgradeType upgradeType)
    {
        if(HeroUpgradeLevelDic.ContainsKey(upgradeType))
        {
            HeroUpgradeLevelDic[upgradeType]++;
        }
        else
        {
            HeroUpgradeLevelDic.Add(upgradeType, 1);
        }

        Managers.Event.TriggerEvent(EEventType.UpdateHeroUpgrade);
    }
    #endregion
}
