using System;
using UnityEngine.UI;
using static Define;

public class InGameScene_Quest : UI_Base
{
    public enum Images
    {
        Quest_NotifiBadge
    }

    private Button _questBtn;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        _questBtn = GetComponent<Button>();
        _questBtn.onClick.AddListener(OnClickButton);
        GetImage((int)Images.Quest_NotifiBadge).gameObject.SetActive(false);
        return true;
    }

    private void OnClickButton()
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_QuestPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
        popupUI.RefreshUI();
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.QuestCheckNotification, new Action(() =>
        {
            GetImage((int)Images.Quest_NotifiBadge).gameObject.SetActive(
            Managers.Backend.GameData.QuestData.IsReadyToClaimQuest());
        }));
    }


    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.QuestCheckNotification, new Action(() =>
        {
            GetImage((int)Images.Quest_NotifiBadge).gameObject.SetActive(
            Managers.Backend.GameData.QuestData.IsReadyToClaimQuest());
        }));
    }
}
