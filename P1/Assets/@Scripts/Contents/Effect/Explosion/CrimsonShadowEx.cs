using UnityEngine;

public class CrimsonShadowExplosionEx : ExplosionBase
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
}