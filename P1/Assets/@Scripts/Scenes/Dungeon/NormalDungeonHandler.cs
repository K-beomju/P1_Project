using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class NormalDungeonHandler : IDungeonHandler
{
    public void InitializeDungeon(DungeonScene scene)
    {
        int dungeonLevel = Managers.Backend.GameData.DungeonData.DungeonLevelDic[scene.DungeonType.ToString()];
        
        // 골드/다이아에 맞는 차트 할당
        scene.DungeonInfo = scene.DungeonType switch
        {
            EDungeonType.Gold => Managers.Data.GoldDungeonChart[dungeonLevel],
            EDungeonType.Dia => Managers.Data.DiaDungeonChart[dungeonLevel],
            _ => throw new ArgumentException($"Unknown DungeonType: {scene.DungeonType}")
        };

        scene.sceneUI.UpdateStageUI(scene.DungeonType, scene.DungeonInfo.DungeonLevel);
        Managers.Game.SetMonsterCount(0, scene.DungeonInfo.KillMonsterCount);

        scene.DungeonTimeLimit = scene.DungeonInfo.DungeonTimeLimit;
        scene.DungeonTimer = scene.DungeonTimeLimit;
        UpdateTimer(scene);
    }

    public IEnumerator SpawnMonsters(DungeonScene scene)
    {
        // 처음에 영웅은 액션 불가
        Managers.Object.Hero.DisableAction();

        // 일반 던전 몬스터 스폰
        Managers.Game.SpawnDungeonMonster(dungeonInfo: scene.DungeonInfo);

        yield return new WaitForSeconds(1f);

        // 영웅 액션 가능
        Managers.Object.Hero.EnableAction();

        // (자동 스킬 체크)
        if (Managers.Backend.GameData.SkillInventory.IsAutoSkill)
            scene.sceneUI?.CheckUseSkillSlot(-1);
    }

    public IEnumerator OnStageOver(DungeonScene scene)
    {
        yield return new WaitForSeconds(1f);

        scene.sceneUI.RefreshDungeonTimer(0, 0);
        // 몬스터 멈추고, UI 팝업
        Managers.Object.Monsters.ToList().ForEach(x => x.DisableAction());
        var popupUI = Managers.UI.ShowPopupUI<UI_DungeonFailPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);

        // 던전 키 복원
        Managers.Backend.GameData.DungeonData.AddKey(scene.DungeonType, 1);
    }

    public IEnumerator OnStageClear(DungeonScene scene)
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DungeonClearPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);

        scene.clearRewardDic.Clear();
        scene.clearRewardDic.Add(scene.DungeonInfo.ItemType, scene.DungeonInfo.DungeonClearReward);
        popupUI.RefreshUI(scene.DungeonType, scene.clearRewardDic);

        Debug.Log($"던전 클리어! {scene.DungeonInfo.ItemType} {scene.DungeonInfo.DungeonClearReward} 보상 지급");

        // 캐릭터 보상 획득
        Managers.Backend.GameData.CharacterData.AddAmount(scene.DungeonInfo.ItemType, scene.DungeonInfo.DungeonClearReward);
        // 던전 레벨 증가
        Managers.Backend.GameData.DungeonData.IncreaseDungeonLevel(scene.DungeonType);

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
