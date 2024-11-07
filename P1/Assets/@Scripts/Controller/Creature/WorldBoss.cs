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
        BossMonsterInfoData data = Managers.Data.BossMonsterChart[dataTemplateID];
        Atk = data.Atk;
        MaxHp = data.MaxHp;
        Hp = MaxHp;

        MoveSpeed = data.MoveSpeed;
        MoveRange = 5;
        IdleWaitTime = 3;

        worldBossTotalDamage = 0;

    }


    #region AI
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    protected override void UpdateMove()
    {
        base.UpdateMove();

    }
    protected override void UpdateDead()
    {
        base.UpdateDead();
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
