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
    private bool isStaying = false;

    public StageInfoData StageInfo { get; private set; }
    private BackendData.GameData.CharacterData CharacterData;

    private CameraController cameraController;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Scene
        SceneType = EScene.GameScene;
        Managers.Scene.SetCurrentScene(this);

        CharacterData = Managers.Backend.GameData.CharacterData;
        // Data
        InitializeGameComponents();
        InitializeScene();
        InitializeUI();

        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeIn, 1f, 1f,
        fadeInCallBack: () =>
        {
            if (PlayerPrefs.GetInt("ShowDialogue", 0) == 0)
            {
                PlayerPrefs.SetInt("ShowDialogue", 1);
                var popupUI = Managers.UI.ShowPopupUI<UI_DialoguePopup>();
                Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
                popupUI.RefreshUI();
            }
            else
                SetupStage();
        });
        return true;
    }

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
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cameraController.Target = hero.transform;


        Managers.Pet.Init();
        Managers.Ad.LoadBannerAd();
    }

    private void InitializeUI()
    {
        Managers.UI.CacheAllPopups();
        sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);

        // 다이얼을 다 봤을 때 활성화 
        if (PlayerPrefs.GetInt("ShowDialogue", 0) == 1)
        {
            // 출석체크 로직
            if (CharacterData.AttendanceCheck() == true)
            {
                Debug.Log("하루가 지나 출석체크 팝업 On");
                var popupUI = Managers.UI.ShowPopupUI<UI_AttendancePopup>();
                Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SETTING_CONTENT_POPUP);
                popupUI.RefreshUI();
            }

        }


        // 데이터 불러온 뒤 UI 표시 부분
        Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);
        Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, CharacterData.Level, CharacterData.Exp, CharacterData.MaxExp); // 경험치 갱신 이벤트
        Managers.Event.TriggerEvent(EEventType.QuestCheckNotification);
        Managers.Event.TriggerEvent(EEventType.MissionItemUpdated);
        Managers.Event.TriggerEvent(EEventType.MyRankingUpdated);

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
            { EGameSceneState.Stay, CoStayStage },
            { EGameSceneState.Over, CoStageOver },
            { EGameSceneState.Clear, CoStageClear }
        };
    }

    #region Normal Stage

    public void SetupStage()
    {
        ChapterLevel = 1;
        StageInfo = Managers.Data.StageChart[CharacterData.StageLevel];
        Debug.Log($"{StageInfo.StageNumber} 스테이지 진입");

        BossBattleTimeLimit = StageInfo.BossBattleTimeLimit;
        BossBattleTimer = BossBattleTimeLimit;

        GameSceneState = EGameSceneState.Play;

        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayStage(CharacterData.StageLevel);
    }

    // 스테이지 이동할 때 그 스테이지만 머물게 
    public void StayStage(int stageLevel)
    {
        isStaying = true;
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);
        Managers.Object.KillAllMonsters();

        StageInfo = Managers.Data.StageChart[stageLevel];
        GameSceneState = EGameSceneState.Stay;
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayStage(stageLevel);
    }

    private IEnumerator CoPlayStage()
    {
        Managers.Game.SetMonsterCount(0, StageInfo.KillMonsterCount);

        yield return StartWait;
        Managers.Game.SpawnStageMonster(StageInfo);
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
        ResetStageAndHero();
        var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
        Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_POPUP - 1);
        fadeUI.ShowFadeInOut(EFadeType.FadeIn, 1f, 1f, 1f);
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplay("보스를 처치하세요!");


        Managers.Game.SpawnStageMonster(StageInfo, true);
        var bossMonster = Managers.Object.BossMonster;
        bossMonster.DisableAction();

        sceneUI.RefreshBossMonsterHp(bossMonster);
        Managers.Object.Hero.LookAt(bossMonster.transform.position);
        cameraController.Target = bossMonster.transform;

        Managers.UI.ShowBaseUI<UI_BossStageDisplayBase>().ShowDisplay();
        yield return new WaitForSeconds(2);

        // 연출 제작 
        cameraController.Target = Managers.Object.Hero.transform;

        // Battle Start
        bossMonster.EnableAction();
        Managers.Object.Hero.EnableAction();
        yield return MonitorBossMonsterBattle();
    }

    private IEnumerator MonitorBossMonsterBattle()
    {
        // 몬스터 살아있는지 검사
        while (Managers.Object.BossMonster.IsValid())
        {

            if (UpdateBossBattleTimer()) // 타이머 상태를 확인
            {
                // 못움직이게 함.
                Managers.Object.BossMonster.DisableAction();
                Managers.Object.Hero.DisableAction();
                yield break; // 타이머가 종료되면 코루틴 중단
            }
            yield return null; // 다음 프레임 대기
        }

        // 보스전 성공___________________
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);

        Camera mainCamera = Camera.main;
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane));
        Managers.Object.SpawnGameObject(screenCenter, "Object/ConfettiBurst").GetComponent<ParticleSystem>().Play();
        Managers.UI.ShowBaseUI<UI_StageClearBase>().ShowStageClearUI();

        yield return new WaitForSeconds(1);
        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeInOut, 1f, 1f, 1f,
        fadeOutCallBack: () =>
        {
            GameSceneState = EGameSceneState.Clear;
            Managers.Object.Hero.Rebirth(true);
        });
    }

    #endregion

    #region RankUp

    private IEnumerator CoRankUpStage()
    {
        ResetStageAndHero();

        // Rank Up 데이터 설정 및 몬스터 스폰
        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Pending);
        var rankUpInfo = Managers.Data.RankUpChart[rankType];

        // 보스 몬스터 생성 및 UI 초기화
        SetupRankUpStage(rankUpInfo);
        yield return new WaitForSeconds(2);

        // 연출 제작 
        cameraController.Target = Managers.Object.RankMonster.transform;
        Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayRankUp(Managers.Data.RankUpMonsterChart[rankUpInfo.MonsterDataId].Name);


        yield return new WaitForSeconds(3);
        cameraController.Target = Managers.Object.Hero.transform;

        // Battle Start
        Managers.Object.Hero.EnableAction();
        yield return MonitorRankUpMonsterBattle();
    }

    private IEnumerator MonitorRankUpMonsterBattle()
    {
        // 몬스터 살아있는지 검사
        while (Managers.Object.RankMonster.CreatureState != ECreatureState.Dead)
        {
            if (UpdateBossBattleTimer()) // 타이머 상태를 확인
            {
                // 못움직이게 함.
                Managers.Object.RankMonster.DisableAction();
                Managers.Object.Hero.DisableAction();
                yield break; // 타이머가 종료되면 코루틴 중단
            }

            yield return null; // 다음 프레임 대기
        }

        // 랭크전 성공___________________
        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Pending);
        Managers.Backend.GameData.RankUpData.UpdateRankUp(rankType);
        Managers.Backend.GameData.QuestData.UpdateQuest(EQuestType.HeroRankUp);

        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeInOut, 1f, 1f, 1, () =>
        {
            Managers.Object.DeleteRankMonster();
            GameSceneState = EGameSceneState.Over;
            Managers.Object.Hero.Rebirth(true);
        }, () =>
        {
            // 성공하면 풀링은 더이상 사용 안함 X 
            (Managers.UI.SceneUI as UI_GameScene).UpdateMyRank();
            Managers.UI.ShowPopupUI<UI_RankUpClearPopup>().RefreshUI();
            Managers.Event.TriggerEvent(EEventType.HeroRankChallenging, false);
        });
    }

    private void ResetStageAndHero()
    {
        Managers.Object.KillAllMonsters();
        Hero hero = Managers.Object.Hero;
        hero.Rebirth(GameSceneState == EGameSceneState.RankUp);
        hero.ForceMove(new Vector3(-3, 0, 0));
        hero.DisableAction();
    }

    private void SetupRankUpStage(RankUpInfoData rankUpInfo)
    {
        BossBattleTimeLimit = rankUpInfo.BossBattleTimeLimit;
        BossBattleTimer = BossBattleTimeLimit;
        sceneUI.RefreshBossStageTimer(BossBattleTimer, BossBattleTimeLimit);

        RankMonster monster = Managers.Object.SpawnRankMonster(new Vector3(3, 0, 0), rankUpInfo.MonsterDataId);
        Managers.Object.Hero.LookAt(monster.transform.position);
        sceneUI.RefreshBossMonsterHp(Managers.Object.RankMonster);
    }

    #endregion

    #region Stay 

    private IEnumerator CoStayStage()
    {
        Managers.Game.SetMonsterCount(1, 1); // 몬스터 카운트 초기화
        while (true) // 스테이지를 무한 반복
        {

            yield return StartWait; // 스테이지 시작 대기
            Managers.Game.SpawnStageMonster(StageInfo); // 몬스터 스폰

            int initialMonsterCount = Managers.Object.Monsters.Count;
            while (true)
            {
                // 남은 몬스터가 초기 수의 절반 이하일 경우 새로운 몬스터 소환
                if (Managers.Object.Monsters.Count <= initialMonsterCount / 2)
                {
                    Managers.Game.SpawnStageMonster(StageInfo); // 새로운 몬스터 소환
                    initialMonsterCount = Managers.Object.Monsters.Count; // 초기 몬스터 수 업데이트
                }

                // 몬스터가 다시 절반 이하가 될 때까지 대기
                yield return FrameWait;
            }
        }

    }


    #endregion


    #region Over & Clear

    private IEnumerator CoStageOver()
    {
        sceneUI.RefreshBossStageTimer(0, BossBattleTimeLimit);
        MoveToNextStage(false);
        yield return null;
    }

    private IEnumerator CoStageClear()
    {
        MoveToNextStage(true);
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

    private bool UpdateBossBattleTimer()
    {
        if (GameSceneState == EGameSceneState.Pause || GameSceneState == EGameSceneState.Over)
            return true; // 이미 종료 상태

        BossBattleTimer = Mathf.Clamp(BossBattleTimer - Time.deltaTime, 0.0f, BossBattleTimeLimit);
        sceneUI.RefreshBossStageTimer(BossBattleTimer, BossBattleTimeLimit);

        if (BossBattleTimer <= 0)
        {
            HandleBattleFailure();
            return true; // 타이머 종료
        }

        return false; // 타이머 진행 중
    }

    // 전투 실패 시 실행하는 함수 : 히어로가 죽었을 때, 타이머가 끝났을 때 
    public void HandleBattleFailure()
    {
        GameSceneState = EGameSceneState.Pause;

        // 먼저 딜레이
        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeInOut, 1f, 1f, 1, () =>
        {
            Managers.Object.Hero.Rebirth(true);
            GameSceneState = EGameSceneState.Over;
        }, () =>
        {
            Managers.UI.ShowPopupUI<UI_StageFailPopup>();
            Managers.Event.TriggerEvent(EEventType.HeroRankChallenging, false);
        });
    }

    public string GetCurrentStage()
    {
        return $"{ChapterLevel}-{StageInfo.StageNumber}";
    }

    public override void Clear()
    {
        Managers.Object.KillAllMonsters();
    }
}
