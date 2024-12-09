using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWarriorEx : ExplosionBase
{
    public void SetInfo(int templateID, Hero owner, SkillData skillData, Creature target)
    {
        Target = target;
        base.SetInfo(templateID, owner, skillData);
        StartCoroutine(CoStartTimer());
    }

    protected override IEnumerator CoStartTimer()
    {
        ApplyDamage(Target);
        
        float sumTime = 0f;
        float remainingDuration = EffectData.Duration;

        while (remainingDuration > 0)
        {
            remainingDuration -= Time.deltaTime;
            sumTime += Time.deltaTime;

            // 틱마다 ProcessDot 호출
            if (sumTime >= EffectData.TickTime)
            {
                ApplyDamage(Target);
                sumTime -= EffectData.TickTime;
            }

            if (Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                break; // 루프 종료
            }

            yield return null;
        }

        // 효과 제거
        base.ClearEffect();
    }

    protected override bool ShouldStartTimer() => false; 

}
