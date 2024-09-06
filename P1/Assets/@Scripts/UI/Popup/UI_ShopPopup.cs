using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPopup : UI_Popup
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
