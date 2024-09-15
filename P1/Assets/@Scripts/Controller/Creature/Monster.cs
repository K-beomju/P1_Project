using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;
using static Define;

public class Monster : Creature, IDamageable
{
    protected float MoveRange = 5.0f;  // 패트롤 범위
    protected float IdleWaitTime = 3.0f; // Idle 상태에서 대기하는 시간

    private Vector3 _initialPosition;  // 패트롤 시작 위치
    private Vector3 _targetPosition;   // 패트롤 목표 위치
    private bool _isMovingToTarget;    // 목표 지점으로 이동 중인지 여부
    private Coroutine _idleCoroutine;  // Idle 상태에서 대기 시간을 처리할 코루틴
    private bool _isDamaged;           // 공격을 받은 상태인지 여부

    private UI_HpBarWorldSpace _hpBar;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Monster;
        gameObject.layer = (int)ELayer.Monster;
        _initialPosition = transform.position;  // 몬스터의 초기 위치 저장
        SetNewPatrolTarget();  // 첫 번째 목표 지점 설정

        return true;
    }

    public override void SetCreatureInfo(int dataTemplateID)
    {
        MonsterInfoData data = Managers.Data.MonsterDic[dataTemplateID];
        Level = Managers.Scene.GetCurrentScene<GameScene>().Data.MonsterLevel;

        MaxHp = new CreatureStat(data.MaxHp + Managers.Data.CreatureUpgradeDic[dataTemplateID].IncreaseMaxHp * (Level - 1));
        Hp = MaxHp.Value;
        // Debug.Log($"MaxHp = 원래 체력: {data.MaxHp} + (업그레이드 체력: {Managers.Data.CreatureUpgradeDic[dataTemplateID].IncreaseMaxHp} * 레벨: {Level - 1})");
        // Debug.Log($"MaxHp 계산 결과: {MaxHp.Value}");
        MoveSpeed = new CreatureStat(data.MoveSpeed);

        MoveRange = 5;
        IdleWaitTime = 3;

        _hpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBarWorldSpace>(gameObject.transform);
        _hpBar.transform.localPosition = new Vector3(0.0f, -0.2f, 0.0f); // FIXME: Prefab 위치 추가 하시오.
        _hpBar.SetSliderInfo(this);
        _hpBar.gameObject.SetActive(false);
    }

    private void SetNewPatrolTarget()
    {
        // 패트롤 범위 내에서 새로운 목표 위치 설정
        _targetPosition = _initialPosition + new Vector3(Random.Range(-MoveRange, MoveRange), Random.Range(-MoveRange, MoveRange), 0);
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
            if (distanceToTarget < 0.1f)
            {
                _isMovingToTarget = false;
                CreatureState = ECreatureState.Idle;
            }
            else
            {
                // 목표 지점을 향해 이동
                Vector3 moveDir = direction.normalized;
                transform.Translate(moveDir * MoveSpeed.Value * Time.deltaTime);
                Sprite.flipX = moveDir.x < 0;  // 좌우 이동 시 스프라이트 방향 전환
            }
        }
    }

    protected override void UpdateDead()
    {
        OnDead();
    }

    public virtual void OnDamaged(Creature attacker)
    {
        float finalDamage = attacker.Atk.Value; // TODO: 방어력이나 다른 계산이 있을 경우 적용
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp.Value);
        if(_hpBar != null && !_hpBar.gameObject.activeSelf)
        _hpBar.gameObject.SetActive(true);

        // 공격을 받으면 움직이지 않도록 Idle 상태로 유지
        _isDamaged = true;
        CreatureState = ECreatureState.Idle;

        if (Hp <= 0)
        {
            OnDead();
        }

        // DmageText
        UI_DamageTextWorldSpace damageText = Managers.UI.MakeWorldSpaceUI<UI_DamageTextWorldSpace>();
        damageText.SetInfo(CenterPosition, finalDamage, false);

        Color originalColor = Sprite.color;
        
        Sprite.DOColor(Color.red, 0.05f)
            .OnComplete(() => Sprite.DOColor(originalColor, 0.05f));
    }

    public virtual void OnDead()
    {
        GameScene gameScene = Managers.Scene.GetCurrentScene<GameScene>();

        if (ObjectType == EObjectType.Monster)
        {
            UI_GoldIconBase goldIcon = Managers.UI.ShowBaseUI<UI_GoldIconBase>();
            goldIcon.SetGoldIconAtPosition(transform.position, () => Managers.Game.AddAmount(EGoodType.Gold, gameScene.Data.MonsterGoldReward));
        }
        else if (ObjectType == EObjectType.BossMonster)
        {
            //UI_CurrencyTextWorldSpace currencyText = Managers.UI.MakeWorldSpaceUI<UI_CurrencyTextWorldSpace>();
            //currencyText.SetCurrencyText(gameScene.Data.MonsterGoldReward);
        }
        Managers.Object.Despawn(this);

    }
}
