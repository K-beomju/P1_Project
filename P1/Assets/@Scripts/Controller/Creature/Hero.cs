using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
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

    public HeroInfo HeroInfo { get; private set; }
    private Coroutine comboDelayCoroutine = null;
    private Coroutine recoveryCoroutine = null;
    private HeroGhost ghost;

    private AnimatorController handController;
    private AnimatorController weaponController;
    private WaitForSeconds recoveryTime = new WaitForSeconds(1);

    private bool isDash = false;
    private bool isWeapon = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ghost = GetComponent<HeroGhost>();

        ObjectType = EObjectType.Hero;
        gameObject.layer = (int)ELayer.Hero;
        handController = Managers.Resource.Load<AnimatorController>("Animations/Hero_Hand");
        weaponController = Managers.Resource.Load<AnimatorController>("Animations/Hero_Weapon");
        Anim.SetBool(HeroAnimation.HashCombo, true);

        HeroMoveState = EHeroMoveState.None;
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.HeroUpgradeUpdated, new Action(ReSetStats));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.HeroUpgradeUpdated, new Action(ReSetStats));
    }


    public override void SetCreatureInfo(int dataTemplateID)
    {
        HeroInfo = Managers.Hero.PlayerHeroInfo;

        Atk = HeroInfo.Atk;
        MaxHp = HeroInfo.MaxHp;
        Hp = MaxHp;
        Recovery = HeroInfo.Recovery;
        CriRate = HeroInfo.CriRate;
        CriDmg = HeroInfo.CriDmg;

        AttackDelay = HeroInfo.AttackDelay;
        AttackRange = HeroInfo.AttackRange;
        AttackSpeedRate = HeroInfo.AttackSpeedRate;
        MoveSpeed = 3;

        HpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBarWorldSpace>();
        HpBar._offset = new Vector3(0.0f, -0.2f, 0.0f);
        HpBar.SetSliderInfo(this);
        HpBar.gameObject.SetActive(true);

    }

    // 레벨업 시 스탯을 다시 계산하는 함수
    public override void ReSetStats()
    {
        Atk = HeroInfo.Atk;
        MaxHp = HeroInfo.MaxHp;
        Hp = MaxHp;

        Debug.Log($"공격력 {Atk}");
        Debug.Log($"체력 {Hp}");

        Debug.Log($"{Level} 레벨에서 공식계산 다시합니다.");
    }

    #region Anim
    protected override void UpdateAnimation()
    {
        if(CreatureState == ECreatureState.Dead)
            Anim.SetTrigger(HeroAnimation.HashDead);

        Anim.SetBool(HeroAnimation.HashAttack, CreatureState == ECreatureState.Attack);
        Anim.SetBool(HeroAnimation.HashMove, CreatureState == ECreatureState.Move && Target != null);
    }

    public void ComboAttackDelay()
    {
        if (comboDelayCoroutine != null)
            StopCoroutine(comboDelayCoroutine);
        comboDelayCoroutine = StartCoroutine(ComboDelayCo());
    }

    private IEnumerator ComboDelayCo()
    {
        Anim.SetBool(HeroAnimation.HashCombo, false);
        yield return new WaitForSeconds(AttackDelay);
        Anim.SetBool(HeroAnimation.HashCombo, true);
        comboDelayCoroutine = null;
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
            ChaseOrAttackTarget(AttackRange);
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
        float attackDistanceSqr = AttackRange * AttackRange;

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distToTargetSqr > attackDistanceSqr)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        LookAt(dir);
        Anim.SetFloat(HeroAnimation.HashAttackSpeed, AttackSpeedRate);
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
            ghost.makeGhost = false;
            isDash = false;
            return;
        }
        else
        {
            // 대쉬 상태가 아닌 경우에만 조건을 검사
            if (!isDash && distToTargetSqr > DASH_DISTANCE_THRESHOLD)
            {
                isDash = true;
                ghost.makeGhost = true; 
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

                float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
                TranslateEx(dir.normalized * moveDist);
            }
        }
    }

    private void TranslateEx(Vector3 dir)
    {
        transform.Translate(dir);
    }

    public void ChangeAnimController(bool weapon = false)
    {
        Anim.runtimeAnimatorController = weapon ? weaponController : handController;
        Anim.Rebind();
        CreatureState = ECreatureState.Idle;
        isWeapon = true;
    }
    #endregion

    public override void OnDamaged(Creature attacker)
    {
        base.OnDamaged(attacker);

        if(recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
        HpBar.DoFadeSlider(() =>
        {
            recoveryCoroutine = StartCoroutine(RecoveryCo());
        });
    }

    public override void OnDead()
    {
        //TODO
        Managers.UI.ShowBaseUI<UI_FadeInBase>().ShowFadeIn(1f, 1f, () =>
        {
            Anim.Rebind();
            Hp = MaxHp;
            CreatureState = ECreatureState.Idle;
            Managers.Scene.GetCurrentScene<GameScene>().GameSceneState = EGameSceneState.Over;

        }, () =>
        {
        });
    }

    private IEnumerator RecoveryCo()
    {
        while(true)
        {
            yield return recoveryTime;
            Hp += Recovery;
            if(Hp >= MaxHp)
            {
                Hp = MaxHp;
                yield break;
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 gizmoVec = transform.position + new Vector3(0, 0.35f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoVec, AttackRange);
    }

}
