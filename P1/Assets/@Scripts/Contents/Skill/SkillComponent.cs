using BackendData.GameData;
using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    public List<SkillBase> SkillList = new List<SkillBase>();// { get; private set; } = new List<SkillBase>();
    public Hero Owner;

    public void SetInfo(Hero owner, IReadOnlyList<SkillSlot> skillSlotList)
    {
        Owner = owner;
        foreach (var skillSlot in skillSlotList)
        {
            if (skillSlot.SkillInfoData != null)
            {
                SkillBase skill = gameObject.AddComponent(Type.GetType(skillSlot.SkillInfoData.Data.ClassName)) as SkillBase;
                if (skill == null)
                    return;
                Debug.LogWarning(skill.name);
                skill.SetInfo(Owner, skillSlot.SkillInfoData.Data);
                SkillList.Add(skill);
            }
        }
    }

    public void UseSkill(int index)
    {
        if (index >= 0 && index < SkillList.Count)
        {
            SkillList[index].DoSkill();
        }
    }

}
