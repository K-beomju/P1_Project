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

    private Dictionary<EGameSceneState, Func<IEnumerator>> _stateCoroutines;
    private IEnumerator _currentCoroutine = null;
    private EDungeonType DungeonType;
    private UI_DungeonScene sceneUI;

    public DungeonInfoData DungeonInfo { get; private set; }
    public WorldBossDungeonInfoData WorldDungeonInfo { get; private set; }
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


        InitializeScene();
        InitializeDungeon();

        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeInOut(EFadeType.FadeIn, 1f, 1f,
        fadeInCallBack: () =>
        {
            int dungeonLevel = DungeonType == EDungeonType.WorldBoss ? 0 : DungeonInfo.DungeonLevel;
            Managers.UI.ShowBaseUI<UI_StageDisplayBase>().ShowDisplayDungeon(DungeonType, dungeonLevel);
            GameSceneState = EGameSceneState.Play;
        });

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

        // UI 초기화
        sceneUI = Managers.UI.ShowSceneUI<UI_DungeonScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);
    }

    private void InitializeDungeon()
    {
        int dungeonLevel = Managers.Backend.GameData.DungeonData.DungeonLevelDic[DungeonType.ToString()];

        if (DungeonType != EDungeonType.WorldBoss)
        {
            DungeonInfo = DungeonType switch
            {
                EDungeonType.Gold => Managers.Data.GoldDungeonChart[dungeonLevel],
                EDungeonType.Dia => Managers.Data.DiaDungeonChart[dungeonLevel],
                _ => throw new ArgumentException($"Unknown DungeonType: {DungeonType}")
            };

            sceneUI.UpdateStageUI(DungeonType, DungeonInfo.DungeonLevel);
            Managers.Game.SetMonsterCount(0, DungeonInfo.KillMonsterCount);

            DungeonTimeLimit = DungeonInfo.DungeonTimeLimit;
            DungeonTimer = DungeonTimeLimit;
            UpdateDungeonTimer();
        }
        else
        {
            WorldDungeonInfo = Managers.Data.WorldBossDungeonChart[1];
            sceneUI.UpdateStageUI(DungeonType, 0);
            Managers.Game.SetMonsterCount(0, 0);

            DungeonTimeLimit = WorldDungeonInfo.DungeonTimeLimit;
            DungeonTimer = WorldDungeonInfo.DungeonTimeLimit;
            UpdateDungeonTimer();
        }
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
        yield return new WaitForSeconds(2f);
        if (DungeonType == EDungeonType.WorldBoss)
        {
            Managers.Game.SpawnDungeonMonster(isBoss: true);
        }
        else
        {
            Managers.Game.SpawnDungeonMonster(dungeonInfo: DungeonInfo);
        }


        // 몬스터가 스폰될 때 자동 스킬 조건을 다시 검사하도록 이벤트 트리거
        if (Managers.Backend.GameData.SkillInventory.IsAutoSkill)
            (Managers.UI.SceneUI as UI_DungeonScene).CheckUseSkillSlot(-1);

        while (DungeonTimer > 0)
        {
            UpdateDungeonTimer(); // 보스 타이머 업데이트 메서드 호출

            if (DungeonType != EDungeonType.WorldBoss)
            {
                if (Managers.Game.ClearStage())
                {
                    GameSceneState = EGameSceneState.Clear;
                }
                if (Managers.Object.Hero.CreatureState == ECreatureState.Dead)
                {
                    GameSceneState = EGameSceneState.Over;
                }
            }
            else
            {

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
        yield return new WaitForSeconds(1f);

        sceneUI.RefreshDungeonTimer(0, 0);
        if (DungeonType != EDungeonType.WorldBoss)
        {
            // 몬스터 멈추고 UI 팝업 켜주고 
            Managers.Object.Monsters.ToList().ForEach(x => x.DisableAction());
            var popupUI = Managers.UI.ShowPopupUI<UI_DungeonFailPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);

            // 던전 키 다시 줌
            Managers.Backend.GameData.DungeonData.AddKey(DungeonType, 1);
        }
        else
        {
            // 월드보스에게 넣은 데미지 팝업 표시 끝 .
            // 랭킹 저장 
            var userData = Managers.Backend.GameData.CharacterData;
            int endTotalWorldBossDmg = Managers.Object.WorldBoss.worldBossTotalDamage;
            int currentTotalWorldBossDmg = userData.WorldBossCombatPower;

            if (currentTotalWorldBossDmg < endTotalWorldBossDmg || currentTotalWorldBossDmg == 0)
            {
                Managers.Backend.GameData.CharacterData.UpdateWorldBossCombatPower(endTotalWorldBossDmg);
            }



            var popupUI = Managers.UI.ShowPopupUI<UI_BattleResultPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
            popupUI.RefreshUI(Managers.Object.WorldBoss.worldBossTotalDamage);
        }

        yield return null;
    }

    private IEnumerator CoStageClear()
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DungeonClearPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);

        clearRewardDic.Clear();
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
