using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ReviewPopup : UI_Popup
{
    public enum Buttons 
    {
        Btn_Yes,
        Btn_No
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_Yes).onClick.AddListener(() =>
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.LukeDogCompany.com.IdleGame");
            Managers.UI.ClosePopupUI();
        });

        GetButton((int)Buttons.Btn_No).onClick.AddListener(() =>
        {
            Managers.UI.ClosePopupUI();
        });

        return true;
    }
}
