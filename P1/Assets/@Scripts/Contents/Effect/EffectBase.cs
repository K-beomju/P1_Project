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
    public EEffectSpawnType EffectSpawnType { get; protected set; }
    public EEffectType EffectType { get; protected set; }

    protected float Duration { get; private set; }

    public virtual void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
        DataTemplateID = templateID;
        EffectData = Managers.Data.EffectChart[DataTemplateID];
        Owner = owner;
        SkillData = skillData;

        EffectSpawnType = EffectData.EffectSpawnType;
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

	protected void AddModifier(CreatureStat stat, object source, int order = 0)
	{
		if (SkillData.DamageMultiplier != 0)
		{
			StatModifier percentAdd = new StatModifier(SkillData.DamageMultiplier, EStatModType.PercentAdd, order, source);
			stat.AddModifier(percentAdd);
		}
	}

	protected void RemoveModifier(CreatureStat stat, object source)
	{
		stat.ClearModifiersFromSource(source);
	}

    public virtual void ClearEffect()
    {
        Managers.Object.Despawn(this);
    }
    

    protected virtual void ApplyDamage(Creature target)
    {
    }

    protected virtual void ProcessDot()
    {
    }

    protected virtual IEnumerator CoStartTimer()
    {
        ProcessDot();

        if (EffectSpawnType == EEffectSpawnType.HasDuration)
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
        else if (EffectSpawnType == EEffectSpawnType.Instant)
        {
            yield return new WaitForSeconds(Duration); // 단발성 이펙트는 Duration 동안 유지될 수 있음
        }

        ClearEffect();
    }


}
