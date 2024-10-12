using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWaveSkill : SkillBase
{
    protected override void ApplySkillEffect()
    {
        GameObject effectObj = Managers.Object.SpawnGameObject(Owner.CenterPosition, SkillData.PrefabKey);
        RedWaveEffect effect = effectObj.GetComponent<RedWaveEffect>();
        effect.SetInfo(Owner, SkillData); // Owner와 SkillData를 전달
    }
}
