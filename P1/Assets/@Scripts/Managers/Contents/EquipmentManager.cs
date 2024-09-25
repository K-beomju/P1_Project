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

    public int Level { get; set; }
    public bool IsEquipped { get; set; }
    public int Count { get; set; }

    public EquipmentInfo(int dataTemplateID)
    {
        Level = 1;
        IsEquipped = false;
        DataTemplateID = dataTemplateID;
        Data = Managers.Data.EquipmentDic[dataTemplateID];
        OwningState = EOwningState.Unowned;
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

    public Dictionary<EEquipmentType, EquipmentInfo> EquippedEquipments = new Dictionary<EEquipmentType, EquipmentInfo>();

    public void Init()
    {
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

    public void AddEquipment(int dataTemplateID)
    {
        if (AllEquipmentInfos.TryGetValue(dataTemplateID, out EquipmentInfo equipEquipmentInfo))
        {
            equipEquipmentInfo.OwningState = EOwningState.Owned;
            equipEquipmentInfo.Count += 1;
            Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
        }
    }

    public void EquipEquipment(int dataTemplateID)
    {
        if (AllEquipmentInfos.TryGetValue(dataTemplateID, out EquipmentInfo equipEquipmentInfo))
        {
            if (EquippedEquipments.TryGetValue(equipEquipmentInfo.Data.EquipmentType, out EquipmentInfo
            unEquipEquipmentInfo))
            {
                // 똑같은 장비 예외처리 
                if (equipEquipmentInfo == unEquipEquipmentInfo)
                    return;

                unEquipEquipmentInfo.IsEquipped = false;
                equipEquipmentInfo.IsEquipped = true;
                EquippedEquipments[equipEquipmentInfo.Data.EquipmentType] = equipEquipmentInfo;
            }
            else
            {
                equipEquipmentInfo.IsEquipped = true;
                EquippedEquipments.Add(equipEquipmentInfo.Data.EquipmentType, equipEquipmentInfo);
            }

            Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
        }
    }

    public void UnEquipEquipment(EEquipmentType equipmentType)
    {
        if (EquippedEquipments.TryGetValue(equipmentType, out EquipmentInfo equipmentInfo))
        {
            equipmentInfo.IsEquipped = false;
            EquippedEquipments.Remove(equipmentType);
        }
        Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
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

    public List<EquipmentInfo> GetEquipmentTypeInfos(EEquipmentType type)
    {
        return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.Data.EquipmentType == type).ToList();
    }

    public bool GetOwneded(int dataTemplateID)
    {
        return OwnedEquipments.Any(equipment => equipment.DataTemplateID == dataTemplateID);
    }

    public int GetEquippedId(EEquipmentType type)
    {
        // 장착된 장비 딕셔너리에서 해당 타입의 장비가 있는지 확인
        if (EquippedEquipments.TryGetValue(type, out EquipmentInfo equippedItem))
        {
            // 장착된 장비가 있으면 dataTemplateID 반환
            return equippedItem.DataTemplateID;
        }

        // 장착된 장비가 없으면 0 반환
        return 0;
    }


    public float OwnedEquipmentValues(EEquipmentType type)
    {
        float ownedValues = 0;

        // 소유 중인 장비 중 해당 타입의 장비 효과 합산
        foreach (var equipment in OwnedEquipments)
        {
            if (equipment.Data.EquipmentType == type)
            {
                ownedValues += equipment.Data.OwnedValue;
            }
        }

        return ownedValues;
    }

    public float EquipEquipmentValue(EEquipmentType type)
    {
        float equipValue = 0;

        // 장착된 장비 중 해당 타입의 장비 효과 합산
        foreach (var equipment in EquippedEquipments.Values)
        {
            if (equipment.Data.EquipmentType == type)
            {
                equipValue += equipment.Data.EquippedValue;
            }
        }

        return equipValue;
    }

}
