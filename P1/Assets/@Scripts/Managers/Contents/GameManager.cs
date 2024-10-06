using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;
using static UnityEditor.Progress;


public class GameManager
{
	private Tilemap _tileMap;
	private int _killMonsters;
	private int _maxMonsters;

	public void Init()
	{

		Managers.Event.AddEvent(EEventType.DrawDataUpdated, new Action<EEquipmentType>((type) =>
		{
            try { 
                Managers.Backend.GameData.DrawLevelData.AddDrawCount(type);
            } 
            catch(Exception e) {
                throw new Exception($"EEventType.DrawDataUpdated({type}) 중 에러가 발생하였습니다\n{e}");
            }
        }));

	}
	#region MonsterSpawn

	public void SetMonsterCount(int killMonsters, int maxMonsters)
	{
		_killMonsters = killMonsters;
		_maxMonsters = maxMonsters;

		Managers.Event.TriggerEvent(EEventType.MonsterCountChanged, _killMonsters, _maxMonsters);
	}

	public void SpawnMonster(BackendData.Chart.Stage.Item stageInfo, bool isBoss = false)
	{
		if (_tileMap == null)
		{
			_tileMap = Util.FindChild(GameObject.Find("BaseMap"), "Terrain_Tile", true).GetComponent<Tilemap>();
			_tileMap.CompressBounds();
		}

		Vector3 heroPosition = Managers.Object.Hero.transform.position; // 플레이어의 현재 위치 가져오기

		if (!isBoss) // 노말 스테이지
		{
			for (int i = 0; i < stageInfo.KillMonsterCount; i++)
			{
				Vector3 spawnPosition = Vector3.zero;

				// 플레이어와의 거리가 최소 5 이상일 때까지 위치를 찾기
				do
				{
					spawnPosition = GetRandomPositionInTileMap(_tileMap.cellBounds); // 타일맵 안의 랜덤 위치 얻기
				}
				while (Vector3.Distance(spawnPosition, heroPosition) < 5f); // 플레이어와의 거리가 5 미만이면 다시 찾음

				Managers.Object.Spawn<Monster>(spawnPosition, stageInfo.MonsterDataIdList[UnityEngine.Random.Range(0, stageInfo.MonsterDataIdList.Count)]);
			}
		}
		else // 보스 스테이지
		{
			Managers.Object.Spawn<BossMonster>(Vector3.zero, stageInfo.BossDataId);
		}
	}

	public void OnMonsterDestroyed()
	{
		_killMonsters += 1;
		// 몬스터가 파괴될 때마다 UI를 업데이트
		SetMonsterCount(_killMonsters, _maxMonsters);
	}

	public bool ClearStage()
	{
		return _killMonsters >= _maxMonsters;
	}

	// 타일맵 안에서 랜덤한 위치를 얻는 함수
	private Vector3 GetRandomPositionInTileMap(BoundsInt bounds)
	{
		// 타일맵의 셀 범위 안에서 랜덤한 X, Y 좌표 선택
		int randomX = UnityEngine.Random.Range(bounds.xMin, bounds.xMax); // 타일맵의 셀 X 범위에서 랜덤 값
		int randomY = UnityEngine.Random.Range(bounds.yMin, bounds.yMax); // 타일맵의 셀 Y 범위에서 랜덤 값

		// 랜덤 셀 좌표를 월드 좌표로 변환
		Vector3Int randomCellPosition = new Vector3Int(randomX, randomY, 0);
		Vector3 worldPosition = _tileMap.CellToWorld(randomCellPosition);

		// 반환할 때 Z 좌표는 0으로 고정
		return new Vector3(worldPosition.x, worldPosition.y, 0);
	}

	#endregion

}
