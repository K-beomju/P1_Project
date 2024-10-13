using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWaveSkill : SkillBase
{

    protected override void ApplySkillEffect()
    {
        GameObject effectObj = Managers.Object.SpawnGameObject(Owner.CenterPosition, SkillData.PrefabKey);
        Vector3 ownerScale = Owner.transform.localScale;
        effectObj.transform.localScale = new Vector3(ownerScale.x > 0 ? Mathf.Abs(effectObj.transform.localScale.x) : -Mathf.Abs(effectObj.transform.localScale.x),
                                                     effectObj.transform.localScale.y,
                                                     effectObj.transform.localScale.z);

        RedWaveEffect effect = effectObj.GetComponent<RedWaveEffect>();
        effect.SetInfo(Owner, SkillData); // Owner와 SkillData를 전달
    }
}
