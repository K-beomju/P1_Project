using BackEnd;
using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class WorldBoss : Monster
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.BossMonster;
    
        return true;
    }

    public override void SetCreatureInfo(int dataTemplateID)
    {
        WorldBossDungeonInfoData data = Managers.Data.WorldBossDungeonChart[1];
        Atk = data.BossAtk;
        MaxHp = data.BossMaxHp;
        Hp = MaxHp;

        AttackRange = 3f;
        worldBossTotalDamage = 0;
        Target = Managers.Object.Hero;
    }


    #region AI
    protected override void UpdateIdle()
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
    }

    public void OnAnimEventHandler()
    {
        if (Target.IsValid() == false || CreatureState == ECreatureState.Dead)
            return;
        
        Managers.Object.SpawnGameObject(Target.transform.position + new Vector3(0, -1, 0), "Object/Effect/Explosion/WorldBossAttackEffect");
        Target.GetComponent<IDamageable>().OnDamaged(this);
    }


    protected override void UpdateAttack()
    {
        if (!isActionEnabled)
            return;

        if (_coWait == null)
        {
            StartWait(2);
            if (Target.IsValid() == false || CreatureState == ECreatureState.Dead)
                return;

            Anim.SetTrigger(AnimName.HashAttack);
        }
    }


    #endregion

    #region Battle
    public override void OnDamaged(Creature attacker, EffectBase effect = null)
    {
        base.OnDamaged(attacker, effect);
        Sprite.flipX = transform.position.x > attacker.transform.position.x;

        (Managers.UI.SceneUI as UI_DungeonScene).RefreshWorldBossDamage(worldBossTotalDamage);
    }

    public override void OnDead()
    {
        base.OnDead();
    }
    #endregion
}
