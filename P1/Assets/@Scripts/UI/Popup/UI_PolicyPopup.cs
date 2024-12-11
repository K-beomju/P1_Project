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
        MainGroup,
        ScrollView_Service,
        ScrollView_Personal
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
        GetButton((int)Buttons.Button_Service).onClick.AddListener(() => ShowPolicyText(PolicyType.Service));
        GetButton((int)Buttons.Button_Personal).onClick.AddListener(() => ShowPolicyText(PolicyType.Personal));
        GetButton((int)Buttons.Btn_Accept).onClick.AddListener(OnClickAccpetButton);

        // Detactive
        GetButton((int)Buttons.Btn_Exit).gameObject.SetActive(false);
        GetObject((int)GameObjects.ScrollView_Service).SetActive(false);
        GetObject((int)GameObjects.ScrollView_Personal).SetActive(false);
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
        GetObject((int)GameObjects.ScrollView_Service).SetActive(false);
        GetObject((int)GameObjects.ScrollView_Personal).SetActive(false);
        GetButton((int)Buttons.Btn_Exit).gameObject.SetActive(false);
    }

    private void ShowPolicyText(PolicyType policyType)
    {
        GetObject((int)GameObjects.MainGroup).SetActive(false);

        GetButton((int)Buttons.Btn_Exit).gameObject.SetActive(true);
        GetObject((int)GameObjects.ScrollView_Service).SetActive(policyType == PolicyType.Service);
        GetObject((int)GameObjects.ScrollView_Personal).SetActive(policyType == PolicyType.Personal);

        // 이용약관
        if(policyType == PolicyType.Service)
        {
            GetTMPText((int)Texts.Text_ServiceDesc).text = serviceDesc;
        }

        // // 개정방 
        if(policyType == PolicyType.Personal)
        {
            GetTMPText((int)Texts.Text_PersonalDesc).text = personalDesc;
        }
    }

    public void GetPolicyV2()
    {
        var bro = Backend.Policy.GetPolicyV2();

        if (!bro.IsSuccess())
        {
            return;
        }

        Policy policy = new Policy();

        LitJson.JsonData policyJson = bro.GetReturnValuetoJSON()["policy"]; // policy로 접근

        policy.terms = policyJson["terms"]?.ToString(); // 입력이 없을 경우 null
        policy.privacy = policyJson["privacy"]?.ToString(); // 입력이 없을 경우 null

        serviceDesc = policy.terms;
        personalDesc = policy.privacy;

        // // 추가 정책을 한번이라도 수정한 경우
        // if (bro.GetReturnValuetoJSON().ContainsKey("policy2"))
        // {

        //     LitJson.JsonData policy2Json = bro.GetReturnValuetoJSON()["policy2"]; // policy2로 접근

        //     Policy policy2 = new Policy();

        //     policy2.terms = policy2Json["terms"]?.ToString(); // 입력이 없을 경우 null
        //     policy2.privacy = policy2Json["privacy"]?.ToString(); // 입력이 없을 경우 null

        //     serviceDesc = policy2.terms;
        //     personalDesc = policy2.privacy;
        // }

    }
}
