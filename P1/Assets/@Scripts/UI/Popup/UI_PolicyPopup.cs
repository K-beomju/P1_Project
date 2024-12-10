using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
        ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_NicknamePopup>();
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
