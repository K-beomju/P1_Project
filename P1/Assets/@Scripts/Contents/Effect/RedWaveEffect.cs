using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWaveEffect : EffectBase
{
    // 데미지 적용 로직
    protected override void ApplyDamage(Creature target)
    {
        float damage = SkillData.UsedValue; // 스킬의 데미지 값 사용
        target.OnDamaged(Owner);
        Debug.Log($"{target.name}에게 {damage} 데미지를 입혔습니다.");
    }

}
