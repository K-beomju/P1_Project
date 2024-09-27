using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterInven : UI_Base
{

    public enum UI_HeroGrowthInvenSlots
    {
        UI_HeroGrowthInvenSlot_Atk,
        UI_HeroGrowthInvenSlot_Hp,
        UI_HeroGrowthInvenSlot_Recovery,
        UI_HeroGrowthInvenSlot_CriRate,
        UI_HeroGrowthInvenSlot_CriDmg
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        Bind<UI_HeroGrowthInvenSlot>(typeof(UI_HeroGrowthInvenSlots));

        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Atk).SetInfo(Define.EHeroUpgradeType.Growth_Atk);
        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Hp).SetInfo(Define.EHeroUpgradeType.Growth_Hp);
        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Recovery).SetInfo(Define.EHeroUpgradeType.Growth_Recovery);
        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_CriRate).SetInfo(Define.EHeroUpgradeType.Growth_CriRate);
        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_CriDmg).SetInfo(Define.EHeroUpgradeType.Growth_CriDmg);

        return true;
    }

    private void OnEnable()
    {

    }

    
    private void OnDisable()
    {
        
    }
}
