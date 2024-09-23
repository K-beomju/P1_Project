using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using static Define;

public class EquipmentInfo
{
    public int DataTemplateID { get; private set; }
    public EquipmentData Data { get; private set; }
    public EOwningState OwningState { get; set; }
    public bool IsEquipped { get; set; }

    public EquipmentInfo(int dataTemplateID)
    {
        DataTemplateID = dataTemplateID;
        Data = Managers.Data.EquipmentDic[dataTemplateID];
        OwningState = EOwningState.Owned;
    }
}

public class EquipmentManager
{
    public Dictionary<int, EquipmentInfo> AllEquipmentInfos { get; private set; } = new Dictionary<int, EquipmentInfo>();

    public List<EquipmentInfo> OwnedEquipments
    {
        get { return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.OwningState == EOwningState.Owned).ToList(); }
    }

    public List<EquipmentInfo> UnownedEquipments
    {
        get { return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.OwningState == EOwningState.Unowned).ToList(); }
    }

    public List<EquipmentInfo> WeaponEquipments
    {
        get { return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.Data.EquipmentType == EEquipmentType.Weapon).ToList(); }
    }

    public List<EquipmentInfo> ArmorEquipments
    {
        get { return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.Data.EquipmentType == EEquipmentType.Armor).ToList(); }
    }

    public List<EquipmentInfo> RingEquipments
    {
        get { return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.Data.EquipmentType == EEquipmentType.Ring).ToList(); }
    }

    public Dictionary<EEquipmentType, EquipmentInfo> EquppedEquipments = new Dictionary<EEquipmentType, EquipmentInfo>();

    public void Init()
    {
        foreach (var equipmentInfo in AllEquipmentInfos)
        {
            if (equipmentInfo.Value.IsEquipped)
            {
                EquppedEquipments.Add(equipmentInfo.Value.Data.EquipmentType, equipmentInfo.Value);
            }
        }

        // 무기
        int index = 100100;
        for (int i = 0; i < 24; i++)
        {
            EquipmentInfo equipmentInfo = new EquipmentInfo(index);
            AllEquipmentInfos.Add(equipmentInfo.DataTemplateID, equipmentInfo);
            index += 1;
        }
        // 갑옷
        index = 200100;
        for (int i = 0; i < 24; i++)
        {
            EquipmentInfo equipmentInfo = new EquipmentInfo(index);
            AllEquipmentInfos.Add(equipmentInfo.DataTemplateID, equipmentInfo);
            index += 1;
        }
        // 반지
        index = 300100;
        for (int i = 0; i < 24; i++)
        {
            EquipmentInfo equipmentInfo = new EquipmentInfo(index);
            AllEquipmentInfos.Add(equipmentInfo.DataTemplateID, equipmentInfo);
            index += 1;
        }

    }

    public EquipmentInfo GetEquipmentInfo(int dataTemplateID)
    {
        if (AllEquipmentInfos.TryGetValue(dataTemplateID, out EquipmentInfo equipmentInfo))
        {
            return equipmentInfo;
        }
        else
        {
            return null;
        }
    }

    public void AddEquipment(int dataTemplateID)
    {
        AllEquipmentInfos[dataTemplateID].OwningState = EOwningState.Owned;
    }

    public void EquipEquipment(int dataTemplateID)
    {
        if (AllEquipmentInfos.TryGetValue(dataTemplateID, out EquipmentInfo equipEquipmentInfo))
        {
            if (EquppedEquipments.TryGetValue(equipEquipmentInfo.Data.EquipmentType, out EquipmentInfo
            unEquipEquipmentInfo))
            {
                unEquipEquipmentInfo.IsEquipped = false;
                equipEquipmentInfo.IsEquipped = true;
                EquppedEquipments[equipEquipmentInfo.Data.EquipmentType] = equipEquipmentInfo;
            }
            else
            {
                equipEquipmentInfo.IsEquipped = true;
                EquppedEquipments.Add(equipEquipmentInfo.Data.EquipmentType, equipEquipmentInfo);
            }

            Managers.Event.TriggerEvent(EEventType.UpdateEquipment);
        }
    }

    public void UnEquipEquipment(EEquipmentType equipmentType)
    {
        if (EquppedEquipments.TryGetValue(equipmentType, out EquipmentInfo equipmentInfo))
        {
            equipmentInfo.IsEquipped = false;
            EquppedEquipments.Remove(equipmentType);

            Managers.Event.TriggerEvent(EEventType.UpdateEquipment);
        }
    }
}
