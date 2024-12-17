using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Pet : BaseObject
{
    public Hero Owner;
    private Coroutine coroutine = null;

    // 기본 이동 속도
    [SerializeField]
    private float baseSpeed = 2f;
    // 영웅과 펫 사이의 거리별 속도 증가량
    [SerializeField]
    private float distanceFactor = 1f;
    // 펫이 따라가는 최소 거리
    [SerializeField]
    private float followDistance = 1f;
    // SmoothDamp 용
    [SerializeField]
    private float smoothTime = 0.3f;
    private Vector2 currentVelocity = Vector2.zero;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Sprite.sortingOrder = SortingLayers.CREATURE;
        Owner = Managers.Object.Hero;
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(FollowMoveToHeroCO());
        return true;
    }

    private IEnumerator FollowMoveToHeroCO()
    {
        if (Owner == null)
            Owner = Managers.Object.Hero;

        while (true)
        {
            if (Owner == null)
                yield return null;

            Vector2 currentPos = transform.position;
            Vector2 targetPos = Owner.transform.position;
            float distance = Vector2.Distance(currentPos, targetPos);

            // followDistance보다 멀리 떨어진 경우에만 이동
            if (distance > followDistance)
            {
                // distance에 비례한 속도 증가
                float adjustedMaxSpeed = baseSpeed + (distance * distanceFactor);

                // SmoothDamp 사용하여 부드러운 이동
                Vector2 newPos = Vector2.SmoothDamp(
                    currentPos,
                    targetPos,
                    ref currentVelocity,
                    smoothTime,
                    adjustedMaxSpeed,
                    Time.deltaTime
                );
                transform.position = newPos;

                Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
                LookAt(direction);
            }
            else
            {
                // 영웅이 충분히 가까우면 제자리에서 부드러운 정지
                currentVelocity = Vector2.zero;
            }

            yield return null;
        }
    }
}
