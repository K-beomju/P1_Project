using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TitleScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Scene
        SceneType = EScene.TitleScene;
        Managers.Scene.SetCurrentScene(this);

        Managers.Backend.Init();
        return true;
    }

    public override void Clear()
    {

    }

}
