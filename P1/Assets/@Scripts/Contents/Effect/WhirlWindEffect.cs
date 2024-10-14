using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WhirlWindEffect : EffectBase
{
    private HashSet<Creature> _targetsInRange = new HashSet<Creature>();

    // 충돌 감지 메서드 - 적이 범위에 들어오면 리스트에 추가
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Creature enemy = other.GetComponent<Creature>();
            if (enemy.IsValid() && !_targetsInRange.Contains(enemy)) // 중복 방지
            {
                _targetsInRange.Add(enemy); // 범위 내 적 추가
                ApplyDamage(enemy); // 적에게 즉시 데미지 적용
            }
        }
    }

    // 충돌 종료 메서드 - 적이 범위를 벗어나면 리스트에서 제거
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Creature enemy = other.GetComponent<Creature>();
            if (enemy != null && _targetsInRange.Contains(enemy))
            {
                _targetsInRange.Remove(enemy); // 범위에서 나간 적 제거
            }
        }
    }

    // 소용돌이 스킬이 범위 내 적들에게 데미지를 입히기 위한 메서드
    protected override void ProcessDot()
    {
        List<Creature> invalidTargets = new List<Creature>(); // 유효하지 않은 타겟들을 추적할 리스트

        // HashSet을 복사한 리스트에서 순회
        foreach (var target in _targetsInRange.ToList())
        {
            if (target == null || !target.IsValid()) // 타겟이 유효하지 않다면
            {
                invalidTargets.Add(target); // 나중에 HashSet에서 제거
            }
            else
            {
                ApplyDamage(target);  // 범위 내 모든 유효한 적들에게 주기적인 데미지 적용
            }
        }

        // 유효하지 않은 타겟들을 HashSet에서 제거
        foreach (var invalidTarget in invalidTargets)
        {
            _targetsInRange.Remove(invalidTarget);
        }
    }

    // 데미지를 입히는 구체적인 로직
    protected override void ApplyDamage(Creature target)
    {
        target.OnDamaged(Owner, this);
    }
}
