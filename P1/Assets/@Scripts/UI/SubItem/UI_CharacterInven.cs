using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterInven : UI_Base
{

    public enum UI_HeroGrowthInvenSlots
    {
        UI_HeroGrowthInvenSlot_Atk,
        UI_HeroGrowthInvenSlot_Hp
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        Bind<UI_HeroGrowthInvenSlot>(typeof(UI_HeroGrowthInvenSlots));

        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Atk).SetInfo(Define.EHeroUpgradeType.Growth_Atk);
        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Hp).SetInfo(Define.EHeroUpgradeType.Growth_Hp);

        return true;
    }

    private void OnEnable()
    {

    }

    
    private void OnDisable()
    {
        
    }
}
