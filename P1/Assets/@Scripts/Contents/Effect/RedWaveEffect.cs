using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWaveEffect : EffectBase
{
    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        Sprite.DOFade(0, 1);
    }


    // 데미지 적용 로직
    protected override void ApplyDamage(Creature target)
    {
        target.OnDamaged(Owner, this);
    }

}