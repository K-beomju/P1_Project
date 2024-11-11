using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AdBuffPopup : UI_Popup
{
    public enum GameObjects 
    {
        BG
    }
    public enum Buttons 
    {
        Btn_Exit
    }

    protected override bool Init()
    {
        if(base.Init() == false)
        return false;
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetObject((int)GameObjects.BG).gameObject.BindEvent(() => ClosePopupUI());
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() => ClosePopupUI());
        return true;
    }
}
