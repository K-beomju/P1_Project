using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_StageFailPopup : UI_Popup
{
    public enum GameObjects
    {
        BG
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, false, SortingLayers.UI_SCENE + 1);

        BindObjects(typeof(GameObjects));
        GetObject((int)GameObjects.BG).BindEvent(ClosePopupUI, EUIEvent.Click);
        return true;
    }
}
