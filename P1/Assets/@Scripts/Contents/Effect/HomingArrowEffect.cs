using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HomingArrowEffect : EffectBase
{
    private float posA = 3;
    private float posB = 4;
    private float t = 0;
    private float speed = 1.3f;

    private BaseObject Target;
    private Vector2[] point = new Vector2[4];
    private Vector2 previousPosition; 

    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        Target = FindRandomTarget(Managers.Object.Monsters);

        // 베지어 곡선의 4개의 포인트 설정
        point[0] = transform.position;
        point[1] = PointSetting(transform.position); 
        point[2] = PointSetting(Target.transform.position);
        point[3] = Target.CenterPosition; 
        previousPosition = transform.position;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        StartCoroutine(CoLaunchProjectile());
    }

    private IEnumerator CoLaunchProjectile()
    {
        if (Target == null)
        {
            Debug.LogWarning("No target found.");
            yield break;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;

            // 투사체의 현재 위치 계산
            Vector2 currentPosition = new Vector2(
                FourPointBezier(point[0].x, point[1].x, point[2].x, point[3].x),
                FourPointBezier(point[0].y, point[1].y, point[2].y, point[3].y)
            );
            Vector2 direction = currentPosition - previousPosition;
            LookAt2D(direction);
            transform.position = currentPosition;
            previousPosition = currentPosition;
            yield return null;

        }

        // 타겟에 도달한 후 효과 적용
        if (Target.IsValid())
        {
            Target.GetComponent<IDamageable>().OnDamaged(Owner, this);
            //GameObject explosion = Managers.Object.SpawnGameObject(Owner.Target.CenterPosition, EffectData.ExplosionKey);
        }
        base.ClearEffect();

    }

    private Vector2 PointSetting(Vector2 origin)
    {
        float x, y;
        x = posA * Mathf.Cos(Random.Range(0, 360) * Mathf.Deg2Rad) + origin.x;
        y = posB * Mathf.Sin(Random.Range(0, 360) * Mathf.Deg2Rad) + origin.y;
        return new Vector2(x, y);
    }

    private float FourPointBezier(float a, float b, float c, float d)
    {
        return Mathf.Pow((1 - t), 3) * a
        + Mathf.Pow((1 - t), 2) * 3 * t * b
        + Mathf.Pow(t, 2) * 3 * (1 - t) * c
        + Mathf.Pow(t, 3) * d;
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
