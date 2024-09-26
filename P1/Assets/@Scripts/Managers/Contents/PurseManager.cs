using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PurseManager
{
    private Dictionary<EGoodType, int> _purseDic = new Dictionary<EGoodType, int>();

    public int _currentLevel { get; private set; }
    private int _currentExp = 0;
    private int _expToNextLevel;

    public void Init()
    {
        _currentLevel = 1;
        _expToNextLevel = CalculateRequiredExp(_currentLevel);
        Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, _currentLevel, _currentExp, _expToNextLevel); // 경험치 갱신 이벤트
        Managers.Event.AddEvent(EEventType.PlayerLevelUp, new Action<int>(level =>
		{
			// 히어로의 레벨 업데이트
			Managers.Hero.PlayerHeroInfo.Level = level;
            Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
		}));

    }

    #region Good & Currency

    public void AddAmount(EGoodType goodType, int amount)
    {
        if (!_purseDic.ContainsKey(goodType))
        {
            _purseDic.Add(goodType, amount);
        }
        else
        {
            _purseDic[goodType] += amount;
        }

        Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);
    }

    public int GetAmount(EGoodType goodType)
    {
        _purseDic.TryGetValue(goodType, out int amount);
        return amount;
    }

    #endregion

    #region Experience & Leveling

    public void AddExp(int exp)
    {
        _currentExp += exp;

        // 레벨업 처리
        while (_currentExp >= _expToNextLevel)
        {
            _currentExp -= _expToNextLevel;
            _currentLevel++;
            _expToNextLevel = CalculateRequiredExp(_currentLevel);
            Managers.Event.TriggerEvent(EEventType.PlayerLevelUp, _currentLevel); // 레벨업 이벤트 발생
        }

        Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, _currentLevel, _currentExp, _expToNextLevel); // 경험치 갱신 이벤트
    }

    private int CalculateRequiredExp(int level)
    {
        float coefficient = 5f;
        float expGrowthFactor = 1f;

        if (level >= 1 && level <= 30)
        {
            expGrowthFactor = 1.0f; // 1~30레벨
        }
        else if (level >= 31 && level <= 70)
        {
            expGrowthFactor = 1.5f; // 31~70레벨
        }
        else if (level >= 71 && level <= 100)
        {
            expGrowthFactor = 2.0f; // 71~100레벨
        }

        return Mathf.RoundToInt(Mathf.Pow((level - 1) * 50 / 49f, expGrowthFactor) * coefficient);
    }


    #endregion
}
