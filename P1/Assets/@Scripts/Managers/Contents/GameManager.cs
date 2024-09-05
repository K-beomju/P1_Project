using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager
{
	#region MonsterSpawn
	public void SpawnMonster(int spawnCount)
	{
		Vector3 heroPos = Managers.Object.Heroes.FirstOrDefault().transform.position;
		for (int i = 0; i < spawnCount; i++)
		{
			float minDistance = 5.0f;
			float randomDistance = UnityEngine.Random.Range(minDistance, 10.0f);  // 최소 5 이상, 최대 10 이하의 랜덤 거리
			Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;  // 랜덤 방향을 얻습니다.

			// 랜덤 방향에 거리를 곱하여 플레이어로부터 일정 거리만큼 떨어진 위치 계산
			Vector3 spawnPosition = heroPos + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;

			Managers.Object.Spawn<Monster>(spawnPosition);
		}
	}

	#endregion

}
