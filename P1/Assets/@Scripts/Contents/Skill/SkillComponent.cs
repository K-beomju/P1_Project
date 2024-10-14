using BackendData.GameData;
using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    public Dictionary<int, SkillBase> SkillDic = new Dictionary<int, SkillBase>();
    public Hero Owner;

    public void SetInfo(Hero owner)
    {
        Owner = owner;

        // 스킬 슬롯 초기화 (6개 슬롯 고정)
        for (int i = 0; i < Managers.Backend.GameData.SkillInventory.SkillSlotList.Count; i++)
        {
            SkillDic[i] = null;
            AddSkill(i);
        }
    }

    public void AddSkill(int index)
    {
        SkillSlot slot = Managers.Backend.GameData.SkillInventory.SkillSlotList[index];
        if (slot.SkillInfoData == null)
            return;

        SkillBase skill = gameObject.AddComponent(Type.GetType(slot.SkillInfoData.Data.ClassName)) as SkillBase;
        if (skill == null)
        {
            Debug.LogWarning("스킬 생성 실패");
            return;
        }
        skill.SetInfo(Owner, slot.SkillInfoData.Data);
        SkillDic[index] = skill;

    }

    public void RemoveSkill(int index)
    {
        SkillBase skill = SkillDic[index];
        if (skill == null)
            return;

        Destroy(skill);
        SkillDic[index] = null;
    }

    public void UseSkill(int index)
    {
        if (index >= 0 && index < SkillDic.Count)
        {
            SkillDic[index].DoSkill();
        }
    }

}
