using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_UpdatePopup : UI_Popup
{
    public enum Buttons
    {
        Button_Update
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Button_Update).onClick.AddListener(() =>
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.LukeDogCompany.com.IdleGame");
            Application.Quit();
        });
        return true;
    }
}
