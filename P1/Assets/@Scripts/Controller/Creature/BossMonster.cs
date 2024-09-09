using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BossMonster : Monster
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.BossMonster;
    
        return true;
    }

    protected override void SetCreatureInfo()
    {
        MaxHp = Hp;

        MoveRange = 5;
        MoveSpeed = 2;
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
    public override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);
        (Managers.UI.SceneUI as UI_GameScene).RefreshBossMonsterHp(this);
    }

    public override void OnDead()
    {
        base.OnDead();
    }
    #endregion
}
