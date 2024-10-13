using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Data;

public class EffectBase : BaseObject
{
    public Hero Owner { get; private set; }
    public SkillData SkillData { get; private set; }
    public EffectData EffectData { get; private set; }
    public EEffectType EffectType { get; private set; }

    protected float Duration { get; private set; }

    public virtual void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
        DataTemplateID = templateID;
        EffectData = Managers.Data.EffectChart[DataTemplateID];
        Owner = owner;
        SkillData = skillData;

        EffectType = EffectData.EffectType;
        Duration = EffectData.Duration;
        Sprite.sortingOrder = SortingLayers.SKILL_EFFECT;

        StartCoroutine(CoStartTimer());
    }

    // 충돌 감지 메서드
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Creature enemy = other.GetComponent<Creature>();
            if (enemy != null)
            {
                ApplyDamage(enemy);
            }
        }
    }

    // 데미지를 적용하는 메서드 (하위 클래스에서 구체적으로 구현)
    protected virtual void ApplyDamage(Creature target)
    {
        // 구체적인 데미지 계산은 하위 클래스에서 구현
    }

    protected virtual void ProcessDot()
    {

    }

    protected virtual IEnumerator CoStartTimer()
    {
        ProcessDot();

        if (EffectType == EEffectType.HasDuration)
        {
            float sumTime = 0f;
            float remainingDuration = Duration;

            while (remainingDuration > 0)
            {
                remainingDuration -= Time.deltaTime;
                sumTime += Time.deltaTime;

                // 틱마다 ProcessDot 호출
                if (sumTime >= EffectData.TickTime)
                {
                    ProcessDot();
                    sumTime -= EffectData.TickTime;
                }

                yield return null;
            }
        }
        else if (EffectType == EEffectType.Instant)
        {
            yield return new WaitForSeconds(Duration); // 단발성 이펙트는 Duration 동안 유지될 수 있음
        }
        Managers.Object.Despawn(this);
    }
}
