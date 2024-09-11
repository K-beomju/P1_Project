using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using static Define;

public class GameManager
{
	private int _currentMonsters;
	private int _maxMonsters;

	#region MonsterSpawn

	public void SetMonsterCount(int currentMonsters, int maxMonsters)
	{
		_currentMonsters = currentMonsters;
		_maxMonsters = maxMonsters;

		Managers.Event.TriggerEvent(EEventType.MonsterCountChanged, _currentMonsters, _maxMonsters);
	}

	public void SpawnMonster(StageData stageData)
	{
		var sceneUI = (Managers.UI.SceneUI as UI_GameScene);
		sceneUI.RefreshShowCurrentStage(stageData.StageNumber, 100);

		if (!stageData.BossStage)
		{
			sceneUI.RefreshShowRemainMonster(stageData.MonsterCount, stageData.MonsterCount);

			Vector3 heroPos = Managers.Object.Heroes.FirstOrDefault().transform.position;
			for (int i = 0; i < stageData.MonsterCount; i++)
			{
				float minDistance = 5.0f;
				float randomDistance = UnityEngine.Random.Range(minDistance, 10.0f);  // 최소 5 이상, 최대 10 이하의 랜덤 거리
				Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;  // 랜덤 방향을 얻습니다.

				// 랜덤 방향에 거리를 곱하여 플레이어로부터 일정 거리만큼 떨어진 위치 계산
				Vector3 spawnPosition = heroPos + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;

				Managers.Object.Spawn<Monster>(spawnPosition);
			}

			SetMonsterCount(stageData.MonsterCount, stageData.MonsterCount);
		}
		else
		{
			Managers.Object.Spawn<BossMonster>(Vector3.zero);
		}
	}

	#endregion

	public void OnMonsterDestroyed()
	{
		_currentMonsters = Managers.Object.Monsters.Count;
		// 몬스터가 파괴될 때마다 UI를 업데이트
		SetMonsterCount(_currentMonsters, _maxMonsters);
	}
}
