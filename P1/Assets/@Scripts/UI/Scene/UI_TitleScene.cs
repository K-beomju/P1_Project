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
        Btn_LoginWithGoogle,
        Btn_CustomLogin
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Btn_LoginWithGoogle).onClick.AddListener(() =>
        {
            if (Backend.IsInitialized == false)
                return;
            StartGoogleLogin();
        });

        GetButton((int)Buttons.Btn_CustomLogin).onClick.AddListener(() =>
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


    public void StartGoogleLogin()
    {
        TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin(true, GoogleLoginCallback);
    }

    private void GoogleLoginCallback(bool isSuccess, string errorMessage, string token)
    {
        if (isSuccess == false)
        {
            Debug.LogError(errorMessage);
            return;
        }

        Debug.Log("구글 토큰 : " + token);
        var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);
        Debug.Log("페데레이션 로그인 결과 : " + bro);
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
    }



}
