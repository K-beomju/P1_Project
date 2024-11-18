using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Data;
using DG.Tweening;
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

    public int ChapterLevel { get; private set; }
    public float BossBattleTimer { get; private set; }
    public float BossBattleTimeLimit { get; private set; }

    public StageInfoData StageInfo { get; private set; }
    private BackendData.GameData.CharacterData CharacterData;

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

        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeIn, 1f, 1f,
        fadeInCallBack: () =>
        {
            SetupStage();
        });
        //Managers.Object.Spawn<Bot>(new Vector3(2,2), 0);
        return true;
    }


    private void InitailizeBackend()
    {
        CharacterData = Managers.Backend.GameData.CharacterData;
        // 코루틴을 통한 정기 데이터 업데이트 시작
        StartCoroutine(Managers.Backend.UpdateGameDataTransaction());
        //StartCoroutine(Managers.Backend.UpdateRankScore());

    }

    private void InitializeGameComponents()
    {
        Managers.Hero.Init();
        Managers.Equipment.Init();
        Managers.Skill.Init();
    }

    private void InitializeScene()
    {
        GameObject map = Managers.Resource.Instantiate("BaseMap");
        PolygonCollider2D polygon = Util.FindChild(map, "Terrain_Tile").GetComponent<PolygonCollider2D>();
        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        cc.GetComponent<CinemachineConfiner>().m_BoundingShape2D = polygon;
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cc.Target = hero;
    }

    private void InitializeUI()
    {
        Managers.UI.CacheAllPopups();
        sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);

        // 데이터 불러온 뒤 UI 표시 부분
        Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);
        Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, CharacterData.Level, CharacterData.Exp, CharacterData.MaxExp); // 경험치 갱신 이벤트

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

            // State 변경시 변경되는 UI 로직 
            sceneUI.UpdateStageUI(GameSceneState);

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
            { EGameSceneState.RankUp, CoRankUpStage },
            { EGameSceneState.Over, CoStageOver },
            { EGameSceneState.Clear, CoStageClear }
        };
    }

    private void SetupStage()
    {
        ChapterLevel = 1;
        StageInfo = Managers.Data.StageChart[CharacterData.StageLevel]; //Managers.Data.StageDataDic[UserData.StageLevel];
        Debug.Log($"{StageInfo.StageNumber} 스테이지 진입");

        BossBattleTimeLimit = StageInfo.BossBattleTimeLimit;
        BossBattleTimer = BossBattleTimeLimit;

        GameSceneState = EGameSceneState.Play;

        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayStage(CharacterData.StageLevel);
    }

    #region GameSceneState

    private IEnumerator CoPlayStage()
    {
        Managers.Game.SetMonsterCount(0, StageInfo.KillMonsterCount);

        yield return StartWait;
        Managers.Game.SpawnStageMonster(StageInfo);

        // 몬스터가 스폰될 때 자동 스킬 조건을 다시 검사하도록 이벤트 트리거
        if (Managers.Backend.GameData.SkillInventory.IsAutoSkill)
            (Managers.UI.SceneUI as UI_GameScene).CheckUseSkillSlot(-1);
        

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
        Managers.Game.SpawnStageMonster(StageInfo, true);
        sceneUI.RefreshBossMonsterHp(Managers.Object.BossMonster);
        while (Managers.Object.BossMonster != null)
        {
            UpdateBossBattleTimer(); // 보스 타이머 업데이트 메서드 호출
            yield return null;
        }
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);
        GameSceneState = EGameSceneState.Clear;
    }

    private IEnumerator CoRankUpStage()
    {
        // 몬스터 다 없애고, 히어로 스탯 초기화 
        KillAllMonsters();
        Hero hero = Managers.Object.Hero;
        hero.Rebirth();
        hero.transform.position = Vector3.zero;
        yield return new WaitForSeconds(1);
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
        CharacterData.UpdateStageLevel(stageDelta);
        Debug.LogWarning($"다음 스테이지 {CharacterData.StageLevel} 입니다!");
        SetupStage();
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
            monster.isStopAI = true;
            Managers.Object.Despawn(monster);
        }

        if (Managers.Object.BossMonster != null)
            Managers.Object.Despawn(Managers.Object.BossMonster);

    }

    public string GetCurrentStage()
    {
        return $"{ChapterLevel}-{CharacterData.StageLevel}";
    }

    public override void Clear()
    {
        KillAllMonsters();
    }
}
