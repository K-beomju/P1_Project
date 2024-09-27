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
        _currentLevel = Managers.Hero.PlayerHeroInfo.Level;
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
        // 레벨에 따른 경험치 증가를 더 부드럽게 조정
        float baseExp = 100f; // 초기 경험치 요구량을 더 높게 설정
        float growthFactor = 1.1f; // 경험치 증가율을 완만하게 설정

        return Mathf.RoundToInt(baseExp * Mathf.Pow(growthFactor, level - 1)); // 레벨에 따른 경험치 요구량 증가

    }



    #endregion
}
