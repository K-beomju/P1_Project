using BackEnd;
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
        BossMonsterInfoData data = Managers.Data.BossMonsterChart[dataTemplateID]; //Managers.Data.BossMonsterDataDic[dataTemplateID];
        Level = Managers.Scene.GetCurrentScene<GameScene>().StageInfo.MonsterLevel;

        Atk = data.Atk + Managers.Data.CreatureUpgradeStatChart[dataTemplateID].IncreaseAtk; //Managers.Data.CreatureUpgradeStatInfoDataDic[dataTemplateID].IncreaseAtk * (Level - 1);
        MaxHp = data.MaxHp + Managers.Data.CreatureUpgradeStatChart[dataTemplateID].IncreaseMaxHp * (Level - 1);
        Hp = MaxHp;
        MoveSpeed = data.MoveSpeed;
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
