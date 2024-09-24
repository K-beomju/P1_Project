using System;
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
        Btn_Ring,
        
        Btn_Equip,
        Btn_Enhance
    }

    public enum EquipemntInven
    {
        EquipemntInven
    }

    private List<UI_EquipmentItem> equipmentItems = new List<UI_EquipmentItem>();
    private EquipmentInfo equipmentInfo;
    public EquipmentInfo EquipmentInfo
    {
        get { return equipmentInfo; }
        set
        {
            if(equipmentInfo != value)
            {
                equipmentInfo = value;
                GetButton((int)Buttons.Btn_Equip).interactable = !equipmentInfo.IsEquipped;
            }
        }
    }

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
        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(() => OnEquipEquipment());
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(() => OnEnhanceEquipment());

        RefreshUI(EEquipmentType.Weapon);
        return true;
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.EquipmentItemClick, new Action<EquipmentInfo>(ShowEquipmentDetailUI));
    }

    void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.EquipmentItemClick, new Action<EquipmentInfo>(ShowEquipmentDetailUI));
    }
    public void RefreshUI(EEquipmentType type)
    {
        for (int i = 0; i < equipmentItems.Count; i++)
        {
            List<EquipmentInfo> equipmentInfos = Managers.Equipment.GetEquipmentInfos(type);
            equipmentItems[i].SetInfo(equipmentInfos[i], false);
        }
    }

    public void ShowEquipmentDetailUI(EquipmentInfo _equipmentInfo)
    {
        EquipmentInfo = _equipmentInfo;
        Debug.Log(equipmentInfo.Data.EquipmentType);
    }

    private void OnEquipEquipment()
    {
        Managers.Equipment.EquipEquipment(equipmentInfo.DataTemplateID);

        foreach (var item in Managers.Equipment.EquppedEquipments)
        {
            Debug.Log(item.Key + " " + item.Value.Data.Name);
        }

        GetButton((int)Buttons.Btn_Equip).interactable = false;
    }

    private void OnEnhanceEquipment()
    {

    }

}
