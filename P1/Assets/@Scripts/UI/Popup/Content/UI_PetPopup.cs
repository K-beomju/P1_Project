using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PetPopup : UI_Popup
{
    public enum UI_PetItems 
    {
        UI_PetItem_Silver,
        UI_PetItem_Blue,
        UI_PetItem_Emerald,
        UI_PetItem_Scale,
        UI_PetItem_Wood,
        UI_PetItem_Gold,
        UI_PetItem_Flame,
        UI_PetItem_Book,
        UI_PetItem_Rune
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_PetItem>(typeof(UI_PetItems));
        return true;
    }

    public void RefreshUI()
    {
        // 6개의 슬롯을 초기화
        for (int i = 0; i < Enum.GetValues(typeof(UI_PetItems)).Length; i++)
        {
            Get<UI_PetItem>(i).RefreshUI();
        }
    }
}
