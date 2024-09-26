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
        BossMonsterInfoData data = Managers.Data.BossMonsterDataDic[dataTemplateID];
        Level = Managers.Scene.GetCurrentScene<GameScene>().Data.MonsterLevel;

        MaxHp = data.MaxHp + Managers.Data.CreatureUpgradeStatInfoDataDic[dataTemplateID].IncreaseMaxHp * (Level - 1);
        Hp = MaxHp;
        // Debug.Log($"MaxHp = 원래 체력: {data.MaxHp} + (업그레이드 체력: {Managers.Data.CreatureUpgradeDic[dataTemplateID].IncreaseMaxHp} * 레벨: {Level - 1})");
        // Debug.Log($"MaxHp 계산 결과: {MaxHp.Value}");
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
    public override void OnDamaged(Creature attacker)
    {
        base.OnDamaged(attacker);
        Sprite.flipX = transform.position.x > attacker.transform.position.x;
        (Managers.UI.SceneUI as UI_GameScene).RefreshBossMonsterHp(this);
    }

    public override void OnDead()
    {
        base.OnDead();
    }
    #endregion
}
