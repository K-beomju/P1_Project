using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hero : MonoBehaviour
{
    #region Variable
    [SerializeField] private float SearchDistance;
    [SerializeField] private int ChaseDistance;
    [SerializeField] private int AttackDistance;
    [SerializeField] private int moveSpeed;

    #endregion

    #region Config
    private CircleCollider2D Collider;
    private SpriteRenderer Sprite;
    private Animator Anim;
    
    private Box Target;
    private Vector3 CenterPosition { get => Collider.bounds.center; set => CenterPosition = value; }
    #endregion

    public enum EHeroState
    {
        Idle,
        Move,
        Attack
    }
    private EHeroState HeroState = EHeroState.Idle;


    private void Awake()
    {
        Collider = GetComponent<CircleCollider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        
        HeroState = EHeroState.Idle;
    }

    private void Start()
    {
        StartCoroutine(CoUpdateAI());
    }

    private IEnumerator CoUpdateAI()
    {
        while (true)
        {
            switch (HeroState)
            {
                case EHeroState.Idle:
                    UpdateIdle();
                    break;
                case EHeroState.Move:
                    UpdateMove();
                    break;
                case EHeroState.Attack:
                    UpdateAttack();
                    break;
            }
            yield return null;
        }
    }

    private void UpdateIdle()
    {
        Box target = FindClosestInRange(SearchDistance, Managers.Object.boxes);
        if (target != null)
        {
            Target = target;
            Debug.Log("ASd");
            HeroState = EHeroState.Move;
        }
    }

    private void UpdateMove()
    {
       if (Target == null)
    {
        // 타겟이 없으면 Idle 상태로 전환
        HeroState = EHeroState.Idle;
        return;
    }
        ChaseOrAttackTarget(AttackDistance, ChaseDistance);
        return;
    }
    private void UpdateAttack()
    {
        if (Target == null)
        {
            HeroState = EHeroState.Idle;
            return;
        }

        Vector3 dir = (Target.transform.position - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = AttackDistance * AttackDistance;

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distToTargetSqr > attackDistanceSqr)
        {
            HeroState = EHeroState.Idle;
            return;
        }

    }

    private Box FindClosestInRange(float range, IEnumerable<Box> objs)
    {
        Box target = null;
        float bestDistanceSqr = float.MaxValue;
        float searchDistanceSqr = range * range;

        foreach (Box obj in objs)
        {
            Vector3 dir = obj.transform.position - CenterPosition;
            float distToTargetSqr = dir.sqrMagnitude;

            if (distToTargetSqr > searchDistanceSqr)
                continue;

            if (distToTargetSqr > bestDistanceSqr)
                continue;

            target = obj;
            bestDistanceSqr = distToTargetSqr;
        }
        return target;
    }

    private void ChaseOrAttackTarget(float attackRange, float chaseRange)
    {
        Vector3 dir = (Target.transform.position - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = attackRange * attackRange;
        float chaseDistanceSqr = chaseRange * chaseRange;

        if (distToTargetSqr <= attackDistanceSqr)
        {
            // 공격 범위 이내로 들어옴 
            Debug.Log(" 공격 범위 이내로 들어옴 ");

            HeroState = EHeroState.Attack;
            return;
        }
        else if (distToTargetSqr <= chaseDistanceSqr)
        {
            Sprite.flipX = dir.x < 0;
            Debug.Log(" chaseDistanceSqr ");

            if (dir.magnitude < 0.01f)
            {
                Debug.Log(" dir.magnitude < 0.01f) ");

                transform.position = Target.transform.position;
                return;
            }

            float moveDist = Mathf.Min(dir.magnitude, moveSpeed * Time.deltaTime);
            transform.position += dir.normalized * moveDist;
        }
        else
        {
            Target = null;
            HeroState = EHeroState.Idle;
        }
    }




    void OnDrawGizmos()
    {
        Vector3 gizmoVec = transform.position + new Vector3(0, 0.35f);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(gizmoVec, SearchDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gizmoVec, ChaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoVec, AttackDistance);
    }
}
