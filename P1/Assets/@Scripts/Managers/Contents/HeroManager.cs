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
        Data = Managers.Data.HeroChart[dataTemplateID];
    }


    public void CalculateInfoStat()
    {
        // StatCalculator 클래스를 이용하여 계산
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

        Debug.Log("총 전투력: " + CalculateTotalCombatPower(this));
    }

    // 스탯 계산
    private float CalculateStat(EHeroUpgradeType upgradeType)
    {
        // 기본 값 및 증가 값 가져오기
        var upgradeData = Managers.Data.HeroUpgradeChart[upgradeType];
        float baseValue = upgradeData.Value;
        float increaseValue = upgradeData.IncreaseValue;
        int currentLevel = Managers.Backend.GameData.UserData.UpgradeStatDic[upgradeType.ToString()];

        // 최종 값 계산
        return baseValue + (increaseValue * (currentLevel - 1));
    }


    // 장비 효과 적용
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


    // 총전투력 계산 함수
    private float CalculateTotalCombatPower(HeroInfo hero)
    {
        // 모든 스탯을 단순히 합산하여 총 전투력을 계산합니다.
        float totalCombatPower = 0.0f;

        // HeroInfo 클래스에서 제공하는 각 스탯의 값을 합산
        totalCombatPower += hero.Atk;       // 공격력
        totalCombatPower += hero.MaxHp;     // 체력
        totalCombatPower += hero.Recovery;  // 회복력
        totalCombatPower += hero.CriRate;   // 치명타 확률
        totalCombatPower += hero.CriDmg;    // 치명타 데미지
        //totalCombatPower += hero.AttackRange; // 공격 범위
        //totalCombatPower += hero.AttackDelay; // 공격 딜레이
        //totalCombatPower += hero.AttackSpeedRate; // 공격 속도

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
            PlayerHeroInfo.Level = Managers.Backend.GameData.UserData.Level;
            PlayerHeroInfo.CalculateInfoStat();
        }));

        PlayerHeroInfo.CalculateInfoStat();
    }


}
