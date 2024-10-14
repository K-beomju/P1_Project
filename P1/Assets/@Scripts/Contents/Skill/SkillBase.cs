using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : InitBase
{
    public Hero Owner { get; private set; } // 스킬은 히어로만 쓸 수 있음
    public Data.SkillData SkillData { get; private set; } // 스킬 정보 데이터

    public virtual void SetInfo(Hero owner, Data.SkillData skillData)
    {
        Owner = owner;
        SkillData = skillData;
    }

    // 스킬을 발동하는 메서드 
    public virtual void DoSkill()
    {
        
        Debug.LogWarning($"{SkillData.Name} 스킬 발동!");
        ApplySkillEffect();
    }

    // 이펙트, 투사체, 소환수 범용적으로 
    protected virtual void ApplySkillEffect()
    {

    }


}
