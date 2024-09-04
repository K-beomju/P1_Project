using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        Managers.UI.ShowBaseUI<UI_Joystick>();


        return true;
    }

    public override void Clear()
    {

    }
}
