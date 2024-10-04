using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using BackendData.GameData;
using static Define;



public class EquipmentManager
{
    public Dictionary<int, EquipmentInfoData> AllEquipmentInfos { get; private set; } = new Dictionary<int, EquipmentInfoData>();

    public List<EquipmentInfoData> OwnedEquipments
    {
        get { return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.OwningState == EOwningState.Owned).ToList(); }
    }

    public List<EquipmentInfoData> UnownedEquipments
    {
        get { return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.OwningState == EOwningState.Unowned).ToList(); }
    }

    public Dictionary<EEquipmentType, EquipmentInfoData> EquippedEquipments = new Dictionary<EEquipmentType, EquipmentInfoData>();

    public void Init()
    {
        // 무기
        int index = 100100;
        for (int i = 0; i < 24; i++)
        {
            EquipmentInfoData equipmentInfo = new EquipmentInfoData(index, EOwningState.Unowned, 0, 0, false);
            AllEquipmentInfos.Add(equipmentInfo.DataTemplateID, equipmentInfo);
            index += 1;
        }
        // 갑옷
        index = 200100;
        for (int i = 0; i < 24; i++)
        {
            EquipmentInfoData equipmentInfo = new EquipmentInfoData(index, EOwningState.Unowned, 0, 0, false);
            AllEquipmentInfos.Add(equipmentInfo.DataTemplateID, equipmentInfo);
            index += 1;
        }
        // 반지
        index = 300100;
        for (int i = 0; i < 24; i++)
        {
            EquipmentInfoData equipmentInfo = new EquipmentInfoData(index, EOwningState.Unowned, 0, 0, false);
            AllEquipmentInfos.Add(equipmentInfo.DataTemplateID, equipmentInfo);
            index += 1;
        }

        List<int> keys = new List<int>(AllEquipmentInfos.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            int dataId = keys[i];

            // 전체 장비 중에 가지고 있는 장비가 있다면
            if (BackendManager.Instance.GameData.UserData.EquipmentInventoryDic.TryGetValue(dataId, out EquipmentInfoData equipmentInfo))
            {
                // 업데이트: Dictionary의 값을 수정합니다.
                AllEquipmentInfos[dataId] = BackendManager.Instance.GameData.UserData.EquipmentInventoryDic[dataId];
            }
        }

    }

    public void AddEquipment(int dataTemplateID)
    {
        BackendManager.Instance.GameData.UserData.AddEquipment(dataTemplateID);

        List<int> keys = new List<int>(AllEquipmentInfos.Keys);

        // 전체 장비 중에 가지고 있는 장비가 있다면
        if (BackendManager.Instance.GameData.UserData.EquipmentInventoryDic.TryGetValue(dataTemplateID, out EquipmentInfoData equipmentInfo))
        {
            // 업데이트: Dictionary의 값을 수정합니다.
            AllEquipmentInfos[dataTemplateID] = BackendManager.Instance.GameData.UserData.EquipmentInventoryDic[dataTemplateID];
        }
    }

    public void EquipEquipment(int dataTemplateID)
    {
        BackendManager.Instance.GameData.UserData.EquipEquipment(dataTemplateID);
    }

    public EquipmentInfoData GetEquipmentInfo(int dataTemplateID)
    {
        if (AllEquipmentInfos.TryGetValue(dataTemplateID, out EquipmentInfoData equipmentInfo))
        {
            return equipmentInfo;
        }
        else
        {
            return null;
        }
    }

    public List<EquipmentInfoData> GetEquipmentTypeInfos(EEquipmentType type)
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
        if (EquippedEquipments.TryGetValue(type, out EquipmentInfoData equippedItem))
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
