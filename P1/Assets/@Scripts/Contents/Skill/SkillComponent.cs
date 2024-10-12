using BackendData.GameData;
using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    public List<SkillBase> SkillList = new List<SkillBase>();// { get; private set; } = new List<SkillBase>();

    public void SetInfo(Hero owner, IReadOnlyList<SkillSlot> skillSlotList)
    {
        foreach (var skillSlot in skillSlotList)
        {
            if (skillSlot.SkillInfoData != null)
            {
                SkillBase skill = gameObject.AddComponent(Type.GetType(skillSlot.SkillInfoData.Data.ClassName)) as SkillBase;
                if (skill == null)
                    return;
                skill.SetInfo(owner, skillSlot.SkillInfoData.Data);
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
