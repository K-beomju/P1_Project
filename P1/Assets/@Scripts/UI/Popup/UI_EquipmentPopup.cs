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
        Btn_Enhance,

        Btn_AutoEquip,
        Btn_BatchEnhance
    }

    public enum Texts
    {
        Text_EquipmentType,

        Text_EquipmentName,
        Text_EquipmentRare,
        Text_EquipmentLevel,
        Text_OwendAmount,
        Text_EquipmentValueText
    }


    private UI_EquipmentItem equipmentItem;
    private List<UI_EquipmentItem> equipmentItems = new List<UI_EquipmentItem>();

    private EquipmentInfo equipmentInfo;
    public EquipmentInfo EquipmentInfo
    {
        get { return equipmentInfo; }
        set
        {
            if (equipmentInfo != value)
            {
                equipmentInfo = value;

                // 장착 버튼은 장착된 장비 였을 때 , 가지고 있지 않은 장비일 때 -> 비활
                // 강화 버튼은 장착된 장비가 강화 갯수만큼 가지고 있지 않을 때 , 가지고 있지 않은 장비 일 때 -> 비활  
                GetButton((int)Buttons.Btn_Equip).interactable = !equipmentInfo.IsEquipped && equipmentInfo.OwningState == EOwningState.Owned;
                GetButton((int)Buttons.Btn_Enhance).interactable = equipmentInfo.OwningState == EOwningState.Owned;
            }
        }
    }

    private EEquipmentType equipmentType;
    public EEquipmentType EquipmentType
    {
        get { return equipmentType; }
        set
        {
            if (equipmentType != value)
            {
                equipmentType = value;
                ShowEquipmentAllUI(value);

                GetText((int)Texts.Text_EquipmentType).text = Util.GetEquipmentString(value);
                switch (equipmentType)
                {
                    case EEquipmentType.Weapon:
                        ShowEquipmentDetailUI(Managers.Equipment.GetEquipmentInfo(100100));
                        break;
                    case EEquipmentType.Armor:
                        ShowEquipmentDetailUI(Managers.Equipment.GetEquipmentInfo(200100));
                        break;
                    case EEquipmentType.Ring:
                        ShowEquipmentDetailUI(Managers.Equipment.GetEquipmentInfo(300100));
                        break;
                }
            }
        }
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_Weapon).onClick.AddListener(() => EquipmentType = EEquipmentType.Weapon);
        GetButton((int)Buttons.Btn_Armor).onClick.AddListener(() => EquipmentType = EEquipmentType.Armor);
        GetButton((int)Buttons.Btn_Ring).onClick.AddListener(() => EquipmentType = EEquipmentType.Ring);
        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(() => OnEquipEquipment());
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(() => OnEnhanceEquipment());
        GetButton((int)Buttons.Btn_AutoEquip).onClick.AddListener(() => OnAutoEquipment());
        GetButton((int)Buttons.Btn_BatchEnhance).onClick.AddListener(() => OnBatchEnhanceEquipment());

        EquipmentType = EEquipmentType.None;
        equipmentItem = Util.FindChild<UI_EquipmentItem>(gameObject, "UI_EquipmentItem", true);
        for (int i = 0; i < 24; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentItem>(GetObject((int)GameObjects.Content_Equipment).transform);
            equipmentItems.Add(item);
        }

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
    public void RefreshUI()
    {
        // 처음에 패널을 활성화하면 디폴드 값으로 무기 
        if (EquipmentType == EEquipmentType.None)
        {
            EquipmentType = EEquipmentType.Weapon;
        }
    }

    // 장비 Item을 클릭했을 때 보여주는 부분 
    public void ShowEquipmentDetailUI(EquipmentInfo _equipmentInfo)
    {
        EquipmentInfo = _equipmentInfo;
        if (EquipmentInfo == null)
            return;
        
        equipmentItem.SetEquipmentInfo(EquipmentInfo);
        GetText((int)Texts.Text_EquipmentName).text = EquipmentInfo.Data.Name;
        GetText((int)Texts.Text_EquipmentRare).text = Util.GetRareTypeString(EquipmentInfo.Data.RareType);
        GetText((int)Texts.Text_EquipmentValueText).text = $"장착 효과 : {EquipmentInfo.Data.EquippedValue}% \n 보유 효과 : {EquipmentInfo.Data.OwnedValue}%";

        Debug.Log(equipmentInfo.Data.EquipmentType);
    }

    // 장비 타입을 바꿨을 때 Item 갱신 
    public void ShowEquipmentAllUI(EEquipmentType type)
    {
        for (int i = 0; i < equipmentItems.Count; i++)
        {
            List<EquipmentInfo> equipmentInfos = Managers.Equipment.GetEquipmentInfos(type);
            equipmentItems[i].SetEquipmentInfo(equipmentInfos[i]);
        }
    }




    #region Button Action

    // 장착 
    private void OnEquipEquipment()
    {
        Managers.Equipment.EquipEquipment(equipmentInfo.DataTemplateID);

        foreach (var item in Managers.Equipment.EquppedEquipments)
        {
            Debug.Log(item.Key + " " + item.Value.Data.Name);
        }

        GetButton((int)Buttons.Btn_Equip).interactable = false;
    }

    // 강화
    private void OnEnhanceEquipment()
    {

    }

    // 자동 장착 
    private void OnAutoEquipment()
    {

    }

    // 일괄 강화 
    private void OnBatchEnhanceEquipment()
    {

    }

    #endregion

}
