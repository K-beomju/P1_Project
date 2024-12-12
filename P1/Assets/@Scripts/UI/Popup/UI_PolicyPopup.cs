using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Messaging;



public class UI_PolicyPopup : UI_Popup
{
    public class Policy
    {
        public string terms;
        public string termsURL;
        public string privacy;
        public string privacyURL;
        public override string ToString()
        {
            string str = $"terms : {terms}\n" +
            $"termsURL : {termsURL}\n" +
            $"privacy : {privacy}\n" +
            $"privacyURL : {privacyURL}\n";
            return str;
        }
    }
    
    public enum GameObjects
    {
        MainGroup
    }

    public enum Toggles
    {
        Toggle_AllCheck,
        Toggle_ServiceCheck,
        Toggle_PersonalCheck,
        Toggle_PushCheck
    }

    public enum Texts
    {
        Text_ServiceDesc,
        Text_PersonalDesc
    }

    public enum Buttons
    {
        Btn_Exit,
        Btn_Accept,
        Button_Service,
        Button_Personal
    }

    public enum PolicyType 
    {
        Service,
        Personal
    }

    private string serviceDesc = null;
    private string personalDesc = null;
    private string token = string.Empty;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObjects(typeof(GameObjects));
        BindToggles(typeof(Toggles));
        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(OnClickExitButton);
        GetButton((int)Buttons.Button_Service).onClick.AddListener(() =>
        {
            // 이용약관
            Application.OpenURL("https://storage.thebackend.io/98b106d62906bad2c8416462d83b8b90f151ab550fd19a5fa04d0607a5bb99d5/terms.html");
        });
        GetButton((int)Buttons.Button_Personal).onClick.AddListener(() => 
        {
            // 개인정보처리방침
            Application.OpenURL("https://storage.thebackend.io/98b106d62906bad2c8416462d83b8b90f151ab550fd19a5fa04d0607a5bb99d5/privacy.html");
        });
        GetButton((int)Buttons.Btn_Accept).onClick.AddListener(OnClickAccpetButton);

        // Detactive
        GetButton((int)Buttons.Btn_Exit).gameObject.SetActive(false);
        GetButton((int)Buttons.Btn_Accept).interactable = false;

        // Toggle
        GetToggle((int)Toggles.Toggle_AllCheck).onValueChanged.AddListener((isChecked) => 
        {
            GetToggle((int)Toggles.Toggle_PersonalCheck).isOn = isChecked;
            GetToggle((int)Toggles.Toggle_ServiceCheck).isOn = isChecked;
            GetToggle((int)Toggles.Toggle_PushCheck).isOn = isChecked;
            GetButton((int)Buttons.Btn_Accept).interactable = isChecked;
        });

        GetToggle((int)Toggles.Toggle_ServiceCheck).onValueChanged.AddListener((isChecked) =>
        {
            GetButton((int)Buttons.Btn_Accept).interactable = EssentialPolicy();
        });
        GetToggle((int)Toggles.Toggle_PersonalCheck).onValueChanged.AddListener((isChecked) =>
        {
            GetButton((int)Buttons.Btn_Accept).interactable = EssentialPolicy();
        });
        
        
        return true;
    }
    void Start() {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase 초기화
                FirebaseApp app = FirebaseApp.DefaultInstance;
                // FCM 토큰 가져오기
                GetToken();
            }
            else
            {
                Debug.Log($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    void GetToken()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;

        // 해당 task는 외부쓰레드로 작동합니다.
        // 만약 GameObject.Instantiate 같은 유니티 함수나 UnityEngine.UI를 사용할 경우, 예외가 발생합니다.
        FirebaseMessaging.GetTokenAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                token = task.Result;
                Debug.Log($"FCM Token: {token}");
            }
            else
            {
                Debug.Log("Failed to get FCM token");
            }
        });
    }

    void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log($"Received Registration Token: {token.Token}");
    }


    private bool EssentialPolicy()
    {
        if(GetToggle((int)Toggles.Toggle_PersonalCheck).isOn && GetToggle((int)Toggles.Toggle_ServiceCheck).isOn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnClickAccpetButton()
    {
        // 푸시알림에 동의했을 경우
        if(GetToggle((int)Toggles.Toggle_PushCheck).isOn)
        {
            SendQueue.Enqueue(Backend.Android.PutDeviceToken, token, (callback) => 
            {
                if (IsBackendError(callback)) {
                    ShowAlertUI("푸시 알람 미처리 안내");
                }
                else {
                    ClosePopupUI();
                    Managers.UI.ShowPopupUI<UI_NicknamePopup>();
                }
            });
        }
        else
        {
            ClosePopupUI();
            Managers.UI.ShowPopupUI<UI_NicknamePopup>();
        }
    }
    

    private void OnClickExitButton()
    {
        GetObject((int)GameObjects.MainGroup).SetActive(true);
        GetButton((int)Buttons.Btn_Exit).gameObject.SetActive(false);
    }

}
