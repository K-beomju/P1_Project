using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamaged(Creature attacker, EffectBase effect = null);
    void OnDead();
}
