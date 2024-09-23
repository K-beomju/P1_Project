using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_EquipmentPopup : UI_Popup
{
    public enum GameObjects
    {
        Content_Equipment
    }

    public enum Buttons
    {
        Btn_Weapon,
        Btn_Armor,
        Btn_Ring
    }

    public enum EquipemntInven 
    {
        EquipemntInven 
    }

    private List<UI_EquipmentItem> equipmentItems = new List<UI_EquipmentItem>();
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        for (int i = 0; i < 24; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentItem>(GetObject((int)GameObjects.Content_Equipment).transform);
            equipmentItems.Add(item);
        }

        GetButton((int)Buttons.Btn_Weapon).onClick.AddListener(() => RefreshUI(EEquipmentType.Weapon));
        GetButton((int)Buttons.Btn_Armor).onClick.AddListener(() => RefreshUI(EEquipmentType.Armor));
        GetButton((int)Buttons.Btn_Ring).onClick.AddListener(() => RefreshUI(EEquipmentType.Ring));

        RefreshUI(EEquipmentType.Weapon);
        return true;
    }

    public void RefreshUI(EEquipmentType type)
    {
        for (int i = 0; i < equipmentItems.Count; i++)
        {
            equipmentItems[i].SetInfo(Managers.Equipment.GetEquipmentInfo(GetEquipmentInfos(type)[i].DataTemplateID), false);
        }
    }

    private List<EquipmentInfo> GetEquipmentInfos(EEquipmentType type)
    {
        List<EquipmentInfo> equipmentInfos = new List<EquipmentInfo>();
        switch (type)
        {
            case EEquipmentType.Weapon:
                equipmentInfos = Managers.Equipment.WeaponEquipments;
                break;
            case EEquipmentType.Armor:
                equipmentInfos = Managers.Equipment.ArmorEquipments;
                break;
            case EEquipmentType.Ring:
                equipmentInfos = Managers.Equipment.RingEquipments;
                break;
        }

        return equipmentInfos;
    }

}
