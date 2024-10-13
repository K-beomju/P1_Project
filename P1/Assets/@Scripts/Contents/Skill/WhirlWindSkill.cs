using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlWindSkill : SkillBase
{
    protected override void ApplySkillEffect()
    {
        GameObject effectObj = Managers.Object.SpawnGameObject(Owner.CenterPosition, SkillData.PrefabKey);
        effectObj.transform.SetParent(Owner.transform, false);
        effectObj.transform.localPosition = Vector3.zero;
        WhirlWindEffect effect = effectObj.GetComponent<WhirlWindEffect>();
        effect.SetInfo(SkillData.EffectId, Owner, SkillData);
    }
}
