using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ExplosionEffect : EffectBase
{
    private Animator Anim;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Sprite.sortingOrder = SortingLayers.SKILL_EFFECT;
        Anim = GetComponent<Animator>();
        StartCoroutine(CoStartTimer());
        return true;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Creature enemy = other.GetComponent<Creature>();
            if (enemy != null && EffectData != null)
            {
                ApplyDamage(enemy);
            }
        }
    }

    protected override void ApplyDamage(Creature target)
    {
        target.OnDamaged(Owner, this);
    }

    protected override IEnumerator CoStartTimer()
    {
        yield return new WaitUntil(() => Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        base.ClearEffect();
    }
}

