using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveGrenade : EffectBase
{
    public override void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);
        StartCoroutine(SpawnExplosionCo());
    }

    private IEnumerator SpawnExplosionCo()
    {
        yield return new WaitForSeconds(1);
        Debug.Log(EffectData.ExplosionKey);
        GameObject go = Managers.Object.SpawnGameObject(transform.position, EffectData.ExplosionKey);
        go.GetComponent<ExplosiveGrenadeEx>().SetInfo(DataTemplateID, Owner, SkillData);
        ClearEffect();
    }

}
