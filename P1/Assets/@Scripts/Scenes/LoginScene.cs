using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class LoginScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Scene
        SceneType = EScene.LoginScene;
        Managers.Scene.SetCurrentScene(this);

        Managers.Backend.Init();
        if (Backend.IsInitialized == false) {
            Debug.LogError("뒤끝 초기화가 안됌");
            return false;
        }

        StartCoroutine(TestLoginCo());


        return true;
    }

    private IEnumerator TestLoginCo()
    {
        yield return new WaitForSeconds(2f);
        Backend.BMember.CustomLogin("jejuRooftop", "1234", callback => 
        {
           Managers.Scene.LoadScene(EScene.LoadingScene);
        });
        //         yield return new WaitForSeconds(1f);

        // Managers.Scene.LoadScene(EScene.LoadingScene);
    }

    public override void Clear()
    {

    }

}
