using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;
using static Define;
using static UnityEditor.Progress;

public class Monster : Creature, IDamageable
{
    protected float MoveRange;  // 패트롤 범위
    protected float IdleWaitTime; // Idle 상태에서 대기하는 시간

    private Vector3 _initialPosition;  // 패트롤 시작 위치
    private Vector3 _targetPosition;   // 패트롤 목표 위치
    private bool _isMovingToTarget;    // 목표 지점으로 이동 중인지 여부
    private Coroutine _idleCoroutine;  // Idle 상태에서 대기 시간을 처리할 코루틴
    private bool _isDamaged;           // 공격을 받은 상태인지 여부

    private Coroutine _damageCoroutine; // 플레이어에게 데미지를 입히는 코루틴

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _isDamaged = false;
        ObjectType = EObjectType.Monster;
        gameObject.layer = (int)ELayer.Monster;
        _initialPosition = transform.position;  // 몬스터의 초기 위치 저장

        SetNewPatrolTarget();  // 첫 번째 목표 지점 설정

        return true;
    }

    public override void SetCreatureInfo(int dataTemplateID)
    {
        MonsterInfoData data = Managers.Data.MonsterChart[dataTemplateID];
        Level = Managers.Scene.GetCurrentScene<GameScene>().StageInfo.MonsterLevel;
        Atk = data.Atk + Managers.Data.CreatureUpgradeStatChart[dataTemplateID].IncreaseAtk; //Managers.Data.CreatureUpgradeStatInfoDataDic[dataTemplateID].IncreaseAtk * (Level - 1);
        MaxHp = data.MaxHp + Managers.Data.CreatureUpgradeStatChart[dataTemplateID].IncreaseMaxHp; //Managers.Data.CreatureUpgradeStatInfoDataDic[dataTemplateID].IncreaseMaxHp * (Level - 1);
        Hp = MaxHp;

        MoveSpeed = data.MoveSpeed;
        MoveRange = 5;
        IdleWaitTime = 1;

        Sprite.DOFade(0, 0);
        Sprite.DOFade(1, 1f);

        HpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBarWorldSpace>(gameObject.transform);
        HpBar.transform.localPosition = new Vector3(0.0f, -0.2f, 0.0f); // FIXME: Prefab 위치 추가 하시오.
        HpBar.SetSliderInfo(this);
        HpBar.gameObject.SetActive(false);
    }

    private void SetNewPatrolTarget()
    {
        // 패트롤 범위 내에서 새로운 목표 위치 설정
        _targetPosition = _initialPosition + new Vector3(UnityEngine.Random.Range(-MoveRange, MoveRange), UnityEngine.Random.Range(-MoveRange, MoveRange), 0);
        _isMovingToTarget = true;
    }

    protected override void UpdateIdle()
    {
        if (_idleCoroutine == null && !_isDamaged)  // 공격을 받지 않았을 때만 대기
        {
            // Idle 상태에서 3초 대기 후 다음 패트롤 지점으로 이동
            _idleCoroutine = StartCoroutine(WaitInIdle());
        }
    }

    private IEnumerator WaitInIdle()
    {
        // 3초 대기
        yield return new WaitForSeconds(IdleWaitTime);

        // 대기 후 다음 패트롤 지점 설정 (공격받지 않은 경우에만)
        if (!_isDamaged)
        {
            SetNewPatrolTarget();
            CreatureState = ECreatureState.Move;
        }

        _idleCoroutine = null;  // 코루틴이 종료되었으므로 다시 사용할 수 있도록 null로 설정
    }

    protected override void UpdateMove()
    {
        if (_isDamaged)  // 공격을 받았다면 이동하지 않음
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        if (_isMovingToTarget)
        {
            // 목표 지점까지의 방향 계산
            Vector3 direction = _targetPosition - transform.position;
            float distanceToTarget = direction.magnitude;

            // 목표 지점에 가까워지면 이동 멈추고 Idle 상태로 전환
            if (distanceToTarget < 0.1f || (Vector2.Distance(transform.position, Managers.Object.Hero.transform.position) < 2))
            {
                _isMovingToTarget = false;
                CreatureState = ECreatureState.Idle;
            }
            else
            {
                // 목표 지점을 향해 이동
                Vector3 moveDir = direction.normalized;
                transform.Translate(moveDir * MoveSpeed * Time.deltaTime);
                Sprite.flipX = moveDir.x < 0;  // 좌우 이동 시 스프라이트 방향 전환
            }
        }
    }

    protected override void UpdateDead()
    {
        OnDead();
    }

    public override void OnDamaged(Creature attacker)
    {
        base.OnDamaged(attacker);
        Sprite.flipX = transform.position.x > attacker.transform.position.x;
        _isDamaged = true;
        // 플레이어에게 데미지를 입히는 코루틴 시작
        if (_damageCoroutine == null)
        {
            _damageCoroutine = StartCoroutine(DealDamageToPlayer());
        }

        CreatureState = ECreatureState.Idle;
    }

    private IEnumerator DealDamageToPlayer()
    {
        yield return new WaitForSeconds(1f);
        if (Managers.Object.Hero != null)
        {
            Managers.Object.Hero.OnDamaged(this);
        }
        _damageCoroutine = null;
    }

    public override void OnDead()
    {

        try
        {
            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }
            GameScene gameScene = Managers.Scene.GetCurrentScene<GameScene>();

            Managers.Backend.GameData.UserData.AddExp(gameScene.StageInfo.MonsterExpReward);
            if (ObjectType == EObjectType.Monster)
            {
                UI_GoldIconBase goldIcon = Managers.UI.ShowBaseUI<UI_GoldIconBase>();
                goldIcon.SetGoldIconAtPosition(transform.position, () =>
                {
                    try
                    {
                        Managers.Backend.GameData.UserData.AddAmount(EGoodType.Gold, gameScene.StageInfo.MonsterGoldReward);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"OnDead -> AddAmount ({EGoodType.Gold}, {gameScene.StageInfo.MonsterGoldReward}) 중 에러가 발생하였습니다\n{e}");
                    }
                });
            }
            else if (ObjectType == EObjectType.BossMonster)
            {
                //UI_CurrencyTextWorldSpace currencyText = Managers.UI.MakeWorldSpaceUI<UI_CurrencyTextWorldSpace>();
                //currencyText.SetCurrencyText(gameScene.Data.MonsterGoldReward);
            }
            Managers.Game.OnMonsterDestroyed();
            Managers.Object.Despawn(this);
        }
        catch(Exception e) {
            throw new Exception($"Monster OnDead 중 에러가 발생하였습니다\n{e}");
        }
    }
}
