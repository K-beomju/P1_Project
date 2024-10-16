using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ExplosionBase : EffectBase
{
    protected Animator Anim { get; set; }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Sprite.sortingOrder = SortingLayers.SKILL_EFFECT;
        Anim = GetComponent<Animator>();
        StartCoroutine(CoStartTimer());
        return true;
    }

    protected override IEnumerator CoStartTimer()
    {
        yield return new WaitUntil(() => Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        base.ClearEffect();
    }
}

