using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_QuestItem : UI_Base
{
    public enum Texts
    {
        Text_QuestName,
        Text_QuestProgress,
        Text_ItemCount
    }

    public enum Sliders
    {
        Slider_QuestProgress
    }

    public enum Buttons
    {
        Btn_Complete
    }

    public enum Images
    {
        Image_Complete
    }

    private Data.QuestData _questData;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));

        GetButton((int)Buttons.Btn_Complete).onClick.AddListener(OnClickButton);
        GetButton((int)Buttons.Btn_Complete).interactable = false;
        GetImage((int)Images.Image_Complete).gameObject.SetActive(false);

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.QuestItemUpdateed, new Action(RefreshUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.QuestItemUpdateed, new Action(RefreshUI));
    }

    public void SetInfo(Data.QuestData questData)
    {
        _questData = questData;
    }

    // CompleteQuest
    private void OnClickButton()
    {
        Managers.Backend.GameData.QuestData.CompleteQuest(_questData);

        RefreshUI();

        // 일퀘면 바로 잠궈줌
        GetImage((int)Images.Image_Complete).gameObject.SetActive(_questData.QuestCategory == EQuestCategory.Daily);
    }

    public void RefreshUI()
    {
        GetTMPText((int)Texts.Text_QuestName).text = _questData.QuestName;
        var questDic = Managers.Backend.GameData.QuestData.GetQuestCategory(_questData.QuestCategory);
        var questInfo = questDic[_questData.QuestType.ToString()];

        int targetCount = questInfo.TargetCount;
        int currentCount = questInfo.CurrentCount;

        // 반복 퀘스트일 경우 보상 수량을 곱하여 표시
        if (_questData.QuestCategory == EQuestCategory.Repeatable)
        {

            int multiplier = currentCount >= targetCount ? currentCount / targetCount : 1; // targetCount 이상일 때만 나눗셈 수행
            int totalReward = _questData.RewardItem[EItemType.Dia] * multiplier;
            GetTMPText((int)Texts.Text_ItemCount).text = totalReward.ToString();
        }
        else
        {
            GetTMPText((int)Texts.Text_ItemCount).text = _questData.RewardItem[EItemType.Dia].ToString();
        }

        // 진행도
        GetSlider((int)Sliders.Slider_QuestProgress).maxValue = targetCount;
        GetSlider((int)Sliders.Slider_QuestProgress).value = currentCount;
        GetTMPText((int)Texts.Text_QuestProgress).text = $"{currentCount} / {targetCount}";

        GetButton((int)Buttons.Btn_Complete).interactable = questInfo.QuestState != EQuestState.InProgress;
    }
}
