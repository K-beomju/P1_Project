using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterPopup : UI_Popup
{
    public enum GameObjects
    {
        Content
    }

    public enum UI_HeroGrowthInvenSlots
    {
        UI_HeroGrowthInvenSlot_Atk,
        UI_HeroGrowthInvenSlot_Hp
    }
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        Bind<UI_HeroGrowthInvenSlot>(typeof(UI_HeroGrowthInvenSlots));

        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Atk).SetInfo(Define.EHeroUpgradeType.Growth_Atk);
        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Hp).SetInfo(Define.EHeroUpgradeType.Growth_Hp);

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
