using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DungeonScene : BaseScene
{

    private UI_DungeonScene sceneUI;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.DungeonScene;
        Managers.Scene.SetCurrentScene(this);

         // Server
        InitailizeBackend();
        // Data
        InitializeGameComponents();
        InitializeScene();
        InitializeUI();

        return true;
    }

    private void InitailizeBackend()
    {

    }

    private void InitializeGameComponents()
    {

    }

    private void InitializeScene()
    {

    }

    private void InitializeUI()
    {     
        sceneUI = Managers.UI.ShowSceneUI<UI_DungeonScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);

    }




    public override void Clear()
    {

    }
}
