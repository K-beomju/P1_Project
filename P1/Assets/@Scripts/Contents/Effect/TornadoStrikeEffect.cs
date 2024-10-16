using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoStrikeEffect : EffectBase
{
    public float pullRadius = 5f;       // 빨아들이는 반경
    public float pullForce = 2f;        // 한 번에 당기는 힘
    public float duration = 5f;         // 토네이도 지속 시간
    public float pullInterval = 0.2f;   // 당기는 주기 (0.2초)

    private List<BaseObject> Targets = new List<BaseObject>();

    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        Targets = FindTargets(Managers.Object.Monsters);  // 주변의 적들 찾기
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        StartCoroutine(CoPullEnemies());
    }

    // 적을 천천히 나눠서 당기는 코루틴
    private IEnumerator CoPullEnemies()
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += pullInterval; // 매 틱마다 0.2초 증가

            foreach (var target in Targets)
            {
                if (target.IsValid())
                {
                    // 현재 위치와 토네이도 중심의 거리 계산
                    Vector3 direction = (transform.position - target.transform.position).normalized;
                    
                    // 나눠서 끌어당기기: 이동할 거리를 점진적으로 줄이기
                    float distanceToMove = pullForce * pullInterval;  // 0.2초마다 당길 거리 계산

                    // 적을 부드럽게 토네이도 중심으로 이동시키기
                    target.transform.position = Vector3.MoveTowards(
                        target.transform.position,           // 현재 위치
                        transform.position,                 // 토네이도 중심 위치
                        distanceToMove                      // 매 틱마다 조금씩 당김
                    );
                }
            }

            yield return new WaitForSeconds(pullInterval);  // 0.2초마다 당기기
        }

        base.ClearEffect();  // 토네이도 종료 후 클리어
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

    // 시각적으로 토네이도 범위를 확인하기 위한 디버그 코드
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
