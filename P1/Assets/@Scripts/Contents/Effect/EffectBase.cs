using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectBase : BaseObject
{
    public Hero Owner { get; private set; } // 이펙트를 생성한 주체
    public Data.SkillData SkillData { get; private set; } // 스킬 정보 데이터

    public virtual void SetInfo(Hero owner, Data.SkillData skillData)
    {
        Owner = owner;
        SkillData = skillData;
        Sprite.sortingOrder = SortingLayers.SKILL_EFFECT;
        // 타겟이 유효한지 체크
        if (Owner.Target != null)
        {
            LookAt(Owner.Target.transform.position);
        }
        else
        {
            Debug.LogWarning("Target is null when setting effect info");
        }
        StartCoroutine(CoReserveDestroy(SkillData.SkillDuration));
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

    private IEnumerator CoReserveDestroy(float duration)
    {
        yield return new WaitForSeconds(duration);
        Managers.Object.Despawn(this);
    }
}
