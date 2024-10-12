using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{  
    public Hero Owner { get; private set; } // 이펙트를 생성한 주체
    public Data.SkillData SkillData { get; private set; } // 스킬 정보 데이터

    public virtual void SetInfo(Hero owner, Data.SkillData skillData)
    {
        Owner = owner;
        SkillData = skillData;
        StartCoroutine(CoReserveDestroy(SkillData.SkillDuration));
    }

    // 충돌 감지 메서드
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
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

    private IEnumerator CoReserveDestroy(float lifeTime)
	{
		yield return new WaitForSeconds(lifeTime);
		Managers.Object.Despawn(this);
	}
}
