using Cinemachine;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DungeonScene : BaseScene
{
    private EGameSceneState _gameSceneState = EGameSceneState.None;
    public EGameSceneState GameSceneState
    {
        get => _gameSceneState;
        set
        {
            if (_gameSceneState != value)
            {
                _gameSceneState = value;
                SwitchCoroutine();
            }
        }
    }

    private Dictionary<EGameSceneState, Func<IEnumerator>> _stateCoroutines;
    private IEnumerator _currentCoroutine = null;
    private EDungeonType DungeonType;
    private UI_DungeonScene sceneUI;

    public DungeonInfoData DungeonInfo { get; private set; }
    private BackendData.GameData.CharacterData CharacterData;
    private Dictionary<EItemType, int> clearRewardDic = new Dictionary<EItemType, int>();

    public float DungeonTimer { get; private set; }
    public float DungeonTimeLimit { get; private set; }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.DungeonScene;
        DungeonType = Managers.Game.GetCurrentDungeon();
        Managers.Scene.SetCurrentScene(this);

        InitailizeBackend();

        InitializeScene();
        InitializeUI();
        InitializeDungeon();

        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeIn, 1f, 1f,
        fadeInCallBack: () =>
        {
            GameSceneState = EGameSceneState.Play;
        });

        return true;
    }

    private void InitailizeBackend()
    {
        CharacterData = Managers.Backend.GameData.CharacterData;
    }

    private void InitializeScene()
    {
        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cc.Target = hero;
    }

    private void InitializeUI()
    {
        sceneUI = Managers.UI.ShowSceneUI<UI_DungeonScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);

        Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, CharacterData.Level, CharacterData.Exp, CharacterData.MaxExp); // 경험치 갱신 이벤트

    }

    private void InitializeDungeon()
    {
        DungeonInfo = DungeonType switch
        {
            EDungeonType.Gold => Managers.Data.GoldDungeonChart[Managers.Backend.GameData.DungeonData.DungeonLevelDic[DungeonType.ToString()]],
            EDungeonType.Dia => Managers.Data.DiaDungeonChart[Managers.Backend.GameData.DungeonData.DungeonLevelDic[DungeonType.ToString()]],
            _ => throw new ArgumentException($"Unknown DungeonType: {DungeonType}")
        };

        Debug.Log(DungeonInfo.ItemType);

        sceneUI.UpdateStageUI(DungeonType, DungeonInfo.DungeonLevel);
        Managers.Game.SetMonsterCount(0, DungeonInfo.KillMonsterCount);

        DungeonTimeLimit = DungeonInfo.DungeonTimeLimit;
        DungeonTimer = DungeonTimeLimit;
        UpdateDungeonTimer();
    }


    private void SwitchCoroutine()
    {
        if (_stateCoroutines == null)
            InitializeStateCoroutines();

        if (_stateCoroutines.TryGetValue(GameSceneState, out var coroutineFunc))
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            _currentCoroutine = coroutineFunc();
            StartCoroutine(_currentCoroutine);
        }
    }


    private void InitializeStateCoroutines()
    {
        _stateCoroutines = new Dictionary<EGameSceneState, Func<IEnumerator>>
        {
            { EGameSceneState.Play, CoPlayStage },
            { EGameSceneState.Pause, CoPauseStage },
            { EGameSceneState.Over, CoStageOver },
            { EGameSceneState.Clear, CoStageClear }
        };
    }


    #region GameSceneState

    private IEnumerator CoPlayStage()
    {
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayDungeon(DungeonType, DungeonInfo.DungeonLevel);
        yield return new WaitForSeconds(2f);
        Managers.Game.SpawnDungeonMonster(DungeonInfo);

        // 몬스터가 스폰될 때 자동 스킬 조건을 다시 검사하도록 이벤트 트리거
        if (Managers.Backend.GameData.SkillInventory.IsAutoSkill)
            (Managers.UI.SceneUI as UI_DungeonScene).CheckUseSkillSlot(-1);

        while (DungeonTimer > 0)
        {
            UpdateDungeonTimer(); // 보스 타이머 업데이트 메서드 호출

            if (Managers.Game.ClearStage())
            {
                GameSceneState = EGameSceneState.Clear;
            }
            yield return null;
        }
    }

    private IEnumerator CoPauseStage()
    {
        yield return null;
    }

    private IEnumerator CoStageOver()
    {
        Debug.Log("던전 오버!");

        sceneUI.RefreshDungeonTimer(0, 0);
        yield return null;
    }

    private IEnumerator CoStageClear()
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DungeonClearPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
        clearRewardDic.Add(DungeonInfo.ItemType, DungeonInfo.DungeonClearReward);
        popupUI.RefreshUI(DungeonType, clearRewardDic);

        Debug.Log($"던전 클리어! {DungeonInfo.ItemType} {DungeonInfo.DungeonClearReward} 보상 지급");
        Managers.Backend.GameData.CharacterData.AddAmount(DungeonInfo.ItemType, DungeonInfo.DungeonClearReward);
        Managers.Backend.GameData.DungeonData.IncreaseDungeonLevel(DungeonType);
        // 팝업 띄우고 보상 주고 다시 게임씬으로 
        yield return null;
    }

    #endregion


    private void UpdateDungeonTimer()
    {
        DungeonTimer = Mathf.Clamp(DungeonTimer - Time.deltaTime, 0.0f, DungeonTimeLimit);
        sceneUI.RefreshDungeonTimer(DungeonTimer, DungeonTimeLimit);

        if (DungeonTimer <= 0)
        {
            GameSceneState = EGameSceneState.Over;
        }
    }


    public override void Clear()
    {

    }
}
