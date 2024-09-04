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

        for (int i = 0; i < 10; i++)
        {
            int randX = Random.Range(-5, 5);
            int randY = Random.Range(-5, 5);
            Managers.Object.Spawn<Monster>(new Vector3(randX, randY, 0));
        }

        return true;
    }

    public override void Clear()
    {

    }
}
