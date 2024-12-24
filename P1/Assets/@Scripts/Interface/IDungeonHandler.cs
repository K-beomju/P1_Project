using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonHandler
{
    /// <summary>
    /// 던전 초기에 필요한 리소스를 불러오고, 기본 상태를 세팅한다.
    /// </summary>
    void InitializeDungeon(DungeonScene scene);

    /// <summary>
    /// 던전에 맞게 몬스터(또는 월드보스)를 스폰한다.
    /// </summary>
    IEnumerator SpawnMonsters(DungeonScene scene);

    /// <summary>
    /// 던전이 Over(실패)상태가 되었을 때 처리한다.
    /// </summary>
    IEnumerator OnStageOver(DungeonScene scene);

    /// <summary>
    /// 던전이 Clear(성공)상태가 되었을 때 처리한다.
    /// </summary>
    IEnumerator OnStageClear(DungeonScene scene);

    /// <summary>
    /// 던전 진행 중 타이머를 갱신한다.
    /// </summary>
    void UpdateTimer(DungeonScene scene);
}