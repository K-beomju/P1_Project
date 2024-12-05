using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class GameManager
{
    private Tilemap tileMap;
    public Tilemap TileMap
    {
        get
        {
            if (tileMap == null)
            {
                tileMap = Util.FindChild(GameObject.Find("BaseMap"), "Terrain_Tile", true).GetComponent<Tilemap>();
                tileMap.CompressBounds();
            }
            return tileMap;
        }
    }

    private int _killMonsters;
    private int _maxMonsters;
    private EDungeonType DungeonType;

    public void SetMonsterCount(int killMonsters, int maxMonsters)
    {
        _killMonsters = killMonsters;
        _maxMonsters = maxMonsters;

        Managers.Event.TriggerEvent(EEventType.MonsterCountChanged, _killMonsters, _maxMonsters);
    }

    public void SpawnStageMonster(StageInfoData stageInfo, bool isBoss = false)
    {
        if (tileMap == null)
        {
            tileMap = Util.FindChild(GameObject.Find("BaseMap"), "Terrain_Tile", true).GetComponent<Tilemap>();
            tileMap.CompressBounds();
        }

        Vector3 heroPosition = Managers.Object.Hero.transform.position;

        if (!isBoss)
        {
            for (int i = 0; i < stageInfo.KillMonsterCount; i++)
            {
                Vector3 spawnPosition;
                do
                {
                    spawnPosition = GetRandomPositionInTileMap(tileMap.cellBounds,2);
                }
                while (Vector3.Distance(spawnPosition, heroPosition) < 5f);

                Managers.Object.Spawn<Monster>(spawnPosition, stageInfo.MonsterDataIdList[Random.Range(0, stageInfo.MonsterDataIdList.Count)]);
            }
        }
        else
        {
            Managers.Object.Spawn<BossMonster>(new Vector3(3,0,0), stageInfo.BossDataId);
        }
    }


    public void SpawnDungeonMonster(DungeonInfoData dungeonInfo = null, bool isBoss = false)
    {
        if (!isBoss)
        {
            for (int i = 0; i < dungeonInfo.KillMonsterCount; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
                Managers.Object.Spawn<Monster>(spawnPosition, dungeonInfo.MonsterDataIdList[Random.Range(0, dungeonInfo.MonsterDataIdList.Count)]);
            }
        }
        else
        {
            Managers.Object.Spawn<WorldBoss>(new Vector3(0, 2, 0), 0);
        }
    }

    public void OnMonsterDestroyed()
    {
        _killMonsters += 1;
        SetMonsterCount(_killMonsters, _maxMonsters);
    }

    public bool ClearStage()
    {
        return _killMonsters >= _maxMonsters;
    }

    public EDungeonType GetCurrentDungeon()
    {
        return DungeonType;
    }

    public void SetCurrentDungeon(EDungeonType dungeon)
    {
        DungeonType = dungeon;
    }

    public void Clear()
    {
        DungeonType = EDungeonType.Unknown;
    }

    private Vector3 GetRandomPositionInTileMap(BoundsInt bounds, float padding)
    {
        // 보정값 적용
        int minX = bounds.xMin + Mathf.CeilToInt(padding);
        int maxX = bounds.xMax - Mathf.CeilToInt(padding);
        int minY = bounds.yMin + Mathf.CeilToInt(padding);
        int maxY = bounds.yMax - Mathf.CeilToInt(padding);

        // 랜덤한 셀 위치 생성
        int randomX = Random.Range(minX, maxX);
        int randomY = Random.Range(minY, maxY);

        // 셀 위치를 월드 좌표로 변환
        Vector3Int randomCellPosition = new Vector3Int(randomX, randomY, 0);
        Vector3 worldPosition = tileMap.CellToWorld(randomCellPosition);

        return new Vector3(worldPosition.x, worldPosition.y, 0);
    }
}
