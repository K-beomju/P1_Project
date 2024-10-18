using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkWarrior : ExplosionBase
{
    public override void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
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
                Debug.LogWarning("asd");
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
