using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature, IDamageable
{
    [SerializeField] private float patrolRange = 5.0f;  // 패트롤 범위
    [SerializeField] private float patrolSpeed = 2.0f;  // 패트롤 속도
    [SerializeField] private float idleWaitTime = 3.0f; // Idle 상태에서 대기하는 시간

    private Vector3 _initialPosition;  // 패트롤 시작 위치
    private Vector3 _targetPosition;   // 패트롤 목표 위치
    private bool _isMovingToTarget;    // 목표 지점으로 이동 중인지 여부
    private Coroutine _idleCoroutine;  // Idle 상태에서 대기 시간을 처리할 코루틴
    private bool _isDamaged;           // 공격을 받은 상태인지 여부
    private bool _isFirstSpawn = true; // 처음 소환된 상태인지 여부

    private UI_HpBarWorldSpace _hpBar;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        gameObject.layer = (int)ELayer.Monster;
        _initialPosition = transform.position;  // 몬스터의 초기 위치 저장
        SetNewPatrolTarget();  // 첫 번째 목표 지점 설정

        return true;
    }

    protected override void SetCreatureInfo()
    {
        MaxHp = Hp;

        _hpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBarWorldSpace>(gameObject.transform);
        _hpBar.transform.localPosition = new Vector3(0.0f, -0.9f, 0.0f); // FIXME: Prefab 위치 추가 하시오.
        _hpBar.SetSliderInfo(this);
    }

    private void SetNewPatrolTarget()
    {
        // 패트롤 범위 내에서 새로운 목표 위치 설정
        _targetPosition = _initialPosition + new Vector3(Random.Range(-patrolRange, patrolRange), Random.Range(-patrolRange, patrolRange), 0);
        _isMovingToTarget = true;
    }

    protected override void UpdateIdle()
    {
        // 처음 소환되었을 때는 바로 이동 시작
        if (_isFirstSpawn)
        {
            _isFirstSpawn = false;
            SetNewPatrolTarget();
            CreatureState = ECreatureState.Move;
            return;
        }

        if (_idleCoroutine == null && !_isDamaged)  // 공격을 받지 않았을 때만 대기
        {
            // Idle 상태에서 3초 대기 후 다음 패트롤 지점으로 이동
            _idleCoroutine = StartCoroutine(WaitInIdle());
        }
    }

    private IEnumerator WaitInIdle()
    {
        // 3초 대기
        yield return new WaitForSeconds(idleWaitTime);

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
                transform.Translate(moveDir * patrolSpeed * Time.deltaTime);
                _sprite.flipX = moveDir.x < 0;  // 좌우 이동 시 스프라이트 방향 전환
            }
        }
    }

    protected override void UpdateDead()
    {
        OnDead();
    }

    public void OnDamage(float damage)
    {
        float finalDamage = damage; // TODO: 방어력이나 다른 계산이 있을 경우 적용
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

        // 공격을 받으면 움직이지 않도록 Idle 상태로 유지
        _isDamaged = true;
        CreatureState = ECreatureState.Idle;

        if (Hp <= 0)
        {
            OnDead();
        }

         // DmageText
        UI_DamageTextWorldSpace damageText = Managers.UI.MakeWorldSpaceUI<UI_DamageTextWorldSpace>();
        damageText.SetInfo(CenterPosition, damage, false);
    }

    public void OnDead()
    {
        Managers.Object.Despawn(this);
    }
}
