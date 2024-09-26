using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Data;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public StageInfoData Data { get; private set; }
    public int ChapterLevel { get; private set; }
    public int StageLevel { get; private set; }
    public float BossBattleTimer { get; private set; }
    public float BossBattleTimeLimit { get; private set; }

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
    private IEnumerator CurrentCoroutine = null;
    private WaitForSeconds FrameWait = new WaitForSeconds(0.2f);
    private WaitForSeconds StartWait = new WaitForSeconds(1f);
    private UI_GameScene sceneUI;
    private bool isClear;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Scene
        SceneType = EScene.GameScene;
        Managers.Scene.SetCurrentScene(this);

        // Data
        InitializeGameComponents();
        InitializeScene();
        InitializeUI();

        ChapterLevel = 1;
        isClear = false;
        SetupStage(1);
        return true;
    }

    private void InitializeGameComponents()
    {
        Managers.Data.Init();
        Managers.Game.Init();
        Managers.Hero.Init();
        Managers.Equipment.Init();
    }

    private void InitializeScene()
    {
        SceneType = EScene.GameScene;
        Managers.Scene.SetCurrentScene(this);
        Managers.Resource.Instantiate("BaseMap");
        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cc.Target = hero;
        Managers.Purse.Init();

    }

    private void InitializeUI()
    {
        Managers.UI.CacheAllPopups();
        sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_GAMESCENE);
        sceneUI.UpdateStageUI(false);
    }


    private void SwitchCoroutine()
    {
        if (_stateCoroutines == null)
            InitializeStateCoroutines();

        if (_stateCoroutines.TryGetValue(GameSceneState, out var coroutineFunc))
        {
            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
            }
            CurrentCoroutine = coroutineFunc();
            StartCoroutine(CurrentCoroutine);
        }
    }

    private void InitializeStateCoroutines()
    {
        _stateCoroutines = new Dictionary<EGameSceneState, Func<IEnumerator>>
        {
            { EGameSceneState.Play, CoPlayStage },
            { EGameSceneState.Pause, CoPauseStage },
            { EGameSceneState.Boss, CoBossStage },
            { EGameSceneState.Over, CoStageOver },
            { EGameSceneState.Clear, CoStageClear }
        };
    }

    private void SetupStage(int stageLevel)
    {
        isClear = false;
        StageLevel = stageLevel;
        Data = Managers.Data.StageDataDic[StageLevel];
        Debug.Log($"{stageLevel} 스테이지 진입");
     
        BossBattleTimeLimit = Data.BossBattleTimeLimit;
        BossBattleTimer = BossBattleTimeLimit;
        
        GameSceneState = EGameSceneState.Play;

        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().RefreshShowDisplayStage(StageLevel);
    }

    #region GameSceneState

    private IEnumerator CoPlayStage()
    {
        yield return StartWait;
        sceneUI.UpdateStageUI(false);
        Managers.Game.SpawnMonster(Data);

        while (!Managers.Game.ClearStage())
        {
            yield return FrameWait;
        }

        GameSceneState = EGameSceneState.Boss;
    }

    private IEnumerator CoPauseStage()
    {
        yield return null;
    }

    private IEnumerator CoBossStage()
    {
        yield return StartWait;
        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeIn(0.5f);
        sceneUI.UpdateStageUI(true);

        Managers.Game.SpawnMonster(Data, true);
        sceneUI.RefreshBossMonsterHp(Managers.Object.BossMonster);
        while (Managers.Object.BossMonster != null)
        {
            UpdateBossBattleTimer(); // 보스 타이머 업데이트 메서드 호출
            yield return null;
        }
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);
        GameSceneState = EGameSceneState.Clear;
    }

    private IEnumerator CoStageOver()
    {
        MoveToNextStage(false);
        yield return null;
    }

    private IEnumerator CoStageClear()
    {
        MoveToNextStage(true);
        yield return null;
    }

    private void MoveToNextStage(bool isClear)
    {
        KillAllMonsters();
        StartCoroutine(MoveToStageAfterDelay(isClear ? 1 : -1));
    }

    private IEnumerator MoveToStageAfterDelay(int stageDelta)
    {
        yield return new WaitForSeconds(0.5f);
        StageLevel += stageDelta;
        Debug.LogWarning($"다음 스테이지 {StageLevel} 입니다!");
        SetupStage(StageLevel);
        GameSceneState = EGameSceneState.Play;
    }

    #endregion

    private void UpdateBossBattleTimer()
    {
        BossBattleTimer = Mathf.Clamp(BossBattleTimer - Time.deltaTime, 0.0f, BossBattleTimeLimit);
        sceneUI.RefreshBossStageTimer(BossBattleTimer, BossBattleTimeLimit);

        if (BossBattleTimer <= 0 && Managers.Object.BossMonster != null)
        {
            GameSceneState = EGameSceneState.Over;
        }
    }

    private void KillAllMonsters()
    {
        foreach (Monster monster in Managers.Object.Monsters)
        {
            monster.CreatureState = ECreatureState.Dead;
        }
        if (Managers.Object.BossMonster != null)
            Managers.Object.BossMonster.CreatureState = ECreatureState.Dead;
    }


    #region ContextMenu Methods

    [ContextMenu("Stage/StageClear")]
    public void StageClearCM()
    {
        GameSceneState = EGameSceneState.Clear;
    }

    #endregion

    public string GetCurrentStage()
    {
        return $"{ChapterLevel}-{StageLevel}";
    }

    public override void Clear()
    {

    }
}
