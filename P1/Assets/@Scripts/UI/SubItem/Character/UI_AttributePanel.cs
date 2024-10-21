using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_AttributePanel : UI_Base
{
    public enum UI_AttributeGrowthSlots
    {
        UI_AttributeGrowthInvenSlot_Atk,
        UI_AttributeGrowthInvenSlot_Hp,
        UI_AttributeGrowthInvenSlot_CriRate,
        UI_AttributeGrowthInvenSlot_CriDmg,
        UI_AttributeGrowthInvenSlot_SkillTime,
        UI_AttributeGrowthInvenSlot_SkillDmg
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Bind<UI_AttributeGrowhInvenSlot>(typeof(UI_AttributeGrowthSlots));
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_Atk).SetInfo(EHeroAttrType.Growth_Atk);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_Hp).SetInfo(EHeroAttrType.Growth_Hp);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriRate).SetInfo(EHeroAttrType.Growth_CriRate);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriDmg).SetInfo(EHeroAttrType.Growth_CriDmg);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillTime).SetInfo(EHeroAttrType.Growth_SkillTime);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillDmg).SetInfo(EHeroAttrType.Growth_SkillDmg);
        return true;
    }

    public void RefreshUI()
    {
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_Atk).UpdateSlotInfoUI();
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_Hp).UpdateSlotInfoUI();
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriRate).UpdateSlotInfoUI();
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriDmg).UpdateSlotInfoUI();
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillTime).UpdateSlotInfoUI();
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillDmg).UpdateSlotInfoUI();
    }
}
