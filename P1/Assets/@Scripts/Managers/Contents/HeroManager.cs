using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
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
    public BackendData.Chart.Hero.Item Data { get; private set; }

    public HeroInfo(int dataTemplateID)
    {
        Level = Managers.Backend.GameData.UserData.Level;
        Debug.Log($"현재 레벨 : {Level}");
        DataTemplateID = dataTemplateID;
        Data = Managers.Backend.Chart.Hero.Dic[dataTemplateID]; //Managers.Data.HeroInfoDataDic[dataTemplateID];
    }


    public void CalculateInfoStat()
    {
        // StatCalculator 클래스를 이용하여 계산
        Atk = Util.CalculateStat(EHeroUpgradeType.Growth_Atk);
        MaxHp = Util.CalculateStat(EHeroUpgradeType.Growth_Hp);
        Recovery = Util.CalculateStat(EHeroUpgradeType.Growth_Recovery);
        CriRate = Util.CalculateStat(EHeroUpgradeType.Growth_CriRate);
        CriDmg = Util.CalculateStat(EHeroUpgradeType.Growth_CriDmg);
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
    public HeroInfo PlayerHeroInfo { get; private set; }

    public void Init()
    {
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


}
