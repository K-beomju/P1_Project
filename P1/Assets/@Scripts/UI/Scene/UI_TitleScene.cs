using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Define;

public class UI_TitleScene : UI_Scene
{
    public enum Buttons
    {
        Btn_Login
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Btn_Login).onClick.AddListener(() =>
        {
            if (Backend.IsInitialized == false)
                return;

            var bro = Backend.BMember.CustomLogin("user1", "1234");
            if (bro.IsSuccess())
            {

                Debug.Log("로그인에 성공했습니다 : " + bro);
                Debug.Log($"유저 닉네임 : " + Backend.UserNickName);
                Debug.Log($"유저 인데이트 : " + Backend.UserInDate);
                Debug.Log($"유저 UID(쿠폰용) : " + Backend.UID);
                Managers.Scene.LoadScene(EScene.LoadingScene);
            }
            else
            {
                // Backend.UserNickname, Backend.UserInDate, Backend.UID에 값이 할당되지 않습니다.  
            }
        });

        return true;
    }

}
