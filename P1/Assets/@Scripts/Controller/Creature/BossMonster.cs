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

    public override void SetCreatureInfo(int dataTemplateID)
    {
        Level = Managers.Scene.GetCurrentScene<GameScene>().Data.MonsterLevel;
        
        MaxHp = new CreatureStat(5);
        Hp = MaxHp.Value;
        MoveSpeed = new CreatureStat(2);

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
