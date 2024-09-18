using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterPopup : UI_Popup
{
    public enum GameObjects
    {
        Content
    }

    public enum UI_AbilityItems
    {
        UI_AbilityItem_Atk,
        UI_AbilityItem_Hp
    }
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        Bind<UI_AbilityItem>(typeof(UI_AbilityItems));

        
        CreateCompanionSlot();
        return true;
    }

    private void CreateCompanionSlot(int defaultCount = 15)
    {
        var slot = GetObject((int)GameObjects.Content);
        for (int i = 0; i < defaultCount; i++)
        {
            Managers.UI.MakeSubItem<UI_CompanionSlot>(slot.transform);
        }
    }
}
