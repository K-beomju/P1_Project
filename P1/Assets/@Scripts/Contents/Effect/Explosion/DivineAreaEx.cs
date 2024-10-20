using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DivineAreaEx : ExplosionBase
{
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

    protected override IEnumerator CoStartTimer()
    {
        yield return transform.DOScale(new Vector3(10, 10), 1).WaitForCompletion();
        yield return new WaitUntil(() => Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        base.ClearEffect();
    }
}
