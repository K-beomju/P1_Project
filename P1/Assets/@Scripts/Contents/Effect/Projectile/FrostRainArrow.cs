using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostRainArrow : ProjectileBase
{
    // 새로운 SetInfo 메서드 (오버로딩)
    public void SetInfo(int templateID, Hero owner, Data.SkillData skillData, Vector3 startPos)
    {
        // base의 SetInfo 호출
        base.SetInfo(templateID, owner, skillData);

        // 새로 받은 위치 값을 설정
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        startPosition = startPos; // 전달받은 시작 위치
        moveSpeed = 10;
    }

    protected override IEnumerator CoLaunchProjectile()
    {
        float screenBottomY = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        Vector3 moveDirection = transform.right;
        while (true)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            if (transform.position.y <= screenBottomY)
            {
                break;
            }
            yield return null;
        }

        base.ClearEffect();
    }



}
