using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackendData.GameData;
using static Define;

public class EquipmentManager
{
    public Dictionary<int, EquipmentInfoData> AllEquipmentInfos { get; private set; } = new Dictionary<int, EquipmentInfoData>();

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

    }

    public void AddEquipment(int dataTemplateID)
    {
        BackendManager.Instance.GameData.EquipmentInventory.AddEquipment(dataTemplateID);
    }

    public void EquipEquipment(int dataTemplateID)
    {
        BackendManager.Instance.GameData.EquipmentInventory.EquipEquipment(dataTemplateID);
    }

    public List<EquipmentInfoData> GetEquipmentTypeInfos(EEquipmentType type)
    {
        return AllEquipmentInfos.Values.Where(equipmentInfo => equipmentInfo.Data.EquipmentType == type).ToList();
    }



    public float OwnedEquipmentValues(EEquipmentType type)
    {
        float ownedValues = 0;

        // 소유 중인 장비 중 해당 타입의 장비 효과 합산
        foreach (var equipment in BackendManager.Instance.GameData.EquipmentInventory.EquipmentInventoryDic.Values)
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
        //foreach (var equipment in BackendManager.Instance.GameData.EquipmentInventory.EquipmentEquipDic.Values)
        //{
        //    if (equipment.Data.EquipmentType == type)
        //    {
        //        equipValue += equipment.Data.EquippedValue;
        //    }
        //}

        return equipValue;
    }

}
