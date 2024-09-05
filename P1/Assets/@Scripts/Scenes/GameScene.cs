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
        
        Hero hero = Managers.Object.Spawn<Hero>(Vector2.zero);
        CameraController cc = Managers.Resource.Instantiate("MainCam").GetComponent<CameraController>();
        cc.Target = hero;
        
        Managers.UI.ShowBaseUI<UI_Joystick>();
    
        GameSceneState = EGameSceneState.Play;
        return true;
    }

    private IEnumerator CoPlayStage()
    {
        Managers.Game.SpawnMonster(10);
        while (true)
        {
            if (Managers.Object.Monsters.Count == 0)
            {
                Debug.Log("ASd");
                yield return null;
            }
               yield return null;
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
