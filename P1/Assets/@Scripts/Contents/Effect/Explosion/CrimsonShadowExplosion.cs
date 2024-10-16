using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonShadowExplosion : ExplosionBase
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

    protected override void ApplyDamage(Creature target)
    {
        target.OnDamaged(Owner, this);
    }
}
