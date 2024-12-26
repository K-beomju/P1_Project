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
    #region Fields

    // - - - 상태 관련 - - -
    private EGameSceneState _gameSceneState = EGameSceneState.None;
    private Dictionary<EGameSceneState, Func<IEnumerator>> _stateCoroutines;
    private IEnumerator CurrentCoroutine = null;

    // - - - 대기시간 캐시 - - -
    private readonly WaitForSeconds FrameWait = new WaitForSeconds(0.2f);
    private readonly WaitForSeconds StartWait = new WaitForSeconds(1f);

    // - - - UI & Scene - - -
    private UI_GameScene sceneUI;
    private CameraController cameraController;

    // - - - 스테이지 정보 - - -
    public int ChapterLevel { get; private set; }
    public float BossBattleTimer { get; private set; }
    public float BossBattleTimeLimit { get; private set; }
    private bool isStaying = false;

    public StageInfoData StageInfo { get; private set; }
    private BackendData.GameData.CharacterData CharacterData;

    #endregion


    #region Properties

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

    #endregion


    #region Unity Lifecycle

    protected override bool Init()
    {
        if (!base.Init())
            return false;

        // Scene
        SceneType = EScene.GameScene;
        Managers.Scene.SetCurrentScene(this);

        // Data
        CharacterData = Managers.Backend.GameData.CharacterData;
        InitializeGameComponents();
        InitializeScene();
        InitializeUI();

        // 페이드 인 후 처리
        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeIn, 1f, 1f,
            fadeInCallBack: () =>
            {
                if (PlayerPrefs.GetInt("ShowDialogue", 0) == 0)
                {
                    // 최초 대화창 한 번만
                    PlayerPrefs.SetInt("ShowDialogue", 1);
                    var popupUI = Managers.UI.ShowPopupUI<UI_DialoguePopup>();
                    Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
                    popupUI.RefreshUI();
                }
                else
                {
                    SetupStage();
                }
            }
        );

        return true;
    }

    public override void Clear()
    {
        Managers.Object.KillAllMonsters();
    }

    #endregion


    #region Initialize Methods

    private void InitializeGameComponents()
    {
        Managers.Hero.Init();
        Managers.Equipment.Init();
        Managers.Skill.Init();
        Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
    }

    private void InitializeScene()
    {
        GameObject map = Managers.Resource.Instantiate("BaseMap");
        PolygonCollider2D polygon = Util.FindChild(map, "Terrain_Tile").GetComponent<PolygonCollider2D>();

        cameraController = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        cameraController.GetComponent<CinemachineConfiner>().m_BoundingShape2D = polygon;

        // 영웅 스폰
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cameraController.Target = hero.transform;

        // 펫, 광고 등
        Managers.Pet.Init();
        Managers.Ad.LoadBannerAd();
    }

    private void InitializeUI()
    {
        Managers.UI.CacheAllPopups();
        sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);

        // 대화 본 뒤 출석체크
        if (PlayerPrefs.GetInt("ShowDialogue", 0) == 1)
        {
            if (CharacterData.AttendanceCheck())
            {
                Debug.Log("하루가 지나 출석체크 팝업 On");
                var popupUI = Managers.UI.ShowPopupUI<UI_AttendancePopup>();
                Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SETTING_CONTENT_POPUP);
                popupUI.RefreshUI();
            }
        }

        // UI 갱신
        Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);
        Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, 
                                   CharacterData.Level, 
                                   CharacterData.Exp, 
                                   CharacterData.MaxExp);
        Managers.Event.TriggerEvent(EEventType.QuestCheckNotification);
        Managers.Event.TriggerEvent(EEventType.MissionItemUpdated);
        Managers.Event.TriggerEvent(EEventType.MyRankingUpdated);
    }

    #endregion


    #region State Coroutines

    private void SwitchCoroutine()
    {
        if (_stateCoroutines == null)
            InitializeStateCoroutines();

        if (_stateCoroutines.TryGetValue(GameSceneState, out var coroutineFunc))
        {
            // 이미 돌고있던 코루틴 중지
            if (CurrentCoroutine != null)
                StopCoroutine(CurrentCoroutine);

            CurrentCoroutine = coroutineFunc();

            // 상태 변경 시, UI 로직 갱신
            sceneUI.UpdateStageUI(GameSceneState);

            // 새 코루틴 실행
            StartCoroutine(CurrentCoroutine);
        }
    }

    private void InitializeStateCoroutines()
    {
        _stateCoroutines = new Dictionary<EGameSceneState, Func<IEnumerator>>
        {
            { EGameSceneState.Play,   CoPlayStage },
            { EGameSceneState.Pause,  CoPauseStage },
            { EGameSceneState.Boss,   CoBossStage },
            { EGameSceneState.RankUp, CoRankUpStage },
            { EGameSceneState.Stay,   CoStayStage },
            { EGameSceneState.Over,   CoStageOver },
            { EGameSceneState.Clear,  CoStageClear }
        };
    }

    #endregion


    #region Normal Stage

    /// <summary> 스테이지 세팅 (일반 스테이지 진입) </summary>
    public void SetupStage()
    {
        ChapterLevel = 1;
        StageInfo = Managers.Data.StageChart[CharacterData.StageLevel];
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayStage(CharacterData.StageLevel);
        GameSceneState = EGameSceneState.Play;
    }

    /// <summary> 특정 스테이지에 머무는 모드 (무한 리스폰 등) </summary>
    public void StayStage(int stageLevel)
    {
        isStaying = true;
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);
        Managers.Object.KillAllMonsters();

        StageInfo = Managers.Data.StageChart[stageLevel];
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayStage(stageLevel);
        GameSceneState = EGameSceneState.Stay;
    }

    private IEnumerator CoPlayStage()
    {
        Managers.Game.SetMonsterCount(0, StageInfo.KillMonsterCount);

        yield return StartWait;
        Managers.Game.SpawnStageMonster(StageInfo);

        // 몬스터를 다 잡을 때까지 대기
        while (!Managers.Game.ClearStage())
            yield return FrameWait;


        // 다 잡았다면 보스 몬스터 로드 

        GameSceneState = EGameSceneState.Boss;
    }

    private IEnumerator CoPauseStage()
    {
        // 일시정지 상태 (필요시 처리)
        yield return null;
    }

    #endregion


    #region Boss Stage

    private IEnumerator CoBossStage()
    {
        ResetStageAndHero();
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplay("보스를 처치하세요!");

        /////////////////////////////////////////////////////////////
        // 보스 몬스터 소환
        BossBattleTimeLimit = StageInfo.BossBattleTimeLimit;
        BossBattleTimer = BossBattleTimeLimit;
        UpdateBossBattleTimer();

        Managers.Game.SpawnStageMonster(StageInfo, isBoss: true);
        var bossMonster = Managers.Object.BossMonster;
        bossMonster.DisableAction();
        sceneUI.RefreshBossMonsterHp(bossMonster);

        var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
        Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_POPUP - 1);
        fadeUI.ShowFadeInOut(EFadeType.FadeIn, 1f, 1f, 1f, fadeInCallBack: () => 
        {
            // 전투 연출 후 본격 전투
            cameraController.Target = Managers.Object.Hero.transform;
            bossMonster.EnableAction();
            Managers.Object.Hero.EnableAction();

        });
        cameraController.Target = bossMonster.transform;
        Managers.Object.Hero.LookAt(bossMonster.transform.position);
        /////////////////////////////////////////////////////////////

        // 보스 스테이지 표시
        Managers.UI.ShowBaseUI<UI_BossStageDisplayBase>().ShowDisplay();
        yield return MonitorBossMonsterBattle();
    }

    private IEnumerator MonitorBossMonsterBattle()
    {
        // 보스가 살아있는지 검사
        while (Managers.Object.BossMonster.IsValid())
        {
            if (UpdateBossBattleTimer())
            {
                // 시간 만료 -> 움직이지 못하도록
                Managers.Object.BossMonster.DisableAction();
                Managers.Object.Hero.DisableAction();
                yield break;
            }
            yield return null;
        }

        // 보스 처치 성공
        yield return ClearBossStage();
    }

    private IEnumerator ClearBossStage()
    {
        // 보스 처치 성공
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);

        var mainCamera = Camera.main;
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane));
        Managers.Object.SpawnGameObject(screenCenter, "Object/ConfettiBurst")
                      .GetComponent<ParticleSystem>().Play();

        Managers.UI.ShowBaseUI<UI_StageClearBase>().ShowStageClearUI();

        yield return new WaitForSeconds(1);

        // 페이드 아웃 후 Clear 상태
        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(
            EFadeType.FadeInOut, 1f, 1f, 1f,
            fadeOutCallBack: () =>
            {
                GameSceneState = EGameSceneState.Clear;
                Managers.Object.Hero.Rebirth(true);
            }
        );
    }

    #endregion


    #region RankUp Stage

    private IEnumerator CoRankUpStage()
    {
        ResetStageAndHero();

        // RankUp 몬스터 생성
        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Pending);
        var rankUpInfo = Managers.Data.RankUpChart[rankType];
        SetupRankUpStage(rankUpInfo);

        yield return new WaitForSeconds(2);

        // 연출
        cameraController.Target = Managers.Object.RankMonster.transform;
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>()
                  .ShowDisplayRankUp(Managers.Data.RankUpMonsterChart[rankUpInfo.MonsterDataId].Name);

        yield return new WaitForSeconds(3);

        cameraController.Target = Managers.Object.Hero.transform;
        Managers.Object.Hero.EnableAction();

        yield return MonitorRankUpMonsterBattle();
    }

    private IEnumerator MonitorRankUpMonsterBattle()
    {
        while (Managers.Object.RankMonster.CreatureState != ECreatureState.Dead)
        {
            if (UpdateBossBattleTimer())
            {
                // 시간 만료 -> 전투 중단
                Managers.Object.RankMonster.DisableAction();
                Managers.Object.Hero.DisableAction();
                yield break;
            }
            yield return null;
        }

        // 랭크업 보스 처치 성공
        ClearRankUpStage();
    }
    
    private void ClearRankUpStage()
    {
        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Pending);
        Managers.Backend.GameData.RankUpData.UpdateRankUp(rankType);
        Managers.Backend.GameData.QuestData.UpdateQuest(EQuestType.HeroRankUp);

        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(
            EFadeType.FadeInOut, 1f, 1f, 1,
            () =>
            {
                Managers.Object.DeleteRankMonster();
                GameSceneState = EGameSceneState.Over;
                Managers.Object.Hero.Rebirth(true);
            },
            () =>
            {
                // UI 처리
                (Managers.UI.SceneUI as UI_GameScene)?.UpdateMyRank();
                Managers.UI.ShowPopupUI<UI_RankUpClearPopup>().RefreshUI();
                Managers.Event.TriggerEvent(EEventType.HeroRankChallenging, false);
            }
        );
    }


    private void SetupRankUpStage(RankUpInfoData rankUpInfo)
    {
        BossBattleTimeLimit = rankUpInfo.BossBattleTimeLimit;
        BossBattleTimer = BossBattleTimeLimit;
        sceneUI.RefreshBossStageTimer(BossBattleTimer, BossBattleTimeLimit);

        // 몬스터 스폰
        RankMonster monster = Managers.Object.SpawnRankMonster(new Vector3(3, 0, 0), rankUpInfo.MonsterDataId);
        Managers.Object.Hero.LookAt(monster.transform.position);
        sceneUI.RefreshBossMonsterHp(Managers.Object.RankMonster);
    }

    #endregion


    #region Stay Stage

    private IEnumerator CoStayStage()
    {
        // 무한 몬스터 스폰 스테이지
        Managers.Game.SetMonsterCount(1, 1);

        while (true)
        {
            // 스테이지 시작 대기
            yield return StartWait;
            Managers.Game.SpawnStageMonster(StageInfo);

            int initialMonsterCount = Managers.Object.Monsters.Count;
            while (true)
            {
                // 현재 몬스터 수가 초반 절반 이하가 되면 새 몬스터 소환
                if (Managers.Object.Monsters.Count <= initialMonsterCount / 2)
                {
                    Managers.Game.SpawnStageMonster(StageInfo);
                    initialMonsterCount = Managers.Object.Monsters.Count;
                }
                yield return FrameWait;
            }
        }
    }

    #endregion


    #region Over & Clear Stage

    private IEnumerator CoStageOver()
    {
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);
        MoveToNextStage(false);
        yield return null;
    }

    private IEnumerator CoStageClear()
    {
        MoveToNextStage(true);

        // 스킬 해금 & 퀘스트 갱신
        Managers.Backend.GameData.SkillInventory.UnLockSkill(StageInfo.StageNumber);
        Managers.Backend.GameData.QuestData.UpdateQuest(EQuestType.StageClear);
        yield return null;
    }

    public void MoveToNextStage(bool isClear)
    {
        Managers.Object.KillAllMonsters();
        StartCoroutine(MoveToStageAfterDelay(isClear ? 1 : -1));
    }

    private IEnumerator MoveToStageAfterDelay(int stageDelta)
    {
        yield return new WaitForSeconds(0.5f);
        CharacterData.UpdateStageLevel(stageDelta);
        Debug.LogWarning($"다음 스테이지 {CharacterData.StageLevel} 입니다!");
        SetupStage();
    }

    #endregion


    #region Helpers

    private void ResetStageAndHero()
    {
        Managers.Object.KillAllMonsters();
        Hero hero = Managers.Object.Hero;

        // RankUp 상태면 다른 조건으로 Rebirth
        hero.Rebirth(GameSceneState == EGameSceneState.RankUp);
        hero.ForceMove(new Vector3(-3, 0, 0));
        hero.DisableAction();
    }

    private bool UpdateBossBattleTimer()
    {
        if (GameSceneState == EGameSceneState.Pause || GameSceneState == EGameSceneState.Over)
            return true; // 이미 종료 상태

        BossBattleTimer = Mathf.Clamp(BossBattleTimer - Time.deltaTime, 0f, BossBattleTimeLimit);
        sceneUI.RefreshBossStageTimer(BossBattleTimer, BossBattleTimeLimit);

        if (BossBattleTimer <= 0)
        {
            HandleBattleFailure();
            return true;
        }
        return false;
    }

    /// <summary> 전투 실패 시(히어로 사망, 타이머 종료 등) 실행하는 함수 </summary>
    public void HandleBattleFailure()
    {
        GameSceneState = EGameSceneState.Pause;

        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(
            EFadeType.FadeInOut, 1f, 1f, 1,
            () =>
            {
                Managers.Object.Hero.Rebirth(true);
                GameSceneState = EGameSceneState.Over;
            },
            () =>
            {
                Managers.UI.ShowPopupUI<UI_StageFailPopup>();
                Managers.Event.TriggerEvent(EEventType.HeroRankChallenging, false);
            }
        );
    }

    public string GetCurrentStage()
    {
        return $"{ChapterLevel}-{StageInfo.StageNumber}";
    }

    #endregion
}
