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

        worldBossTotalDamage = 0;
    }


    #region AI
    protected override void UpdateIdle()
    {
        //base.UpdateIdle();
    }

    protected override void UpdateMove()
    {
        //base.UpdateMove();

    }
    protected override void UpdateDead()
    {
        //base.UpdateDead();
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
