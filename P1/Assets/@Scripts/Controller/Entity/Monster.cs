using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : InitBase, IDamageable
{
    [SerializeField] private float Hp;
    [SerializeField] private float MaxHp;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        MaxHp = Hp;
        return true;
    }

    public void OnDamage(float damage)
    {
		float finalDamage = damage; // TODO
		Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

		if (Hp <= 0)
		{
			OnDead();
		}
    }

    public void OnDead()
    {
		Managers.Object.Despawn(this);
    }

}
