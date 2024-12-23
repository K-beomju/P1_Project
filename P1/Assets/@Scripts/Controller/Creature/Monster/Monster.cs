using Data;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using static Define;

public class Monster : Creature, IDamageable
{
    private Coroutine idleCoroutine; // Idle 상태에서 대기 시간을 처리할 코루틴
    private Vector3 initialPosition; // 패트롤 시작 위치
    protected Vector3 patrolTargetPosition; // 패트롤 목표 위치

    protected bool isPatrolling; // 패트롤 중인지 여부
    protected float idleWaitTime = 1f; // Idle 상태에서 대기 시간
    protected float patrolRange = 5f; // 패트롤 범위
    private float traceRange = 3f; // 추적 범위
    private float attackRange = 0.5f; // 공격 범위

    protected override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        ObjectType = EObjectType.Monster;
        gameObject.layer = (int)ELayer.Monster;
        return true;
    }


    private void OnDestroy()
    {
        DOTween.Kill(Sprite);
    }

    public override void SetCreatureInfo(int dataTemplateID)
    {
        BaseScene currnetScene = Managers.Scene.GetCurrentScene<BaseScene>();

        if (currnetScene is GameScene gameScene)
        {
            StageInfoData stageInfo = Managers.Data.StageChart[gameScene.StageInfo.StageNumber];
            Atk = stageInfo.MonsterAtk;
            MaxHp = stageInfo.MonsterMaxHp;
            Hp = MaxHp;
            AttackRange = 0.5f;
        }
        else if (currnetScene is DungeonScene dungeonScene)
        {
            var dungeonType = Managers.Game.GetCurrentDungeon();
            switch (dungeonType)
            {
                case EDungeonType.Gold:
                    GoldDungeonInfoData goldDungeonInfo = Managers.Data.GoldDungeonChart[dungeonScene.DungeonInfo.DungeonLevel];
                    Atk = goldDungeonInfo.MonsterAtk;
                    MaxHp = goldDungeonInfo.MonsterMaxHp;
                    Hp = MaxHp;
                    break;
                case EDungeonType.Dia:
                    DiaDungeonInfoData diaDungeonInfoData = Managers.Data.DiaDungeonChart[dungeonScene.DungeonInfo.DungeonLevel];
                    Atk = diaDungeonInfoData.MonsterAtk;
                    MaxHp = diaDungeonInfoData.MonsterMaxHp;
                    Hp = MaxHp;
                    break;

            }
        }


        MoveSpeed = 1f;
        initialPosition = transform.position; // 초기 위치 저장
        Target = Managers.Object.Hero;

        SetNewPatrolTarget(); // 첫 번째 목표 지점 설정
        Sprite.DOFade(0, 0).OnComplete(() => Sprite.DOFade(1, 1f));


        if (HpBar == null)
        {
            HpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBarWorldSpace>(Util.FindChild<Transform>(gameObject, "HpBarPos"));
            HpBar.transform.localPosition = Vector2.zero;
            HpBar.SetSliderInfo(this);
            HpBar.gameObject.SetActive(false);
        }
    }

    protected void SetNewPatrolTarget()
    {
        // 타일맵이 null이 아닌지 확인
        if (Managers.Game.TileMap == null)
        {
            Debug.LogError("TileMap is not set!");
            return;
        }

        // 맵 안에서 이동 가능한 랜덤 위치 설정
        patrolTargetPosition = Util.GetRandomPositionWithinMap(Managers.Game.TileMap, initialPosition, patrolRange, 2);
        isPatrolling = true;
    }

    #region AI
    protected override void UpdateIdle()
    {
        if (!isActionEnabled || idleCoroutine != null)
            return;

        idleCoroutine = StartCoroutine(IdleRoutine());
    }

    private IEnumerator IdleRoutine()
    {
        yield return new WaitForSeconds(idleWaitTime);
        SetNewPatrolTarget();
        CreatureState = ECreatureState.Move;
        idleCoroutine = null;
    }

    protected override void UpdateMove()
    {
        if (!isActionEnabled)
            return;

        Vector3 directionToTarget = Target.CenterPosition - CenterPosition;
        float distanceToTargetSqr = directionToTarget.sqrMagnitude;

        if (IsInAttackRange(distanceToTargetSqr))
        {
            TransitionToState(ECreatureState.Attack);
        }
        else if (IsInTraceRange(distanceToTargetSqr))
        {
            TraceTarget(directionToTarget);
        }
        else if (isPatrolling)
        {
            Patrol();
        }
        else
        {
            TransitionToState(ECreatureState.Idle);
        }
    }

    protected override void UpdateAttack()
    {
        if (!isActionEnabled)
            return;

        Vector3 directionToTarget = Target.CenterPosition - CenterPosition;
        float distanceToTargetSqr = directionToTarget.sqrMagnitude;

        if (!IsInAttackRange(distanceToTargetSqr))
        {
            TransitionToState(ECreatureState.Move);
            return;
        }

        LookAt(directionToTarget);

        if (_coWait == null)
        {
            StartWait(2f); // 공격 대기 시간
            if (Target.IsValid() && CreatureState != ECreatureState.Dead)
                Target.GetComponent<IDamageable>().OnDamaged(this);
        }
    }


    #endregion

    #region State Transitions

    private bool IsInAttackRange(float distanceToTargetSqr)
    {
        return distanceToTargetSqr <= attackRange * attackRange;
    }

    private bool IsInTraceRange(float distanceToTargetSqr)
    {
        return distanceToTargetSqr <= traceRange * traceRange;
    }

    private void TraceTarget(Vector3 directionToTarget)
    {
        isPatrolling = false;
        LookAt(directionToTarget);
        float moveDistance = Mathf.Min(directionToTarget.magnitude, MoveSpeed * Time.deltaTime);
        transform.Translate(directionToTarget.normalized * moveDistance);
        Anim.SetBool(AnimName.HashMove, true);
    }

    private void Patrol()
    {
        Vector3 directionToPatrolTarget = patrolTargetPosition - transform.position;
        float distanceToPatrolTarget = directionToPatrolTarget.magnitude;

        if (distanceToPatrolTarget < 0.1f)
        {
            isPatrolling = false;
            TransitionToState(ECreatureState.Idle);
            Anim.SetBool(AnimName.HashMove, false);
        }
        else
        {
            LookAt(directionToPatrolTarget);
            float moveDistance = Mathf.Min(distanceToPatrolTarget, MoveSpeed * Time.deltaTime);
            transform.Translate(directionToPatrolTarget.normalized * moveDistance);
            Anim.SetBool(AnimName.HashMove, true);
        }
    }

    private void TransitionToState(ECreatureState newState)
    {
        CreatureState = newState;
        if (newState == ECreatureState.Idle)
        {
            Anim.SetBool(AnimName.HashMove, false);
        }
    }

    #endregion

    #region  Battle


    public override void OnDamaged(Creature attacker, EffectBase effect = null)
    {
        base.OnDamaged(attacker, effect);

        if (this is RankMonster || this is BossMonster)
            return;

        Vector2 direction = attacker.transform.position - transform.position;
        LookAt(direction);
    }

    public override void OnDead()
    {
        try
        {
            // 공통 로직: 코루틴 중지 및 기본 처리
            if (idleCoroutine != null)
            {
                StopCoroutine(idleCoroutine);
                idleCoroutine = null;
            }

            switch (Managers.Scene.GetCurrentScene<BaseScene>())
            {
                case GameScene gameScene:
                    HandleGameSceneDeath(gameScene);
                    break;

                case DungeonScene dungeonScene:
                    HandleDungeonSceneDeath(dungeonScene);
                    break;
            }

            // 추가 처리는 자식 클래스에서 오버라이드 가능
            OnAfterDead();
        }
        catch (Exception e)
        {
            throw new Exception($"Monster OnDead 중 에러가 발생하였습니다\n{e}");
        }
    }

    // 공통 처리: 게임 씬에서 몬스터가 죽었을 때
    private void HandleGameSceneDeath(GameScene gameScene)
    {
        Managers.Backend.GameData.CharacterData.AddExp(gameScene.StageInfo.MonsterExpReward);

        if (ObjectType == EObjectType.Monster)
        {
            UI_ItemIconBase itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
            itemIcon.SetItemIconAtPosition(EItemType.Gold, transform.position, () =>
            {
                try
                {
                    Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Gold,
                        gameScene.StageInfo.MonsterGoldReward);
                }
                catch (Exception e)
                {
                    throw new Exception(
                        $"OnDead -> AddAmount ({EItemType.Gold}, {gameScene.StageInfo.MonsterGoldReward}) 중 에러가 발생하였습니다\n{e}");
                }
            });

            // 펫 조각
            PetData petData = Util.GetPetCraftData(gameScene.ChapterLevel);
            if (petData != null)
            {
                bool isDropped = UnityEngine.Random.Range(0f, 1f) <= petData.DropCraftItemRate;

                if (isDropped)
                {
                    Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification($"{petData.PetName} 조각 획득!");

                    itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
                    itemIcon.SetPetCraftItemAtPosition(petData.PetType, transform.position);

                    // 펫 조각 지급
                    Managers.Backend.GameData.PetInventory.AddPetCraft(petData.PetType, 1);
                }
            }



            Managers.Game.OnMonsterDestroyed();
            Managers.Backend.GameData.QuestData.UpdateQuest(EQuestType.KillMonster);
            Managers.Backend.GameData.MissionData.UpdateMission(EMissionType.EnemyDefeat);

            // 폭발 효과 생성
            HpBar.gameObject.SetActive(false);
            Managers.Object.SpawnGameObject(CenterPosition, "Object/Effect/Explosion/DeadEffect");
            Managers.Object.Despawn(this);
        }
    }

    // 공통 처리: 던전 씬에서 몬스터가 죽었을 때
    private void HandleDungeonSceneDeath(DungeonScene dungeonScene)
    {
        // 던전 씬에 대한 추가 처리
        Managers.Game.OnMonsterDestroyed();

        // 폭발 효과 생성
        HpBar.gameObject.SetActive(false);
        Managers.Object.SpawnGameObject(CenterPosition, "Object/Effect/Explosion/DeadEffect");
        Managers.Object.Despawn(this);

    }

    // 추가 작업을 위한 Hook 메서드
    protected virtual void OnAfterDead()
    {
        // 자식 클래스에서 필요한 경우 오버라이드
    }

    #endregion

}