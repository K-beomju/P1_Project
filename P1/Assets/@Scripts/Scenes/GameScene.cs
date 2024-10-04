using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cinemachine;
using Data;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
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
    private IEnumerator CurrentCoroutine = null;
    private WaitForSeconds FrameWait = new WaitForSeconds(0.2f);
    private WaitForSeconds StartWait = new WaitForSeconds(1f);
    private UI_GameScene sceneUI;

    public StageInfoData Data { get; private set; }
    public int ChapterLevel { get; private set; }
    public int StageLevel { get; private set; }
    public float BossBattleTimer { get; private set; }
    public float BossBattleTimeLimit { get; private set; }
    private bool isClear;

    private delegate void BackendLoadStep();
    private readonly Queue<BackendLoadStep> _initializeStep = new Queue<BackendLoadStep>();


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Scene
        SceneType = EScene.GameScene;
        Managers.Scene.SetCurrentScene(this);

        // Server
        InitailizeBackend();
        // Data
        InitializeGameComponents();
        InitializeScene();
        InitializeUI();

        ChapterLevel = 1;
        isClear = false;
        SetupStage(1);
        return true;
    }

    private void InitailizeBackend()
    {

        // 코루틴을 통한 정기 데이터 업데이트 시작
        BackendManager.Instance.StartUpdate();


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

        GameObject map = Managers.Resource.Instantiate("BaseMap");
        PolygonCollider2D polygon = Util.FindChild(map, "Terrain_Tile").GetComponent<PolygonCollider2D>();
        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        cc.GetComponent<CinemachineConfiner>().m_BoundingShape2D = polygon;
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cc.Target = hero;
        Managers.Purse.Init();

    }

    private void InitializeUI()
    {
        Managers.UI.CacheAllPopups();
        sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);
        sceneUI.UpdateStageUI(false);

        Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);

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
        Managers.Game.SetMonsterCount(0, Data.KillMonsterCount);

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
        sceneUI.RefreshBossStageTimer(0, 0);
        MoveToNextStage(false);
        yield return null;
    }

    private IEnumerator CoStageClear()
    {
        MoveToNextStage(true);
        yield return null;
    }

    public void MoveToNextStage(bool isClear)
    {
        KillAllMonsters();
        StartCoroutine(MoveToStageAfterDelay(isClear ? 1 : -1));
    }

    private IEnumerator MoveToStageAfterDelay(int stageDelta)
    {
        yield return new WaitForSeconds(0.5f);
        StageLevel += stageDelta;
        if (StageLevel == 0)
            StageLevel = 1;
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
        for (int i = Managers.Object.Monsters.Count - 1; i >= 0; i--)
        {
            Monster monster = Managers.Object.Monsters.ElementAt(i);
            Managers.Object.Despawn(monster);
        }

        if (Managers.Object.BossMonster != null)
            Managers.Object.Despawn(Managers.Object.BossMonster);

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
