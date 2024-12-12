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
            AttackRange = 1f;
            MoveSpeed = 1.5f;

            Target = Managers.Object.Hero;
        }
    }


    #region AI
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    protected override void UpdateMove()
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
        else
        {
            LookAt(dir);
            float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
            transform.Translate(dir.normalized * moveDist);
        }


    }

    protected override void UpdateAttack()
    {
        if (!isActionEnabled)
            return;

        Vector3 dir = Target.transform.position - transform.position;
        float distToTargetSqr = dir.sqrMagnitude;

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distToTargetSqr > 3)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        LookAt(dir);

        if (_coWait == null)
        {
            StartWait(2);
            if (Target.IsValid() == false || CreatureState == ECreatureState.Dead)
                return;

            Target.GetComponent<IDamageable>().OnDamaged(this);
        }
    }

    #endregion

    #region Battle
    public override void OnDamaged(Creature attacker, EffectBase effect = null)
    {
        base.OnDamaged(attacker, effect);
        (Managers.UI.SceneUI as UI_GameScene).RefreshBossMonsterHp(this);
    }

    public override void OnDead()
    {
        // Clear 보상 재화 연출 
        for (int i = 0; i < 10; i++)
        {
            UI_ItemIconBase itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
            itemIcon.SetItemIconAtPosition(EItemType.Dia, transform.position);
        }

        for (int i = 0; i < 10; i++)
        {
            UI_ItemIconBase itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
            itemIcon.SetItemIconAtPosition(EItemType.Gold, transform.position);
        }

        // Clear 보상 지급 
        Dictionary<EItemType, int> rewardItem = Managers.Scene.GetCurrentScene<GameScene>().StageInfo.RewardItem;

        Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Gold, rewardItem[EItemType.Gold]);
        Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, rewardItem[EItemType.Dia]);

        Managers.Object.SpawnGameObject(CenterPosition, "Object/Effect/Explosion/DeadEffect");

        Managers.Object.Despawn(this);
    }
    #endregion
}
