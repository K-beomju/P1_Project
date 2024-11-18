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
        BaseScene currnetScene = Managers.Scene.GetCurrentScene<BaseScene>();

        if (currnetScene is GameScene gameScene)
        {
            StageInfoData stageInfo = Managers.Data.StageChart[gameScene.StageInfo.StageNumber];
            Atk = stageInfo.BossMonsterAtk;
            MaxHp = stageInfo.BossMonsterMaxHp;
            Hp = MaxHp;
        }

        MoveSpeed = 1;
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
