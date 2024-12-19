using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProjectileBase : EffectBase
{
    protected Vector3 startPosition;
    protected Vector3 targetPosition;
    protected float moveSpeed = 10;
    protected BaseObject Target;

    protected abstract IEnumerator CoLaunchProjectile();

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        StartCoroutine(CoLaunchProjectile());
    }

    protected void LookAt2D(Vector2 forward)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    // 무작위 적들 찾기
    protected BaseObject FindRandomTarget(IEnumerable<BaseObject> objs)
    {
        // 객체가 없으면 null 반환
        if (Managers.Object.BossMonster.IsValid())
            return Managers.Object.BossMonster;

        if (Managers.Object.RankMonster.IsValid())
            return Managers.Object.RankMonster;

        if (!objs.Any())
            return null;

        // 무작위로 타겟 선택
        System.Random rand = new System.Random();
        BaseObject target = objs.ElementAt(rand.Next(objs.Count()));
        return target;
    }
}
