using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class RankUpMonster : Monster
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
        // RankUpMonsterDataInfoData data = Managers.Data.RankUpMonsterChart[dataTemplateID];
        // Atk = data.Atk;
        // MaxHp = data.MaxHp;
        // Hp = MaxHp;
        // MoveSpeed = data.MoveSpeed;

        MoveRange = 5;
        IdleWaitTime = 3;
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
        (Managers.UI.SceneUI as UI_GameScene).RefreshBossMonsterHp(this);
    }

    public override void OnDead()
    {
        base.OnDead();
    }

    #endregion
}