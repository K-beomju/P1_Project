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
        Text_Loading,
        Text_TouchStart
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
        ToggleUIGroup(GameObjects.Group_Buttons, false);
        ToggleUIGroup(GameObjects.Group_Loading, false);

        // 버튼 이벤트 바인딩
        GetButton((int)Buttons.Btn_LoginWithGoogle).onClick.AddListener(StartGoogleLogin);

#if UNITY_EDITOR
        // 에디터에서 버튼 활성화 및 동작 추가
        GetButton((int)Buttons.Btn_CustomLogin).onClick.AddListener(StartCustomLogin);
#elif UNITY_ANDROID
    // 안드로이드에서 버튼 비활성화
        GetButton((int)Buttons.Btn_CustomLogin).gameObject.SetActive(false);
#endif

        GetTMPText((int)Texts.Text_TouchStart).gameObject.SetActive(false);

        //Managers.Sound.Play(ESound.Bgm,"Sounds/TitleBGM", 0.3f);
        return true;
    }


    private void Start()
    {
        // 자동로그인 토큰 검사 
        UpdateCheck((update) =>
        {
            if (update)
            {
                // 유저가 스토어에서 업데이트를 하도록 업데이트 UI를 띄워줍니다.
                Managers.UI.ShowPopupUI<UI_UpdatePopup>();
            }
            else
            {
                LoginWithBackendToken();
            }
        });
    }

    #region Login
    private void LoginWithBackendToken()
    {
        SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>
        {
            Debug.Log($"Backend.BMember.LoginWithTheBackendToken : {callback}");

            if (callback.IsSuccess())
            {
                HandlePostLogin();
                ToggleUIGroup(GameObjects.Group_Loading, true);
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
        if (string.IsNullOrEmpty(Backend.UserNickName))
        {
            Managers.UI.ShowPopupUI<UI_PolicyPopup>();
        }
        else
        {
            Debug.Log("로그인 성공");
            Debug.Log($"유저 닉네임 : {Backend.UserNickName}");
            Debug.Log($"유저 인데이트 : {Backend.UserInDate}");
            Debug.Log($"유저 UID(쿠폰용) : {Backend.UID}");
            ShowAlertUI($"로그인에 성공하였습니다.");
        }
        Managers.Scene.GetCurrentScene<TitleScene>().InitBackendDataLoad();
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

    public void ShowTouchToStart()
    {
        if (string.IsNullOrEmpty(Backend.UserNickName))
            return;
        ToggleUIGroup(GameObjects.Group_Loading, false);
        GetTMPText((int)Texts.Text_TouchStart).gameObject.SetActive(true);
        GetTMPText((int)Texts.Text_TouchStart).gameObject.BindEvent(() =>
        {
            TitleScene titleScene = Managers.Scene.GetCurrentScene<TitleScene>();
            titleScene.InGameStart();
        }, EUIEvent.Click);
    }

    private void UpdateCheck(Action<bool> updateCallBack)
    {
        // 유니티 플레이어 세팅에 설정한 버전 정보
        Version client = new Version(Application.version);
        Debug.Log("clientVersion: " + client);

#if UNITY_EDITOR
        // 뒤끝 버전 정보 조회는 iOS / Android 환경에서만 호출 할 수 있습니다.
        updateCallBack(false);
        Debug.Log("에디터 모드에서는 버전 정보를 조회할 수 없습니다.");
        return;
#endif

        // 뒤끝 콘솔에서 설정한 버전 정보를 조회
        Backend.Utils.GetLatestVersion(callback =>
        {
            if (callback.IsSuccess() == false)
            {
                Debug.LogError("버전 정보를 조회하는데 실패하였습니다.\n" + callback);
                return;
            }

            var version = callback.GetReturnValuetoJSON()["version"].ToString();
            Version server = new Version(version);

            var result = server.CompareTo(client);
            if (result == 0)
            {
                updateCallBack(false);
                // 0 이면 두 버전이 일치하는 것 입니다.
                return;
            }
            else if (result < 0)
            {
                // 0 미만인 경우 server 버전이 client 보다 작은 경우 입니다.
                // 애플/구글 스토어에 검수를 넣었을 경우 여기에 해당 할 수 있습니다.
                // ex)
                // 검수를 신청한 클라이언트 버전은 3.0.0, 
                // 라이브에 운용중인 클라이언트 버전은 2.0.0,
                // 뒤끝 콘솔에 등록한 버전은 2.0.0 

                // 아무 작업을 안하고 리턴
                updateCallBack(false);
                return;
            }
            // 0보다 크면 server 버전이 클라이언트 이후 버전일 수 있습니다.
            else if (client == null)
            {
                // 단 클라이언트 버전 정보가 null인 경우에도 0보다 큰 값이 리턴될 수 있습니다.
                // 이 때는 아무 작업을 안하고 리턴하도록 하겠습니다.
                updateCallBack(false);
                Debug.LogError("클라이언트 버전 정보가 null 입니다.");
                return;
            }

            // 여기까지 리턴 없이 왔으면 server 버전(뒤끝 콘솔에 등록한 버전)이 
            // 클라이언트보다 높은 경우 입니다.
            updateCallBack(true);
        });
    }

}
