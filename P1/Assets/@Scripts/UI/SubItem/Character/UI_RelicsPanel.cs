using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_RelicsPanel : UI_Base
{
    public enum Buttons
    {
        Btn_DrawTen,
        Btn_DrawThirty
    }

    public enum UI_RelicGrowthSlots
    {
        UI_RelicGrowInvenSlot_Atk,
        UI_RelicGrowInvenSlot_MaxHp,
        UI_RelicGrowInvenSlot_Recovery,
        UI_RelicGrowInvenSlot_MonsterDmg,
        UI_RelicGrowInvenSlot_BossMonsterDmg,
        UI_RelicGrowInvenSlot_ExpRate,
        UI_RelicGrowInvenSlot_GoldRate
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_RelicGrowInvenSlot>(typeof(UI_RelicGrowthSlots));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawRelic(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawRelic(30));

        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_Atk).SetInfo(EHeroRelicType.Relic_Atk);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_MaxHp).SetInfo(EHeroRelicType.Relic_MaxHp);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_Recovery).SetInfo(EHeroRelicType.Relic_Recovery);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_MonsterDmg).SetInfo(EHeroRelicType.Relic_MonsterDmg);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_BossMonsterDmg).SetInfo(EHeroRelicType.Relic_BossMonsterDmg);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_ExpRate).SetInfo(EHeroRelicType.Relic_ExpRate);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_GoldRate).SetInfo(EHeroRelicType.Relic_GoldRate);

        return true;
    }

    private void OnDrawRelic(int drawCount)
    {
       
    }

}
