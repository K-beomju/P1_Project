using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class Hero : InitBase
{
    #region Variable
    [SerializeField] private float SearchDistance;
    [SerializeField] private float ChaseDistance;
    [SerializeField] private float AttackDistance;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private int ComboCount;
    #endregion

    #region Config
    private CircleCollider2D Collider;
    private SpriteRenderer Sprite;
    private Animator Anim;

    private Box Target;
    private Vector3 CenterPosition { get => Collider.bounds.center; set => CenterPosition = value; }
    private Vector2 _moveDir = Vector2.zero;

    private readonly int hashIsAttackAnimation = Animator.StringToHash("IsAttack");
    private readonly int hashAttackSpeedAnimation = Animator.StringToHash("AttackSpeed");
    #endregion

    #region State
    protected EHeroState _heroState = EHeroState.Idle;
    public virtual EHeroState HeroState
    {
        get { return _heroState; }
        set
        {
            if (_heroState != value)
            {
                _heroState = value;
            }
        }
    }
    #endregion

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = GetComponent<CircleCollider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();

        HeroState = EHeroState.Idle;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
		Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
		Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
		Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        return true;
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
            Anim.SetBool(hashIsAttackAnimation, false);
            HeroState = EHeroState.Idle;
            return;
        }

        Vector3 dir = (Target.transform.position - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = AttackDistance * AttackDistance;

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distToTargetSqr > attackDistanceSqr)
        {
            Anim.SetBool(hashIsAttackAnimation, false);
            HeroState = EHeroState.Idle;
            return;
        }

        Anim.SetBool(hashIsAttackAnimation, true);
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

            float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
            transform.position += dir.normalized * moveDist;
        }
        else
        {
            Target = null;
            HeroState = EHeroState.Idle;
        }
    }

    void Update()
    {
        TranslateEx(_moveDir * Time.deltaTime * MoveSpeed);
    }

    public void TranslateEx(Vector3 dir)
    {
        transform.Translate(dir);
        Sprite.flipX = dir.x < 0;
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
        Debug.Log(dir);
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                HeroState = EHeroState.Move;
                break;
            case EJoystickState.Drag:
                break;
            case EJoystickState.PointerUp:
                HeroState = EHeroState.Idle;
                break;
            default:
                break;
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
