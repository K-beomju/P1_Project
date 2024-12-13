using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class RankMonster : Monster
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.RankUpMonster;
        return true;
    }

    public override void SetCreatureInfo(int dataTemplateID)
    {
        RankUpMonsterInfoData data = Managers.Data.RankUpMonsterChart[dataTemplateID];
        Atk = data.Atk;
        MaxHp = data.MaxHp;
        Hp = MaxHp;
        AttackRange = data.AttackRange;

        Sprite.DOFade(0, 0);
        Sprite.DOFade(1, 1f).SetDelay(2);

        MoveSpeed = 1.5f;
        patrolRange = 5;
        idleWaitTime = 2;

        Target = Managers.Object.Hero;
        CancelWait();
    }

    protected override void UpdateAnimation()
    {
        Anim.SetBool(AnimName.HashMove, CreatureState == ECreatureState.Move);
    }

    public void OnAnimEventHandler()
    {
        if (Target.IsValid() == false || CreatureState == ECreatureState.Dead)
            return;
        
        Target.GetComponent<IDamageable>().OnDamaged(this);
    }


    #region AI
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    protected override void UpdateMove()
    {
        if (!isActionEnabled)
            return;

        Vector3 dir = Target.CenterPosition - CenterPosition;
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = AttackRange * AttackRange;

        if (distToTargetSqr <= attackDistanceSqr)
        {
            CreatureState = ECreatureState.Attack;
            return;
        }
        else
        {
            if (Vector2.Distance(transform.position, Target.transform.position) < 5f)
            {
                isPatrolling = false;

                LookAt(dir);
                float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
                transform.Translate(dir.normalized * moveDist);
            }
            else
            {
                // 추적 범위를 벗어나면 패트롤 실행
                if (!isPatrolling)
                {
                    SetNewPatrolTarget();
                    isPatrolling = true;
                }

                Vector3 patrolDir = patrolTargetPosition - transform.position;
                float patrolDistance = patrolDir.magnitude;

                if (patrolDistance < 0.1f)
                {
                    // 목표 지점에 도달하면 Idle 상태로 전환
                    CreatureState = ECreatureState.Idle;
                }
                else
                {
                    // 목표 지점을 향해 이동
                    transform.Translate(patrolDir.normalized * MoveSpeed * Time.deltaTime);
                    LookAt(patrolDir);
                }
            }
        }


    }

    protected override void UpdateAttack()
    {
        if (!isActionEnabled)
            return;

        Vector3 dir = Target.transform.position - transform.position;
        float distToTargetSqr = dir.sqrMagnitude;

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distToTargetSqr > 3)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        LookAt(dir);

        if (_coWait == null)
        {
            StartWait(2);
            Anim.SetTrigger(AnimName.HashAttack);

        }
    }

    #endregion

    #region Battle

    public override void OnDamaged(Creature attacker, EffectBase effect = null)
    {
        base.OnDamaged(attacker, effect);
        (Managers.UI.SceneUI as UI_GameScene).RefreshBossMonsterHp(this);
    }

    public override void OnDead()
    {
        Anim.SetTrigger(AnimName.HashDead);
        //Managers.Resource.Destroy(this.gameObject);

    }

    #endregion
}
