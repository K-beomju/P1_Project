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

    private Coroutine _comboDelayCoroutine = null;
    private UI_HpBarWorldSpace _hpBar;
    private HeroGhost _ghost;

    private bool isDash = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _ghost = GetComponent<HeroGhost>();

        ObjectType = EObjectType.Hero;
        gameObject.layer = (int)ELayer.Hero;
        Anim.SetBool(HeroAnimation.HashCombo, true);

        HeroMoveState = EHeroMoveState.None;
        return true;
    }

    public override void SetCreatureInfo(int dataTemplateID)
    {
        Level = Managers.Purse._currentLevel;
        MaxHp = new CreatureStat(100);
        Hp = MaxHp.Value;
        Atk = new CreatureStat(5);
        AttackSpeedRate = new CreatureStat(1);
        AttackDelay = new CreatureStat(1);
        AttackRange = new CreatureStat(1);
        MoveSpeed = new CreatureStat(3);

        _hpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBarWorldSpace>(gameObject.transform);
        _hpBar.transform.localPosition = new Vector3(0.0f, -0.2f, 0.0f);
        _hpBar.SetSliderInfo(this);
        _hpBar.gameObject.SetActive(false);
    }

    // 레벨업 시 스탯을 다시 계산하는 함수
    public override void ReSetStats()
    {
        base.ReSetStats();
        Debug.Log($"{Level} 레벨에서 공식계산 다시합니다.");
    }

    #region Anim
    protected override void UpdateAnimation()
    {
        Anim.SetBool(HeroAnimation.HashAttack, CreatureState == ECreatureState.Attack);
        Anim.SetBool(HeroAnimation.HashMove, CreatureState == ECreatureState.Move && Target != null);
    }

    public void ComboAttackDelay()
    {
        if (_comboDelayCoroutine != null)
            StopCoroutine(_comboDelayCoroutine);
        _comboDelayCoroutine = StartCoroutine(ComboDelayCo());
    }

    private IEnumerator ComboDelayCo()
    {
        Anim.SetBool(HeroAnimation.HashCombo, false);
        yield return new WaitForSeconds(AttackDelay.Value);
        Anim.SetBool(HeroAnimation.HashCombo, true);
        _comboDelayCoroutine = null;
    }

    public void OnAnimEventHandler()
    {
        if (Target.IsValid() == false)
            return;
        Target.GetComponent<IDamageable>().OnDamaged(this);

        CreatureState = ECreatureState.Move;
    }
    #endregion


    #region AI Update
    protected override void UpdateIdle()
    {
        Target = FindClosestTarget(Managers.Object.Monsters);
        if (Target != null)
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

            Target = target;
            ChaseOrAttackTarget(AttackRange.Value);
        }
        else
        {
            CreatureState = ECreatureState.Idle;
        }
    }

    protected override void UpdateAttack()
    {
        if (Target == null)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        Vector3 dir = (Target.transform.position - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = AttackRange.Value * AttackRange.Value;

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distToTargetSqr > attackDistanceSqr)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        LookAt(dir);
        Anim.SetFloat(HeroAnimation.HashAttackSpeed, AttackSpeedRate.Value);
    }
    #endregion

    #region Target Search & Movement
    private BaseObject FindClosestTarget(IEnumerable<BaseObject> objs)
    {
        if (Managers.Object.BossMonster != null)
            return Managers.Object.BossMonster;

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
        Vector3 dir = (Target.transform.position - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = attackRange * attackRange;
        LookAt(dir);


        if (distToTargetSqr <= attackDistanceSqr)
        {
            CreatureState = ECreatureState.Attack;
            _ghost.makeGhost = false;
            isDash = false;
            return;
        }
        else
        {
            // 대쉬 상태가 아닌 경우에만 조건을 검사
            if (!isDash && distToTargetSqr > DASH_DISTANCE_THRESHOLD)
            {
                isDash = true; 
                _ghost.makeGhost = true; 
            }

            if (isDash)
            {
                Vector3 targetPosition = transform.position + dir.normalized * dir.magnitude;
                transform.position = Vector3.Lerp(transform.position, targetPosition, LERP_SPEED);
            }
            else
            {
                if (dir.magnitude < 0.01f)
                {
                    transform.position = Target.transform.position;
                    return;
                }

                float moveDist = Mathf.Min(dir.magnitude, MoveSpeed.Value * Time.deltaTime);
                TranslateEx(dir.normalized * moveDist);
            }
        }
    }

    private void TranslateEx(Vector3 dir)
    {
        transform.Translate(dir);
    }
    #endregion

    void OnDrawGizmos()
    {
        Vector3 gizmoVec = transform.position + new Vector3(0, 0.35f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoVec, AttackRange.Value);
    }

}