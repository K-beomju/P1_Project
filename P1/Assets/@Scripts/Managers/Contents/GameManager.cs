using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class GameManager
{
    private Tilemap _tileMap;
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
        if (_tileMap == null)
        {
            _tileMap = Util.FindChild(GameObject.Find("BaseMap"), "Terrain_Tile", true).GetComponent<Tilemap>();
            _tileMap.CompressBounds();
        }

        Vector3 heroPosition = Managers.Object.Hero.transform.position;

        if (!isBoss)
        {
            for (int i = 0; i < stageInfo.KillMonsterCount; i++)
            {
                Vector3 spawnPosition;
                do
                {
                    spawnPosition = GetRandomPositionInTileMap(_tileMap.cellBounds);
                }
                while (Vector3.Distance(spawnPosition, heroPosition) < 5f);

                Managers.Object.Spawn<Monster>(spawnPosition, stageInfo.MonsterDataIdList[Random.Range(0, stageInfo.MonsterDataIdList.Count)]);
            }
        }
        else
        {
            Managers.Object.Spawn<BossMonster>(Vector3.zero, stageInfo.BossDataId);
        }
    }

    public void SpawnDungeonMonster(DungeonInfoData dungeonInfo, bool isBoss = false)
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
            Managers.Object.Spawn<WorldBoss>(new Vector3(0, 2, 0), dungeonInfo.BossDataId);
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

    private Vector3 GetRandomPositionInTileMap(BoundsInt bounds)
    {
        int randomX = Random.Range(bounds.xMin, bounds.xMax);
        int randomY = Random.Range(bounds.yMin, bounds.yMax);

        Vector3Int randomCellPosition = new Vector3Int(randomX, randomY, 0);
        Vector3 worldPosition = _tileMap.CellToWorld(randomCellPosition);

        return new Vector3(worldPosition.x, worldPosition.y, 0);
    }
}
