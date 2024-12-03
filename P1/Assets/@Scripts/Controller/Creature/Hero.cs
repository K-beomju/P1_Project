using System;
using System.Collections;
using System.Collections.Generic;
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
    public SkillComponent Skills { get; private set; }

    private Coroutine comboDelayCoroutine = null;
    private Coroutine recoveryCoroutine = null;
    private HeroGhost ghost;

    private RuntimeAnimatorController  handController;
    private RuntimeAnimatorController  weaponController;
    private WaitForSeconds recoveryTime = new WaitForSeconds(1);

    public Action OnAttackAction { get; set; } = null;

    private bool isDash = false;
    private bool isMove = false;
    public bool isDashEnabled = true;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ghost = GetComponent<HeroGhost>();

        ObjectType = EObjectType.Hero;
        gameObject.layer = (int)ELayer.Hero;
        Sprite.sortingOrder = SortingLayers.HERO;

        handController = Managers.Resource.Load<RuntimeAnimatorController>("Animations/Hero_Hand");
        weaponController = Managers.Resource.Load<RuntimeAnimatorController>("Animations/Hero_Weapon");
        Anim.SetBool(AnimName.HashCombo, true);

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

        // Stat
        Atk = HeroInfo.Atk;
        MaxHp = HeroInfo.MaxHp;
        Hp = MaxHp;
        Recovery = HeroInfo.Recovery;
        CriRate = HeroInfo.CriRate;
        CriDmg = HeroInfo.CriDmg;

        AttackDelay = HeroInfo.AttackDelay;
        AttackRange = HeroInfo.AttackRange;
        MoveSpeed = 3;

        // Buff
        ReduceDmgBuff = new CreatureStat(1);

        Skills = gameObject.AddComponent<SkillComponent>();
        Skills.SetInfo(this);

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
    }

    #region Anim
    protected override void UpdateAnimation()
    {
        if (CreatureState == ECreatureState.Dead)
            return;

        Anim.SetBool(AnimName.HashAttack, CreatureState == ECreatureState.Attack && Target.CreatureState != ECreatureState.Dead);
        Anim.SetBool(AnimName.HashMove, CreatureState == ECreatureState.Move && Target != null);
    }

    public void ComboAttackDelay()
    {
        if (comboDelayCoroutine != null)
            StopCoroutine(comboDelayCoroutine);
        comboDelayCoroutine = StartCoroutine(ComboDelayCo());
    }

    private IEnumerator ComboDelayCo()
    {
        Anim.SetBool(AnimName.HashCombo, false);
        yield return new WaitForSeconds(AttackDelay);
        Anim.SetBool(AnimName.HashCombo, true);
        comboDelayCoroutine = null;
    }

    public void OnAnimEventHandler()
    {
        if (Target.IsValid() == false || CreatureState == ECreatureState.Dead)
            return;

        if (OnAttackAction != null)
        {
            OnAttackAction.Invoke();
        }
        else
        {
            Managers.Object.SpawnGameObject(Target.CenterPosition, "Object/Effect/Explosion/HeroAttackEffect");

            Target.GetComponent<IDamageable>().OnDamaged(this);
        }
        CreatureState = ECreatureState.Move;
    }

    #endregion


    #region AI Update
    protected override void UpdateIdle()
    {
        if (!isActionEnabled)
            return;

        if (!isMove)
            return;

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
        if (!isActionEnabled)
            return;

        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            Creature target = FindClosestTarget(Managers.Object.Monsters);
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
        if (!isActionEnabled)
            return;

        if (Target == null || !Target.IsValid())
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
    }
    #endregion

    #region Target Search & Movement
    private Creature FindClosestTarget(IEnumerable<Creature> objs)
    {
        if (Managers.Object.BossMonster != null)
            return Managers.Object.BossMonster;

        if (Managers.Object.Bot != null)
            return Managers.Object.Bot;

        if (Managers.Object.RankMonster != null)
            return Managers.Object.RankMonster;


        Creature target = null;
        float bestDistanceSqr = float.MaxValue; // 매우 큰 값으로 초기화하여 첫 번째 비교가 무조건 이루어지게 함

        foreach (Creature obj in objs)
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
        if (!isActionEnabled || !isMove)
            return;

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
            if (!isDash && isDashEnabled && distToTargetSqr > DASH_DISTANCE_THRESHOLD)
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

                if(dir.magnitude < 3)
                {
                    Target.CreatureState = ECreatureState.Idle;
                    
                }

                float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
                transform.Translate(dir.normalized * moveDist);
            }
        }
    }

    public void ChangeAnimController(bool weapon = false)
    {
        Anim.runtimeAnimatorController = weapon ? weaponController : handController;
        Anim.Rebind();
        CreatureState = ECreatureState.Idle;
    }
    #endregion

    public override void OnDamaged(Creature attacker, EffectBase effect = null)
    {
        base.OnDamaged(attacker, effect);

        if (recoveryCoroutine != null)
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
        Anim.SetTrigger(AnimName.HashDead);

        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }

        //TODO
        BaseScene currnetScene = Managers.Scene.GetCurrentScene<BaseScene>();

        if (currnetScene is GameScene gameScene)
        {
            gameScene.HandleBattleFailure();
        }
    }

    public void Rebirth()
    {
        EnableAction();
        transform.position = Vector2.zero;
        Skills.Clear();
        Anim.Rebind();
        Hp = MaxHp;
        CreatureState = ECreatureState.Idle;
    }

    private IEnumerator RecoveryCo()
    {
        while (true)
        {
            yield return recoveryTime;
            Hp += Recovery;
            if (Hp >= MaxHp)
            {
                Hp = MaxHp;
                yield break;
            }
        }
    }

    public void GetUp()
    {
        isMove = true;
    }


    // 포지션 이동때 그림자 생성 방지
    public void ForceMove(Vector3 newPosition)
    {
        // 기존 Ghost 생성 상태를 저장
        bool previousMakeGhostState = ghost.makeGhost;

        // Ghost 생성 비활성화
        ghost.makeGhost = false;

        // 위치 강제 이동
        transform.position = newPosition;

        // Ghost 생성 상태 복원
        ghost.makeGhost = previousMakeGhostState;
    }

    void OnDrawGizmos()
    {
        Vector3 gizmoVec = transform.position + new Vector3(0, 0.35f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoVec, AttackRange);
    }

}
