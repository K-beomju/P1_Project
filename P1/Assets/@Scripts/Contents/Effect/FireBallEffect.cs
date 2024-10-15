using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireBallEffect : EffectBase
{
    public Vector3 StartPosition { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public Action EndCallback { get; private set; }
    public bool LookAtTarget { get; private set; }

    public override void SetInfo(int templateID, Hero owner, Data.SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y),
            Mathf.Abs(transform.localScale.z)
        ); StartPosition = transform.position;
        List<Monster> monsters = Managers.Object.Monsters.ToList();
        monsters.Shuffle<Monster>();
        TargetPosition = monsters[0].CenterPosition;
        LookAtTarget = true;
        EndCallback = () => Managers.Object.Despawn(this);
        StartCoroutine(CoLaunchProjectile());
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Creature enemy = other.GetComponent<Creature>();
        }
    }

    private IEnumerator CoLaunchProjectile()
    {
        float journeyLength = Vector3.Distance(StartPosition, TargetPosition);
        float totalTime = journeyLength / 10;
        float elapsedTime = 0;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / totalTime;
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, normalizedTime);

            if (LookAtTarget)
                LookAt2D(TargetPosition - transform.position);

            yield return null;
        }

        //transform.position = TargetPosition;
        //LookAt2D(TargetPosition - transform.position); // 타겟을 계속 바라보게 함
        EndCallback?.Invoke();
    }

   	protected void LookAt2D(Vector2 forward)
	{
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
	}

}
