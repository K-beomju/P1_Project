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
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));        
        BindButtons(typeof(Buttons));        
        BindSliders(typeof(Sliders));
        BindTMPTexts(typeof(Texts));

        // 로그인 버튼 그룹은 활성화, 데이터 로딩은 비활성화 
        GetObject((int)GameObjects.Group_Buttons).SetActive(true);
        GetObject((int)GameObjects.Group_Loading).SetActive(false);

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

            var bro = Backend.BMember.CustomLogin("asd", "1234");
            if (bro.IsSuccess())
            {
                Debug.Log("로그인에 성공했습니다 : " + bro);
                Debug.Log($"유저 닉네임 : " + Backend.UserNickName);
                Debug.Log($"유저 인데이트 : " + Backend.UserInDate);
                Debug.Log($"유저 UID(쿠폰용) : " + Backend.UID);

                GetObject((int)GameObjects.Group_Buttons).SetActive(false);
                GetObject((int)GameObjects.Group_Loading).SetActive(true);

                Managers.Scene.GetCurrentScene<TitleScene>().InitBackendDataLoad();
            }
            else
            {
                // Backend.UserNickname, Backend.UserInDate, Backend.UID에 값이 할당되지 않습니다.  
            }
        });
        return true;
    }

    #region Login
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
