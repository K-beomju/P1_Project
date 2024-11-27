using BackEnd;
using System;
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

        try
        {
            Debug.Log("구글 토큰 : " + token);
            var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);
            Debug.Log("페데레이션 로그인 결과 : " + bro);

            if (IsBackendError(bro))
            {
                if (bro.GetStatusCode() == "401")
                {
                    if (bro.GetMessage().Contains("maintenance"))
                    {
                        ShowAlertUI("서버 점검중입니다.");
                    }
                }
                else if (bro.GetStatusCode() == "403")
                {
                    if (bro.GetMessage().Contains("forbidden block user"))
                    {
                        ShowAlertUI($"해당 계정이 차단당했습니다\n차단사유 : {bro.GetErrorCode()}");
                    }
                }
                else
                {
                    ShowAlertUI($"로그인에 실패하였습니다\n{bro.ToString()}");
                    return;
                }
            }
            else
            {
                if (bro.IsSuccess() == false)
                {
                    ShowAlertUI($"로그인에 실패하였습니다\n{bro.ToString()}");
                    return;
                }

                // 닉네임이 없을 경우, 닉네임 생성 UI 생성 
                if (string.IsNullOrEmpty(Backend.UserNickName))
                {
                    Managers.UI.ShowPopupUI<UI_NicknamePopup>();
                }
                else
                {
                    Debug.Log("로그인에 성공했습니다 : " + bro);
                    Debug.Log($"유저 닉네임 : " + Backend.UserNickName);
                    Debug.Log($"유저 인데이트 : " + Backend.UserInDate);
                    Debug.Log($"유저 UID(쿠폰용) : " + Backend.UID);
                    Managers.Scene.LoadScene(EScene.LoadingScene);
                }
            }

        }
        catch (Exception e)
        {
            ShowAlertUI(e.ToString());
        }
    }



}
