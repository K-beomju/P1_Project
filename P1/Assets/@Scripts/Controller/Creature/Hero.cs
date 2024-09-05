using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;


public class Hero : Creature
{
    #region State
    private EHeroMoveState _heroMoveState = EHeroMoveState.None;
    public EHeroMoveState HeroMoveState
    {
        get { return _heroMoveState; }
        set { _heroMoveState = value; }
    }
    #endregion

    #region Variable
    [SerializeField] private float AttackDistance;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackDelay;
    [SerializeField] private float AttackPower;

    public BaseObject _target;
    private Coroutine _comboDelayCoroutine = null;

    #endregion

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _anim.SetBool(HeroAnimation.HashCombo, true);

        CreatureState = ECreatureState.Idle;
        HeroMoveState = EHeroMoveState.None;

        StartCoroutine(CoUpdateAI());
        return true;
    }

    #region Anim
    protected override void UpdateAnimation()
    {
        _anim.SetBool(HeroAnimation.HashAttack, CreatureState == ECreatureState.Attack);
        _anim.SetBool(HeroAnimation.HashMove, CreatureState == ECreatureState.Move && _target != null);
    }

    public void ComboAttackDelay()
    {
        if (_comboDelayCoroutine != null)
            StopCoroutine(_comboDelayCoroutine);
        _comboDelayCoroutine = StartCoroutine(ComboDelayCo());
    }

    private IEnumerator ComboDelayCo()
    {
        _anim.SetBool(HeroAnimation.HashCombo, false);
        yield return new WaitForSeconds(AttackDelay);
        _anim.SetBool(HeroAnimation.HashCombo, true);
        _comboDelayCoroutine = null;
    }

    public void OnAnimEventHandler()
    {
        if (_target.IsValid() == false)
            return;
        _target.GetComponent<IDamageable>().OnDamage(AttackPower);

        CreatureState = ECreatureState.Move;
    }
    #endregion

    #region AI Update
    protected override void UpdateIdle()
    {
        _target = FindClosestTarget(Managers.Object.Monsters);
        if (_target != null)
        {
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.TargetMonster;
            return;
        }
    }

    protected override void UpdateMove()
    {
        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            BaseObject target = FindClosestTarget(Managers.Object.Monsters);
            if (target == null)
            {
                CreatureState = ECreatureState.Idle;
                return;
            }

            _target = target;
            ChaseOrAttackTarget(AttackDistance);
        }
        else
        {
            CreatureState = ECreatureState.Idle;
        }
    }

    protected override void UpdateAttack()
    {
        if (_target == null)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        Vector3 dir = (_target.transform.position - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = AttackDistance * AttackDistance;

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distToTargetSqr > attackDistanceSqr)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        _anim.SetFloat(HeroAnimation.HashAttackSpeed, AttackSpeed);
    }
    #endregion

    #region Target Search & Movement
    private BaseObject FindClosestTarget(IEnumerable<BaseObject> objs)
    {
        BaseObject target = null;
        float bestDistanceSqr = float.MaxValue; // 매우 큰 값으로 초기화하여 첫 번째 비교가 무조건 이루어지게 함

        foreach (BaseObject obj in objs)
        {
            Vector3 dir = obj.transform.position - CenterPosition;
            float distToTargetSqr = dir.sqrMagnitude; // 제곱된 거리 계산

            // 가장 가까운 몬스터를 찾음
            if (distToTargetSqr < bestDistanceSqr)
            {
                target = obj;
                bestDistanceSqr = distToTargetSqr;
            }
        }

        return target;
    }

    private void ChaseOrAttackTarget(float attackRange)
    {
        Vector3 dir = (_target.transform.position - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = attackRange * attackRange;

        if (distToTargetSqr <= attackDistanceSqr)
        {
            CreatureState = ECreatureState.Attack;
            return;
        }
        else
        {
            _sprite.flipX = dir.x < 0;

            if (dir.magnitude < 0.01f)
            {
                transform.position = _target.transform.position;
                return;
            }
            float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
            TranslateEx(dir.normalized * moveDist);
        }
    }

    private void TranslateEx(Vector3 dir)
    {
        transform.Translate(dir);
        _sprite.flipX = dir.x < 0;
    }
    #endregion

    void OnDrawGizmos()
    {
        Vector3 gizmoVec = transform.position + new Vector3(0, 0.35f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoVec, AttackDistance);
    }

}
