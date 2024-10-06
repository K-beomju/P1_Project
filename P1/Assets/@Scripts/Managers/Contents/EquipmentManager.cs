using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackendData.GameData;
using UnityEngine;
using static Define;

public class EquipmentManager
{
    public Dictionary<int, EquipmentInfoData> AllEquipmentInfos { get; private set; } = new Dictionary<int, EquipmentInfoData>();

    public void Init()
    {
        InitializeEquipment(EEquipmentType.Weapon, 100100, 24);
        InitializeEquipment(EEquipmentType.Armor, 200100, 24);
        InitializeEquipment(EEquipmentType.Ring, 300100, 24);
    }

    // 무기, 갑옷, 반지의 초기화를 각각 개별 메서드로 분리하여 중복 코드 제거
    private void InitializeEquipment(EEquipmentType type, int startIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int index = startIndex + i;
            var equipmentInfo = new EquipmentInfoData(index, EOwningState.Unowned, 0, 0, false);
            AllEquipmentInfos.Add(equipmentInfo.DataTemplateID, equipmentInfo);
        }
    }

    // 특정 타입의 장비 정보 리스트 반환
    public List<EquipmentInfoData> GetEquipmentTypeInfos(EEquipmentType type)
    {
        return AllEquipmentInfos.Values
            .Where(equipmentInfo => equipmentInfo.Data.EquipmentType == type)
            .ToList();
    }

    // 소유 중인 특정 타입 장비의 효과 합산 값 반환
    public float OwnedEquipmentValues(EEquipmentType type)
    {
        float ownedValues = 0;

        // 소유 중인 장비 중 해당 타입의 장비 효과 합산
        foreach (var equipment in Managers.Backend.GameData.EquipmentInventory.EquipmentInventoryDic.Values)
        {
            if (equipment.Data.EquipmentType == type)
            {
                ownedValues += equipment.Data.OwnedValue;
            }
        }

        return ownedValues;
    }

    // 장착 중인 특정 타입 장비의 효과 합산 값 반환
    public float EquipEquipmentValue(EEquipmentType type)
    {
        float equipValue = 0;

        // 장착된 장비 중 해당 타입의 장비 효과 합산
        foreach (var equipment in Managers.Backend.GameData.EquipmentInventory.EquipmentEquipDic.Values)
        {
            if (equipment.Data.EquipmentType == type)
            {
                equipValue += equipment.Data.EquippedValue;
            }
        }

        return equipValue;
    }

}
