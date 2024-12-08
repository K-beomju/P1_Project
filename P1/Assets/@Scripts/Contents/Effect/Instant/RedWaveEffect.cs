using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWaveEffect : EffectBase
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        Sprite.color = Color.white;
        Sprite.DOFade(0, 1);
    }

    // 데미지 적용 로직
    protected override void ApplyDamage(Creature target)
    {
        if(gameObject.activeInHierarchy)
        target.OnDamaged(Owner, this);
    }


}
