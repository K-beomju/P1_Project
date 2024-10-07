using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Bot : Creature, IDamageable
{
     protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Monster;
        gameObject.layer = (int)ELayer.Monster;
        return true;
    }

    public override void SetCreatureInfo(int dataTemplateID)
    {
        MaxHp = 1000000000000;
        Hp = MaxHp;

        HpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBarWorldSpace>(gameObject.transform);
        HpBar.transform.localPosition = new Vector3(0.0f, 1.4f, 0.0f);
        HpBar.SetSliderInfo(this);
        HpBar.gameObject.SetActive(true);
    }


    protected override void UpdateIdle()
    {

    }

    
    protected override void UpdateMove()
    {
       
    }

    protected override void UpdateDead()
    {
        OnDead();
    }

    public override void OnDamaged(Creature attacker)
    {
        base.OnDamaged(attacker);
        Sprite.flipX = transform.position.x > attacker.transform.position.x;

    }

    public override void OnDead()
    {

    }
}
