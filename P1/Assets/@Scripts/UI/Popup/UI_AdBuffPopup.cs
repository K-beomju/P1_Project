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

    public enum AdBuffItems
    {
        AdBuffItem_Gold,
        AdBuffItem_Exp,
        AdBuffItem_Atk
    }

    protected override bool Init()
    {
        if(base.Init() == false)
        return false;
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        Bind<UI_AdBuffItem>(typeof(AdBuffItems));

        GetObject((int)GameObjects.BG).gameObject.BindEvent(() => ClosePopupUI());
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() => ClosePopupUI());
        Get<UI_AdBuffItem>((int)AdBuffItems.AdBuffItem_Gold).SetInfo(Define.EAdBuffType.IncreaseGold);
        Get<UI_AdBuffItem>((int)AdBuffItems.AdBuffItem_Exp).SetInfo(Define.EAdBuffType.IncreaseExp);
        Get<UI_AdBuffItem>((int)AdBuffItems.AdBuffItem_Atk).SetInfo(Define.EAdBuffType.Atk);

        return true;
    }
}
