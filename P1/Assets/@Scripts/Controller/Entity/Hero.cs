using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class Hero : InitBase
{
    #region State
    private EHeroState _heroState = EHeroState.Idle;
    public virtual EHeroState HeroState
    {
        get { return _heroState; }
        set
        {
            if (_heroState != value)
            {
                _heroState = value;
                UpdateAnimation();
            }
        }
    }

    private EHeroMoveState _heroMoveState = EHeroMoveState.None;
    public EHeroMoveState HeroMoveState
    {
        get { return _heroMoveState; }
        set { _heroMoveState = value; }
    }
    #endregion

    #region Variable
    [SerializeField] private float SearchDistance;
    [SerializeField] private float AttackDistance;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackDelay;

    private CircleCollider2D Collider;
    private SpriteRenderer Sprite;
    private Animator Anim;

    private Monster Target;

    private Vector3 CenterPosition { get => Collider.bounds.center; set => CenterPosition = value; }
    private Vector2 MoveDir = Vector2.zero;
    private Coroutine comboDelayCo = null;
    #endregion

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = GetComponent<CircleCollider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();

        HeroState = EHeroState.Idle;
        HeroMoveState = EHeroMoveState.None;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        StartCoroutine(CoUpdateAI());
        return true;
    }

    #region Anim
    private void UpdateAnimation()
    {
        switch (HeroState)
        {
            case EHeroState.Idle:
                Anim.SetBool(HeroAnimation.HashAttack, false);
                break;
            case EHeroState.Move:
                Anim.SetBool(HeroAnimation.HashAttack, false);
                Anim.SetBool(HeroAnimation.HashMove, true);
                break;
            case EHeroState.Attack:
                Anim.SetBool(HeroAnimation.HashAttack, true);
                Anim.SetBool(HeroAnimation.HashMove, false);
                break;
            default:
                break;
        }
    }

    public void ComboAttackDelay()
    {
        if (comboDelayCo != null)
            StopCoroutine(comboDelayCo);
        comboDelayCo = StartCoroutine(ComboDelayCo());
    }


    private IEnumerator ComboDelayCo()
    {
        Anim.SetBool(HeroAnimation.HashCombo, false);
        yield return new WaitForSeconds(AttackDelay);
        Anim.SetBool(HeroAnimation.HashCombo, true);
        comboDelayCo = null;
    }
    #endregion

    #region AI Update
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
        // 0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            HeroState = EHeroState.Move;
            return;
        }

        Monster target = FindClosestInRange(SearchDistance, Managers.Object.Monsters);
        if (target != null)
        {
            Target = target;
            HeroState = EHeroState.Move;
            HeroMoveState = EHeroMoveState.TargetMonster;
            return;
        }
        else
        {
            Anim.SetBool(HeroAnimation.HashMove, false);
        }
    }

    private void UpdateMove()
    {
        // 0. 누르고 있다면, 강제 이동
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            TranslateEx(MoveDir * Time.deltaTime * MoveSpeed);
            return;
        }

        // 1. 주변 몬스터 서치
        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            if (Target == null)
            {
                // 타겟이 없으면 Idle 상태로 전환
                HeroState = EHeroState.Idle;
                return;
            }
            ChaseOrAttackTarget(AttackDistance, SearchDistance);
            return;
        }

        HeroState = EHeroState.Idle;
    }
    private void UpdateAttack()
    {
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            HeroState = EHeroState.Move;
            return;
        }

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

        Anim.SetFloat(HeroAnimation.HashAttackSpeed, AttackSpeed);
    }
    #endregion

    #region Target Search & Movement
    private Monster FindClosestInRange(float range, IEnumerable<Monster> objs)
    {
        Monster target = null;
        float bestDistanceSqr = float.MaxValue;
        float searchDistanceSqr = range * range;

        foreach (Monster obj in objs)
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
            HeroState = EHeroState.Attack;
            return;
        }
        else if (distToTargetSqr <= chaseDistanceSqr)
        {
            Sprite.flipX = dir.x < 0;

            if (dir.magnitude < 0.01f)
            {
                transform.position = Target.transform.position;
                return;
            }
            float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
            TranslateEx(dir.normalized * moveDist);
        }
        else
        {
            Target = null;
            HeroMoveState = EHeroMoveState.None;
            HeroState = EHeroState.Idle;
        }
    }

    private void TranslateEx(Vector3 dir)
    {
        transform.Translate(dir);
        Sprite.flipX = dir.x < 0;
    }
    #endregion


    #region Input Handlers
    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        MoveDir = dir;
        Anim.SetFloat(HeroAnimation.HashMoveDir,  Mathf.Abs(MoveDir.x));
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case EJoystickState.PointerUp:
                HeroMoveState = EHeroMoveState.None;
                break;
            default:
                break;
        }
    }
    #endregion

    void OnDrawGizmos()
    {
        Vector3 gizmoVec = transform.position + new Vector3(0, 0.35f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gizmoVec, SearchDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoVec, AttackDistance);
    }

}
