using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBindExplosion : ExplosionBase
{
    public Creature Target { get; set; }

    protected override IEnumerator CoStartTimer()
    {
        yield return new WaitUntil(() => Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        if (Target != null && EffectData != null)
            ApplyDamage(Target);
        base.ClearEffect();
    }

    protected override void ApplyDamage(Creature target)
    {
        target.OnDamaged(Owner, this);
    }
}
