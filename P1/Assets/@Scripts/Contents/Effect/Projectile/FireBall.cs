using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireBall : ProjectileBase
{
    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        Target = FindRandomTarget(Managers.Object.Monsters);
        startPosition = transform.position;
        moveSpeed = 10;

        if(!Target.IsValid())
        {
            base.ClearEffect();
            return;
        }
        targetPosition = Target.transform.position;
    }

    protected override IEnumerator CoLaunchProjectile()
    {
        if(!Target.IsValid())
        {
            base.ClearEffect();
            yield break;
        }

        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float totalTime = journeyLength / moveSpeed;
        float elapsedTime = 0;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / totalTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, normalizedTime);

            LookAt2D(targetPosition - transform.position);

            yield return null;
        }
        Managers.Object.SpawnGameObject(transform.position, EffectData.ExplosionKey);

        if (Target.IsValid())
        {
            ApplyDamage(Target as Creature);
        }
        base.ClearEffect();

    }

}
