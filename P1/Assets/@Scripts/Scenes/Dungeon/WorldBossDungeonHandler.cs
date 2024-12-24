

using System.Collections;
using UnityEngine;
using static Define;

public class WorldBossDungeonHandler : IDungeonHandler
{
    public void InitializeDungeon(DungeonScene scene)
    {
        // 월드보스 차트
        scene.WorldDungeonInfo = Managers.Data.WorldBossDungeonChart[1];

        scene.sceneUI.UpdateStageUI(scene.DungeonType, 0);
        Managers.Game.SetMonsterCount(0, 0);

        scene.DungeonTimeLimit = scene.WorldDungeonInfo.DungeonTimeLimit;
        scene.DungeonTimer = scene.WorldDungeonInfo.DungeonTimeLimit;
        UpdateTimer(scene);
    }

    public IEnumerator SpawnMonsters(DungeonScene scene)
    {
        // 영웅 액션 불가
        Managers.Object.Hero.DisableAction();

        // 월드보스 소환
        Managers.Game.SpawnDungeonMonster(isBoss: true);
        Managers.Object.WorldBoss.DisableAction();

        yield return new WaitForSeconds(1f);

        // 월드보스 액션 가능
        Managers.Object.WorldBoss.EnableAction();
        // 영웅 액션 가능
        Managers.Object.Hero.EnableAction();

        // (자동 스킬 체크)
        if (Managers.Backend.GameData.SkillInventory.IsAutoSkill)
            (scene.sceneUI as UI_DungeonScene)?.CheckUseSkillSlot(-1);
    }

    public IEnumerator OnStageOver(DungeonScene scene)
    {
        yield return new WaitForSeconds(1f);
        scene.sceneUI.RefreshDungeonTimer(0, 0);

        // 월드보스 데미지 팝업 & 랭킹 등록 처리
        var userData = Managers.Backend.GameData.CharacterData;
        int endTotalWorldBossDmg = Managers.Object.WorldBoss.worldBossTotalDamage;
        int currentTotalWorldBossDmg = userData.WorldBossCombatPower;

        if (currentTotalWorldBossDmg < endTotalWorldBossDmg || currentTotalWorldBossDmg == 0)
        {
            userData.UpdateWorldBossCombatPower(endTotalWorldBossDmg);
        }

        var popupUI = Managers.UI.ShowPopupUI<UI_BattleResultPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
        popupUI.RefreshUI(endTotalWorldBossDmg);
    }

    public IEnumerator OnStageClear(DungeonScene scene)
    {
        // 월드보스 던전은 '클리어' 개념을 별도로 만들 수도 있지만
        // 필요하다면 이곳에 보상 지급 로직을 넣어도 무방
        yield return null;
    }

    public void UpdateTimer(DungeonScene scene)
    {
        scene.DungeonTimer = Mathf.Clamp(scene.DungeonTimer - Time.deltaTime, 0f, scene.DungeonTimeLimit);
        scene.sceneUI.RefreshDungeonTimer(scene.DungeonTimer, scene.DungeonTimeLimit);

        if (scene.DungeonTimer <= 0)
        {
            scene.GameSceneState = EGameSceneState.Over;
        }
    }
}
