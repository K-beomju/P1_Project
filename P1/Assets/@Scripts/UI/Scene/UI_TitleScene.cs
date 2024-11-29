using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Define;

public class UI_TitleScene : UI_Scene
{
    public enum GameObjects
    {
        Group_Buttons,
        Group_Loading
    }

    public enum Buttons
    {
        Btn_LoginWithGoogle,
        Btn_CustomLogin
    }

    public enum Sliders
    {
        Slider_Loading
    }

    public enum Texts
    {
        Text_Loading
    }

    protected override bool Init()
    {
        if (!base.Init())
            return false;
        
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindSliders(typeof(Sliders));
        BindTMPTexts(typeof(Texts));

        // 초기 UI 상태 설정
        ToggleUIGroup(GameObjects.Group_Buttons, true);
        ToggleUIGroup(GameObjects.Group_Loading, false);

        // 버튼 이벤트 바인딩
        GetButton((int)Buttons.Btn_LoginWithGoogle).onClick.AddListener(StartGoogleLogin);
        GetButton((int)Buttons.Btn_CustomLogin).onClick.AddListener(StartCustomLogin);

        return true;
    }


    private void Start()
    {
        LoginWithBackendToken();
    }

    #region Login
    private void LoginWithBackendToken()
    {
        SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>
        {
            Debug.Log($"Backend.BMember.LoginWithTheBackendToken : {callback}");

            if (callback.IsSuccess())
            {
#if UNITY_EDITOR
                ToggleUIGroup(GameObjects.Group_Buttons, false);
                ToggleUIGroup(GameObjects.Group_Loading, true);
                HandlePostLogin();
#else
                StartGoogleLogin();
#endif
            }
            else
            {
                ToggleUIGroup(GameObjects.Group_Buttons, true);
            }
        });
    }

    public void StartGoogleLogin()
    {
        TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin(true, (isSuccess, errorMessage, token) =>
        {
            if (!isSuccess)
            {
                Debug.LogError(errorMessage);
                return;
            }

            var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);
            Debug.Log("페데레이션 로그인 결과 : " + bro);

            if (HandleBackendError(bro))
                return;

            ToggleUIGroup(GameObjects.Group_Buttons, false);
            ToggleUIGroup(GameObjects.Group_Loading, true);
            HandlePostLogin();
        });
    }

    private void StartCustomLogin()
    {
        if (!Backend.IsInitialized)
            return;

        var bro = Backend.BMember.CustomLogin("asd", "1234");
        if (HandleBackendError(bro))
            return;

        ToggleUIGroup(GameObjects.Group_Buttons, false);
        ToggleUIGroup(GameObjects.Group_Loading, true);
        HandlePostLogin();
    }
    #endregion

    #region Helper Methods
    private void HandlePostLogin()
    {
        TitleScene titleScene = Managers.Scene.GetCurrentScene<TitleScene>();

        if (string.IsNullOrEmpty(Backend.UserNickName))
        {
            ShowAlertUI($"닉네임이 없습니다.");
            Managers.UI.ShowPopupUI<UI_NicknamePopup>();
            titleScene.SceneMove = false;
            titleScene.InitBackendDataLoad();
        }
        else
        {
            Debug.Log("로그인 성공");
            Debug.Log($"유저 닉네임 : {Backend.UserNickName}");
            Debug.Log($"유저 인데이트 : {Backend.UserInDate}");
            Debug.Log($"유저 UID(쿠폰용) : {Backend.UID}");
            ShowAlertUI($"로그인에 성공하였습니다.");
            titleScene.InitBackendDataLoad();
        }
    }

    private bool HandleBackendError(BackendReturnObject bro)
    {
        if (!IsBackendError(bro))
            return false;

        string statusCode = bro.GetStatusCode();
        string message = bro.GetMessage();

        if (statusCode == "401" && message.Contains("maintenance"))
        {
            ShowAlertUI("서버 점검중입니다");
        }
        else if (statusCode == "403" && message.Contains("Forbidden blocked user"))
        {
            ShowAlertUI($"해당 계정이 차단당했습니다\n차단사유 : {bro.GetErrorCode()}");
        }
        else
        {
            ShowAlertUI($"로그인에 실패하였습니다");
        }

        return true;
    }

    private void ToggleUIGroup(GameObjects group, bool isActive)
    {
        GetObject((int)group).SetActive(isActive);
    }
    #endregion

    #region Data Loading
    public void ShowDataName(string info)
    {
        GetTMPText((int)Texts.Text_Loading).text = info;
        Debug.Log(info);
    }

    public void ShowDataSlider(int currentCount, int maxCount)
    {
        GetSlider((int)Sliders.Slider_Loading).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Loading).value = currentCount;
    }
    #endregion


}
