using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Data;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public StageInfoData Data { get; private set; }
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

    private IEnumerator CurrentCoroutine = null;
    private WaitForSeconds FrameWait;
    private WaitForSeconds StartWait;

    private UI_GameScene sceneUI;
    private bool isClear;

    private void SwitchCoroutine()
    {
        IEnumerator coroutine = null;

        switch (GameSceneState)
        {
            case EGameSceneState.Play:
                coroutine = CoPlayStage();
                break;
            case EGameSceneState.Pause:
                coroutine = CoPauseStage();
                break;
            case EGameSceneState.Boss:
                coroutine = CoBossStage();
                break;
            case EGameSceneState.Over:
                coroutine = CoStageOver();
                break;
            case EGameSceneState.Clear:
                coroutine = CoStageClear();
                break;
        }

        if (CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
        }

        CurrentCoroutine = coroutine;
        StartCoroutine(CurrentCoroutine);
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;
        Managers.Scene.SetCurrentScene(this);

        // Data
        Managers.Data.Init();

        // Scene Setting
        Managers.Resource.Instantiate("BaseMap");
        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cc.Target = hero;

        // UI
        Managers.UI.CacheAllPopups();
        sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, 100);
        Managers.Purse.Init();



        isClear = false;
        FrameWait = new WaitForSeconds(0.2f);
        StartWait = new WaitForSeconds(1f);

        SetupStage(1);
        sceneUI.ShowNormalOrBossStageUI(false);
        GameSceneState = EGameSceneState.Play;
        return true;
    }

    private void SetupStage(int stageLevel)
    {
        isClear = false;
        StageLevel = stageLevel;
        Data = Managers.Data.StageDic[StageLevel];
        Debug.Log($"{stageLevel} 스테이지 진입");

        if (Data.StageType == EStageType.NormalStage)
        {
            Debug.Log($"노말 스테이지 로드");
        }
        else if (Data.StageType == EStageType.BossStage)
        {
            Debug.Log($"보스 스테이지 로드");
            BossBattleTimeLimit = Data.BossBattleTimeLimit;
            BossBattleTimer = BossBattleTimeLimit;
        }
        GameSceneState = Data.StageType == EStageType.NormalStage ?
        EGameSceneState.Play : EGameSceneState.Boss;

        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().RefreshShowDisplayStage(StageLevel);
    }

    #region GameSceneState

    private IEnumerator CoPlayStage()
    {
        yield return StartWait;
        sceneUI.ShowNormalOrBossStageUI(false);

        Managers.Game.SpawnMonster(Data);

        while (!isClear)
        {
            if (Managers.Object.Monsters.Count == 0)
                isClear = true;

            yield return FrameWait;
        }

        GameSceneState = EGameSceneState.Clear;
    }

    private IEnumerator CoPauseStage()
    {
        yield return null;
    }

    private IEnumerator CoBossStage()
    {
        yield return StartWait;
        sceneUI.ShowNormalOrBossStageUI(true);

        Managers.Game.SpawnMonster(Data, true);
        while (Managers.Object.BossMonster != null)
        {
            UpdateBossBattleTimer(); // 보스 타이머 업데이트 메서드 호출
            yield return null;
        }

        GameSceneState = EGameSceneState.Clear;
    }

    private IEnumerator CoStageOver()
    {
        KillAllMonsters();
        yield return new WaitForSeconds(0.5f);
        --StageLevel;
        Debug.LogWarning($"다음 스테이지 {StageLevel} 입니다!");
        SetupStage(StageLevel);
        GameSceneState = StageLevel % 5 == 0
        ? EGameSceneState.Boss : EGameSceneState.Play;

    }

    private IEnumerator CoStageClear()
    {
        KillAllMonsters();
        yield return new WaitForSeconds(0.5f);

        ++StageLevel;
        Debug.LogWarning($"다음 스테이지 {StageLevel} 입니다!");
        SetupStage(StageLevel);
    }

    #endregion

    private void UpdateBossBattleTimer()
    {
        BossBattleTimer = Mathf.Clamp(BossBattleTimer - Time.deltaTime, 0.0f, BossBattleTimeLimit);
        sceneUI.RefreshBossStageTimer(BossBattleTimer, BossBattleTimeLimit);

        if (BossBattleTimer <= 0)
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

    [ContextMenu("Stage/NextBossStage")]
    public void NextBossStageCM()
    {
        StageLevel = (Mathf.FloorToInt(StageLevel / 5.0f) * 5) + 4;
        GameSceneState = EGameSceneState.Clear;
    }

    #endregion

    public override void Clear()
    {

    }
}
