using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using static Define;

public class GameManager
{
	private Dictionary<EGoodType, int> _purseDic = new Dictionary<EGoodType, int>();

	private int _currentMonsters;
	private int _maxMonsters;

	public int _currentLevel { get; set; } = 1;
	private int _currentExp = 0;
	private int _expToNextLevel;

	public void Init()
	{
		_expToNextLevel = CalculateRequiredExp(_currentLevel);
		Managers.Event.TriggerEvent(EEventType.UpdateExp, _currentLevel, _currentExp, _expToNextLevel); // 경험치 갱신 이벤트
																										// LevelUp 이벤트에 히어로의 스탯 재계산 함수 연결
		Managers.Event.AddEvent(EEventType.LevelUp, new Action<int>(level =>
		{
			// 히어로의 레벨 업데이트
			Managers.Object.Hero.Level = level;

			// 레벨업 시 스탯을 다시 계산하는 함수 호출
			Managers.Object.Hero.RecalculateStats();
		}));
	}

	#region MonsterSpawn

	public void SetMonsterCount(int currentMonsters, int maxMonsters)
	{
		_currentMonsters = currentMonsters;
		_maxMonsters = maxMonsters;

		Managers.Event.TriggerEvent(EEventType.MonsterCountChanged, _currentMonsters, _maxMonsters);
	}

	public void SpawnMonster(StageInfoData stageData, bool isBoss = false)
	{
		var sceneUI = (Managers.UI.SceneUI as UI_GameScene);
		sceneUI.RefreshShowCurrentStage(stageData.StageNumber, 100);

		if (!isBoss) // 노말 스테이지
		{
			sceneUI.RefreshShowRemainMonster(stageData.MonsterCount, stageData.MonsterCount);

			Vector3 heroPos = Managers.Object.Hero.transform.position;
			for (int i = 0; i < stageData.MonsterCount; i++)
			{
				float minDistance = 5.0f;
				float randomDistance = UnityEngine.Random.Range(minDistance, 10.0f);  // 최소 5 이상, 최대 10 이하의 랜덤 거리
				Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;  // 랜덤 방향을 얻습니다.

				// 랜덤 방향에 거리를 곱하여 플레이어로부터 일정 거리만큼 떨어진 위치 계산
				Vector3 spawnPosition = heroPos + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;

				Managers.Object.Spawn<Monster>(spawnPosition, stageData.MonsterDataIdList[UnityEngine.Random.Range(0, stageData.MonsterDataIdList.Count)]);
			}

			SetMonsterCount(stageData.MonsterCount, stageData.MonsterCount);
		}
		else // 보스 스테이지
		{
			Managers.Object.Spawn<BossMonster>(Vector3.zero, stageData.BossDataId);
		}
	}

	public void OnMonsterDestroyed()
	{
		_currentMonsters = Managers.Object.Monsters.Count;
		// 몬스터가 파괴될 때마다 UI를 업데이트
		SetMonsterCount(_currentMonsters, _maxMonsters);
	}

	#endregion

	#region Good 
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

		Managers.Event.TriggerEvent(EEventType.UpdateCurrency);
	}

	public int GetAmount(EGoodType goodType)
	{
		_purseDic.TryGetValue(goodType, out int amount);
		return amount;
	}

	public void AddExp(int exp)
	{
		_currentExp += exp;

		// 경험치가 다음 레벨 요구량을 초과했을 경우 레벨업 처리
		while (_currentExp >= _expToNextLevel)
		{
			_currentExp -= _expToNextLevel;
			_currentLevel++;
			_expToNextLevel = CalculateRequiredExp(_currentLevel);
			Managers.Event.TriggerEvent(EEventType.LevelUp, _currentLevel); // int 매개변수로 레벨 전달
		}

		Managers.Event.TriggerEvent(EEventType.UpdateExp, _currentLevel, _currentExp, _expToNextLevel); // 경험치 갱신 이벤트
	}

	private int CalculateRequiredExp(int level)
	{
		// 제곱수 0.7을 사용하는 공식
		float coefficient = 5f;
		float expGrowthFactor = 1f; // 레벨에 따른 경험치 요구량 (*제곱수)


		// 레벨 구간별로 제곱수를 조정
		if (level >= 1 && level <= 30)
		{
			expGrowthFactor = 1.0f; // 1~30레벨: 빠르게 성장
		}
		else if (level >= 31 && level <= 70)
		{
			expGrowthFactor = 1.5f; // 31~70레벨: 성장이 느려짐
		}
		else if (level >= 71 && level <= 100)
		{
			expGrowthFactor = 2.0f; // 71~100레벨: 매우 느리게 성장
		}
		return Mathf.RoundToInt(Mathf.Pow((level - 1) * 50 / 49f, expGrowthFactor) * coefficient);
	}

	#endregion
}
