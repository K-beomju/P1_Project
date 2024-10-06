using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Data;
using System;
using BackendData.GameData;

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
        Level = Managers.Backend.GameData.UserData.Level;
        Debug.Log($"현재 레벨 : {Level}");
        DataTemplateID = dataTemplateID;
        Data = Managers.Data.HeroInfoDataDic[dataTemplateID];
    }


    public void CalculateInfoStat()
    {
        // StatCalculator 클래스를 이용하여 계산
        Atk = Util.CalculateStat(EHeroUpgradeType.Growth_Atk, Managers.Hero.HeroGrowthUpgradeLevelDic);
        MaxHp = Util.CalculateStat(EHeroUpgradeType.Growth_Hp, Managers.Hero.HeroGrowthUpgradeLevelDic);
        Recovery = Util.CalculateStat(EHeroUpgradeType.Growth_Recovery, Managers.Hero.HeroGrowthUpgradeLevelDic);
        CriRate = Util.CalculateStat(EHeroUpgradeType.Growth_CriRate, Managers.Hero.HeroGrowthUpgradeLevelDic);
        CriDmg = Util.CalculateStat(EHeroUpgradeType.Growth_CriDmg, Managers.Hero.HeroGrowthUpgradeLevelDic);
        Debug.Log(CriRate);
        // 장비 효과 적용
        Atk = Util.ApplyEquipmentEffect(EEquipmentType.Weapon, Atk);
        MaxHp = Util.ApplyEquipmentEffect(EEquipmentType.Armor, MaxHp);

        AttackRange = Data.AttackRange;
        AttackDelay = Data.AttackDelay;
        AttackSpeedRate = Data.AttackSpeedRate;

        if (Managers.Object.Hero != null)
            Managers.Object.Hero.ReSetStats();

        Debug.Log("총 전투력: " + Util.CalculateTotalCombatPower(this));
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

        Managers.Event.AddEvent(EEventType.PlayerLevelUp, new Action<int>(level =>
        {
            // 히어로의 레벨 업데이트
            PlayerHeroInfo.Level = Managers.Backend.GameData.UserData.Level;
            PlayerHeroInfo.CalculateInfoStat();
        }));

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
