using BackendData.GameData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class SkillManager
{
    public Dictionary<int, SkillInfoData> AllSkillInfos { get; private set; } = new Dictionary<int, SkillInfoData>();

    public void Init()
    {
        AllSkillInfos.Add(100100, new SkillInfoData(100100, EOwningState.Unowned, 0, 0, false));
        AllSkillInfos.Add(100101, new SkillInfoData(100101, EOwningState.Unowned, 0, 0, false));
        AllSkillInfos.Add(100102, new SkillInfoData(100102, EOwningState.Unowned, 0, 0, false));
        AllSkillInfos.Add(100103, new SkillInfoData(100103, EOwningState.Unowned, 0, 0, false));
        AllSkillInfos.Add(100104, new SkillInfoData(100104, EOwningState.Unowned, 0, 0, false));

        if(Managers.Backend.GameData.SkillInventory.SkillSlotList[0].SlotType != ESkillSlotType.Lock)
        {
            Managers.Backend.GameData.SkillInventory.AddSkill(100100);
            Managers.Backend.GameData.SkillInventory.AddSkill(100101);
            Managers.Backend.GameData.SkillInventory.AddSkill(100102);
            Managers.Backend.GameData.SkillInventory.AddSkill(100103);
            Managers.Backend.GameData.SkillInventory.AddSkill(100104);

        }
    }

    public List<SkillInfoData> GetSkillInfos(bool needsSync = false)
    {
        List<SkillInfoData> skillInfos = AllSkillInfos.Values.ToList();

        if(!needsSync)
        return skillInfos;

        for (int i = 0; i < skillInfos.Count; i++)
        {
            if(Managers.Backend.GameData.SkillInventory.SkillInventoryDic.TryGetValue(skillInfos[i].DataTemplateID, out var skillInfoData))
            {
                skillInfos[i] = skillInfoData;
            }
        }
        return skillInfos;
    }


    
}
