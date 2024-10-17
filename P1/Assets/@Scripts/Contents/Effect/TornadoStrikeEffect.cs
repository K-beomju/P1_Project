using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoStrikeEffect : EffectBase
{
    private float pullRadius = 4f;       // 빨아들이는 반경
    private float pullForce = 0.4f;        // 한 번에 당기는 힘

    private float circleR = 3; 
    private float objSpeed = 50;
    private float deg; 

    private List<BaseObject> Targets = new List<BaseObject>();

    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        StartCoroutine(MoveCircleCo());
    }

    protected override void ProcessDot()
    {
        Targets = FindTargets(Managers.Object.Monsters);  // 주변의 적들 찾기
        foreach (var target in Targets)
        {
            if (target.IsValid())
            {
                float distanceToMove = pullForce;

                target.transform.position = Vector3.MoveTowards(
                    target.transform.position,           // 현재 위치
                    transform.position,                 // 토네이도 중심 위치
                    distanceToMove                      // 매 틱마다 조금씩 당김
                );
            }
            ApplyDamage(target as Creature);
        }

    }

    // 범위 내의 적들 찾기
    private List<BaseObject> FindTargets(IEnumerable<BaseObject> objs)
    {
        List<BaseObject> visibleEnemies = new List<BaseObject>();
        foreach (BaseObject enemy in objs)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= pullRadius)
            {
                visibleEnemies.Add(enemy);
            }
        }

        return visibleEnemies;
    }


    // Ower 원형 그리며 이동 
    private IEnumerator MoveCircleCo()
    {
        while (true)
        {
            deg += Time.deltaTime * objSpeed;
            if (deg < 360)
            {
                var rad = Mathf.Deg2Rad * (deg);
                var x = circleR * Mathf.Sin(rad);
                var y = circleR * Mathf.Cos(rad);
                transform.position = Owner.transform.position + new Vector3(x, y);
            }
            else
            {
                deg = 0;
            }
            yield return null;
        }

    }

    // 시각적으로 토네이도 범위를 확인하기 위한 디버그 코드
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
