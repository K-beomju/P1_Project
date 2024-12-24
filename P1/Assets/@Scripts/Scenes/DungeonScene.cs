using BackendData.GameData;
using Cinemachine;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // ★ 추가: 던전 전용 핸들러 인터페이스
    private IDungeonHandler _dungeonHandler;

    private Dictionary<EGameSceneState, Func<IEnumerator>> _stateCoroutines;
    private IEnumerator _currentCoroutine = null;
    
    private EDungeonType _dungeonType;
    public EDungeonType DungeonType => _dungeonType;

    // UI
    public UI_DungeonScene sceneUI;

    // 공통적으로 쓰이는 정보들
    public DungeonInfoData DungeonInfo { get; set; }
    public WorldBossDungeonInfoData WorldDungeonInfo { get; set; }

    public float DungeonTimer { get; set; }
    public float DungeonTimeLimit { get; set; }

    // 보상 임시 딕셔너리
    public Dictionary<EItemType, int> clearRewardDic = new Dictionary<EItemType, int>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.DungeonScene;
      
        // 던전 타입을 가져온다
        _dungeonType = Managers.Game.GetCurrentDungeon();
        Managers.Scene.SetCurrentScene(this);

        // 던전 핸들러 주입
        if (_dungeonType == EDungeonType.WorldBoss)
            _dungeonHandler = new WorldBossDungeonHandler();
        else
            _dungeonHandler = new NormalDungeonHandler();

        InitializeScene();
        InitializeUI();

        //InitializeDungeon();
        // 핸들러에 던전 초기화 위임
        _dungeonHandler.InitializeDungeon(this);

        // 페이드 인 이후에 스테이지 플레이로 전환
        Managers.UI.ShowBaseUI<UI_FadeInBase>()?.ShowFadeInOut(
            EFadeType.FadeIn, 1f, 1f,
            fadeInCallBack: () =>
            {
                int dungeonLevel = (_dungeonType == EDungeonType.WorldBoss) ? 0 : DungeonInfo.DungeonLevel;
                Managers.UI.ShowBaseUI<UI_StageDisplayBase>()?.ShowDisplayDungeon(_dungeonType, dungeonLevel);
                GameSceneState = EGameSceneState.Play;
            }
        );

        return true;
    }


    private void InitializeScene()
    {
        string mapName = DungeonType switch
        {
            EDungeonType.Gold => "GoldDungeonMap",
            EDungeonType.Dia => "DiaDungeonMap",
            EDungeonType.WorldBoss => "WorldBossMap",
            _ => throw new ArgumentException($"Unknown mapName: {DungeonType}")
        };

        GameObject map = Managers.Resource.Instantiate(mapName);
        PolygonCollider2D polygon = Util.FindChild(map, "Terrain_Tile").GetComponent<PolygonCollider2D>();
        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        cc.GetComponent<CinemachineConfiner>().m_BoundingShape2D = polygon;
        
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero, 0);
        cc.Target = hero.transform;
    }

    private void InitializeUI()
    {
        sceneUI = Managers.UI.ShowSceneUI<UI_DungeonScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);

        // 경험치 갱신 이벤트
        var charData = Managers.Backend.GameData.CharacterData;
        Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, 
                                   charData.Level, 
                                   charData.Exp, 
                                   charData.MaxExp);

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
        // IDungeonHandler를 통해 몬스터(또는 월드보스) 스폰
        yield return StartCoroutine(_dungeonHandler.SpawnMonsters(this));
        
        // 플레이 상태에서는 타이머가 동작
        while (true)
        {
            _dungeonHandler.UpdateTimer(this);

            // 월드보스가 아닌 경우, 클리어 체크 / 영웅 죽음 체크
            if (_dungeonType != EDungeonType.WorldBoss)
            {
                if (Managers.Game.ClearStage()) 
                {
                    GameSceneState = EGameSceneState.Clear;
                }
                else if (Managers.Object.Hero.CreatureState == ECreatureState.Dead)
                {
                    GameSceneState = EGameSceneState.Over;
                }
            }
            else 
            {
                // 월드보스 던전은 영웅 죽음 체크만
                if (Managers.Object.Hero.CreatureState == ECreatureState.Dead)
                {
                    GameSceneState = EGameSceneState.Over;
                }
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
        yield return StartCoroutine(_dungeonHandler.OnStageOver(this));
    }

    private IEnumerator CoStageClear()
    {
        // 핸들러가 OnStageClear 처리
        yield return StartCoroutine(_dungeonHandler.OnStageClear(this));
    }

    #endregion

    private void KillAllMonsters()
    {
        for (int i = Managers.Object.Monsters.Count - 1; i >= 0; i--)
        {
            Monster monster = Managers.Object.Monsters.ElementAt(i);
            monster.DisableAction();
            Managers.Object.Despawn(monster);
        }
    }


    public override void Clear()
    {
        KillAllMonsters();
    }
}
