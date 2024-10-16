using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronWillEffect : EffectBase
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        AddModifier(Owner.ReduceDmgBuff,this);
    }

    public override void ClearEffect()
    {
        RemoveModifier(Owner.ReduceDmgBuff, this);
        base.ClearEffect();
    }
}
