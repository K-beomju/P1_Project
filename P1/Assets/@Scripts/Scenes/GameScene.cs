using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    private IEnumerator _currentCoroutine = null;
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

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _currentCoroutine = coroutine;
        StartCoroutine(_currentCoroutine);
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero);
        cc.Target = hero;

        Managers.UI.CacheAllPopups();
        var sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, 100);

        GameSceneState = EGameSceneState.Play;
        return true;
    }

    private IEnumerator CoPlayStage()
    {
        int initialSpawnCount = 10;  // 처음에 스폰할 몬스터 수
        float respawnDelay = 2.0f;  // 리스폰 사이의 시간 간격

        WaitForSeconds respawnWait = new WaitForSeconds(respawnDelay);
        WaitForSeconds frameWait = new WaitForSeconds(0.2f); // 0.2초 대기 시간

        // 처음에 몬스터 스폰
        Managers.Game.SpawnMonster(initialSpawnCount);

        while (true)
        {
            // 현재 남아있는 몬스터 수가 respawnThreshold 이하일 때 새로운 몬스터를 스폰
            if (Managers.Object.Monsters.Count == 0)
            {
                yield return respawnWait; // 리스폰 전 딜레이
                Managers.Game.SpawnMonster(initialSpawnCount);  // 몬스터 리스폰
            }

            // 반복적으로 체크
            yield return frameWait;
        }
    }

    private IEnumerator CoPauseStage()
    {
        yield return null;
    }

    private IEnumerator CoBossStage()
    {
        yield return null;
    }

    private IEnumerator CoStageOver()
    {
        yield return null;
    }

    private IEnumerator CoStageClear()
    {

        yield return null;
    }

    public override void Clear()
    {

    }
}
