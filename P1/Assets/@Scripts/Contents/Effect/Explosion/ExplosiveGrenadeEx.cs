using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveGrenadeEx : ExplosionBase
{
    private float explosiveRadius = 1f;        // 한 번에 당기는 힘
    List<BaseObject> visibleMonsters = new List<BaseObject>();

    public override void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        visibleMonsters.Clear();
        foreach (BaseObject monster in Managers.Object.Monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance <= explosiveRadius)
            {
                visibleMonsters.Add(monster);
            }
        }
    }

    protected override IEnumerator CoStartTimer()
    {
        yield return new WaitUntil(() => Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        foreach (BaseObject monster in visibleMonsters)
        {
            if (monster != null)
            {
                ApplyDamage(monster as Creature);
            }
        }

        base.ClearEffect();
    }
     private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosiveRadius);
    }
}
