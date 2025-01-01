using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using BackendData.GameData;
using static Define;
using System;

public class InGameScene_Mission : UI_Base
{
    public enum Texts
    {
        Text_MissionTitle,
        Text_MissionInfo,
        Text_RewardValue
    }

    public enum Images
    {
        Image_RewardItem,
        Mission_NotifiBadge
    }

    private BackendData.GameData.MissionData MissionData;

    private Button _missionBtn;
    private RectTransform _rewardRect;
    private bool _isCompleted = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));

        _missionBtn = GetComponent<Button>();
        _missionBtn.onClick.AddListener(OnClickMissionButton);
        _rewardRect = GetTMPText((int)Texts.Text_RewardValue).GetComponent<RectTransform>();
        MissionData = Managers.Backend.GameData.MissionData;
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.MissionItemUpdated, new Action(RefreshUI));
    }

    private void OnDestroy()
    {
        Managers.Event.RemoveEvent(EEventType.MissionItemUpdated, new Action(RefreshUI));
    }

    private void OnClickMissionButton()
    {
        if (_isCompleted == false)
            return;

        // 완료 보상 지급
        for (int i = 0; i < 10; i++)
        {
            UI_ItemIconBase itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
            itemIcon.SetItemIconAtCanvasPosition(EItemType.Dia, Util.GetCanvasPosition(_rewardRect.position));
        }

        // Clear 보상 지급 
        Dictionary<EItemType, int> rewardItem = Managers.Data.MissionChart[MissionData.GetCurrentMission().Id].RewardItem;
        Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, rewardItem[EItemType.Dia]);

        _isCompleted = false;
        Managers.Sound.Play(ESound.Effect, "Sounds/SuccessReward", 0.5f);
        Managers.Backend.GameData.MissionData.CompleteMission(MissionData.GetCurrentMission());
        
        if(MissionData.GetCurrentMission() == null)
        {
            ShowAlertUI("모든 미션을 완료했습니다");
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        MissionInfoData missionInfoData = MissionData.GetCurrentMission();
        if (missionInfoData == null)
        {
            Debug.LogWarning("모든 미션이 끝났습니다");
            Managers.Event.RemoveEvent(EEventType.MissionItemUpdated, new Action(RefreshUI));
            gameObject.SetActive(false);
            return;
        }

        int currentValue = MissionData.MissionDic[missionInfoData.MissionType];

        GetTMPText((int)Texts.Text_MissionInfo).text = $"{Managers.Data.MissionChart[missionInfoData.Id].Remark} ({currentValue}/{missionInfoData.CompleteValue})";

        // TODO 수정
        GetTMPText((int)Texts.Text_RewardValue).text = $"{Managers.Data.MissionChart[missionInfoData.Id].RewardItem[EItemType.Dia]}";

        if (currentValue >= missionInfoData.CompleteValue)
        {
            if (_isCompleted)
                return;

            GetTMPText((int)Texts.Text_MissionTitle).text = $"보상 받기";
            _missionBtn.interactable = true;
            GetImage((int)Images.Mission_NotifiBadge).gameObject.SetActive(true);
            _isCompleted = true;
        }
        else
        {
            GetImage((int)Images.Mission_NotifiBadge).gameObject.SetActive(false);
            _missionBtn.interactable = false;
            GetTMPText((int)Texts.Text_MissionTitle).text = $"미션 {missionInfoData.Id}";
        }

    }
}
