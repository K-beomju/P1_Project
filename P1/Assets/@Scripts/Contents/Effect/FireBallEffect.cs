using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireBallEffect : EffectBase
{
    public Vector3 StartPosition;
    public Vector3 TargetPosition;
    public Action EndCallback;

    private float speed = 10;

    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        BaseObject Target = FindRandomTarget(Managers.Object.Monsters);
        StartPosition = transform.position;
        TargetPosition = Target.transform.position;
        EndCallback = () =>
        {
            if (Target.IsValid())
            {
                GameObject effectObj = Managers.Object.SpawnGameObject(Target.CenterPosition, EffectData.ExplosionKey);
                Target.GetComponent<IDamageable>().OnDamaged(Owner, this);
            }
            base.ClearEffect();
        };

        StartCoroutine(CoLaunchProjectile());

    }


    private IEnumerator CoLaunchProjectile()
    {
        float journeyLength = Vector3.Distance(StartPosition, TargetPosition);
        float totalTime = journeyLength / speed;
        float elapsedTime = 0;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / totalTime;
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, normalizedTime);

            LookAt2D(TargetPosition - transform.position);

            yield return null;
        }

        EndCallback?.Invoke();
    }

    private void LookAt2D(Vector2 forward)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    private BaseObject FindRandomTarget(IEnumerable<BaseObject> objs)
    {
        // 객체가 없으면 null 반환
        if (!objs.Any())
            return null;

        // 무작위로 타겟 선택
        System.Random rand = new System.Random();
        BaseObject target = objs.ElementAt(rand.Next(objs.Count()));
        return target;
    }

}
