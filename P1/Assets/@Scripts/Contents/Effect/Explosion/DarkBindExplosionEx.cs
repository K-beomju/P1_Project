using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBindExplosionEx : ExplosionBase
{
    public void SetInfo(int templateID, Hero owner, SkillData skillData, Creature target)
    {
        Target = target;
        base.SetInfo(templateID, owner, skillData);
        StartCoroutine(CoStartTimer());
    }

    protected override IEnumerator CoStartTimer()
    {
        yield return new WaitUntil(() => Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        if (Target != null && EffectData != null)
            ApplyDamage(Target);
        base.ClearEffect();
    }

}
