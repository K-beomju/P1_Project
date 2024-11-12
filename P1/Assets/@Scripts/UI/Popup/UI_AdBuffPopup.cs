using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

    public enum AdBuffScrollItems
    {
        UI_AdBuffScrollItem_Gold,
        UI_AdBuffScrollItem_Exp,
        UI_AdBuffScrollItem_Atk
    }

    protected override bool Init()
    {
        if(base.Init() == false)
        return false;
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        Bind<UI_AdBuffScrollItem>(typeof(AdBuffScrollItems));

        GetObject((int)GameObjects.BG).gameObject.BindEvent(() => ClosePopupUI());
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() => ClosePopupUI());
        Get<UI_AdBuffScrollItem>((int)AdBuffScrollItems.UI_AdBuffScrollItem_Gold).SetInfo(EAdBuffType.IncreaseGold);
        Get<UI_AdBuffScrollItem>((int)AdBuffScrollItems.UI_AdBuffScrollItem_Exp).SetInfo(EAdBuffType.IncreaseExp);
        Get<UI_AdBuffScrollItem>((int)AdBuffScrollItems.UI_AdBuffScrollItem_Atk).SetInfo(EAdBuffType.Atk);

        return true;
    }
}
