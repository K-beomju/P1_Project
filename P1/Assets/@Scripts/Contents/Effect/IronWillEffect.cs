using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronWillEffect : EffectBase
{
    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        AddModifier(Owner.ReduceDmgBuff,this);
    }

    public override void ClearEffect()
    {
        RemoveModifier(Owner.ReduceDmgBuff, this);
        base.ClearEffect();
    }
}
