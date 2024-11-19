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

        Sprite.DOFade(0, 0);
        Sprite.DOFade(1, 1f);
        
        MoveSpeed = 1;
        MoveRange = 5;
        IdleWaitTime = 1;
    }


    #region AI
    // protected override void UpdateIdle()
    // {
    //     base.UpdateIdle();
    // }

    // protected override void UpdateMove()
    // {
    //    base.UpdateMove();
    // }

    // protected override void UpdateAttack()
    // {
        
    // }

    #endregion

    #region Battle

    public override void OnDamaged(Creature attacker, EffectBase effect = null)
    {
        return;
        // base.OnDamaged(attacker, effect);
        // Sprite.flipX = transform.position.x > attacker.transform.position.x;
        // (Managers.UI.SceneUI as UI_GameScene).RefreshBossMonsterHp(this);
    }

    public override void OnDead()
    {
        base.OnDead();
    }

    #endregion
}
