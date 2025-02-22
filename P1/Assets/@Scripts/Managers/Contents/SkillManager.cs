using BackendData.GameData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class SkillManager
{
    public Dictionary<int, SkillInfoData> AllSkillInfos { get; private set; } = new Dictionary<int, SkillInfoData>();
    public static bool Initialized { get; set; } = false;

    public void Init()
    {
        if (Initialized == false)
        {
            Initialized = true;

            AllSkillInfos.Add(100100, new SkillInfoData(100100, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100101, new SkillInfoData(100101, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100102, new SkillInfoData(100102, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100103, new SkillInfoData(100103, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100104, new SkillInfoData(100104, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100105, new SkillInfoData(100105, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100106, new SkillInfoData(100106, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100107, new SkillInfoData(100107, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100108, new SkillInfoData(100108, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100109, new SkillInfoData(100109, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100110, new SkillInfoData(100110, EOwningState.Unowned, 0, 0, false));
            AllSkillInfos.Add(100111, new SkillInfoData(100111, EOwningState.Unowned, 0, 0, false));
        }
    }

    public List<SkillInfoData> GetSkillInfos()
    {
        List<SkillInfoData> skillInfos = AllSkillInfos.Values.ToList();

        for (int i = 0; i < skillInfos.Count; i++)
        {
            if (Managers.Backend.GameData.SkillInventory.SkillInventoryDic.TryGetValue(skillInfos[i].DataTemplateID, out var skillInfoData))
            {
                skillInfos[i] = skillInfoData;
            }
        }
        return skillInfos;
    }



}
