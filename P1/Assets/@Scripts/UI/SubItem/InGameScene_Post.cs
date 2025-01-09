using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class InGameScene_Post : UI_Base
{
    public enum Images 
    {
        Post_NotifiBadge
    }

    private Button _postBtn;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        _postBtn = GetComponent<Button>();
        _postBtn.onClick.AddListener(OnClickButton);

        return true;
    }

    private void OnClickButton()
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_PostPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
        popupUI.RefreshUI();
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.PostCheckNotification, new Action(RefreshUI));
        RefreshUI();
    }

    void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.PostCheckNotification, new Action(RefreshUI));
    }

    private void RefreshUI()
    {
        if(Managers.Backend.Post.Dictionary != null)
            GetImage((int)Images.Post_NotifiBadge).gameObject.SetActive(Managers.Backend.Post.Dictionary.Count > 0);
    }

}
