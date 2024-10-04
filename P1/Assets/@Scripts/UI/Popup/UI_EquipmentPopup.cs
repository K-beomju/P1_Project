using BackendData.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_EquipmentPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
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

    public enum Sliders
    {
        Slider_EquipmentCount
    }


    private UI_EquipmentItem equipmentItem;
    private List<UI_EquipmentItem> equipmentItems = new List<UI_EquipmentItem>();

    private EquipmentInfoData selectEquipmentInfo;
    public EquipmentInfoData SelectEquipmentInfo
    {
        get { return selectEquipmentInfo; }
        set
        {
            if (selectEquipmentInfo != value)
            {
                selectEquipmentInfo = value;
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
                GetTMPText((int)Texts.Text_EquipmentType).text = Util.GetEquipmentString(value);
            }
        }
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));

        GetButton((int)Buttons.Btn_Weapon).onClick.AddListener(() => OnClickButton(EEquipmentType.Weapon));
        GetButton((int)Buttons.Btn_Armor).onClick.AddListener(() => OnClickButton(EEquipmentType.Armor));
        GetButton((int)Buttons.Btn_Ring).onClick.AddListener(() => OnClickButton(EEquipmentType.Ring));
        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(() => OnEquipEquipment());
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(() => OnEnhanceEquipment());
        GetButton((int)Buttons.Btn_AutoEquip).onClick.AddListener(() => OnAutoEquipment());
        GetButton((int)Buttons.Btn_BatchEnhance).onClick.AddListener(() => OnBatchEnhanceEquipment());
        GetObject((int)GameObjects.BG).BindEvent(() =>
        {
            (Managers.UI.SceneUI as UI_GameScene).CloseDrawPopup(this);

        }, EUIEvent.Click);

        equipmentItem = Util.FindChild<UI_EquipmentItem>(gameObject, "UI_EquipmentItem", true);
        for (int i = 0; i < 24; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentItem>(GetObject((int)GameObjects.Content_Equipment).transform);
            equipmentItems.Add(item);
        }

        EquipmentType = EEquipmentType.Weapon;
        return true;
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.EquipmentItemClick, new Action<EquipmentInfoData>(ShowEquipmentDetailUI));
    }

    void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.EquipmentItemClick, new Action<EquipmentInfoData>(ShowEquipmentDetailUI));
    }

    void OnClickButton(EEquipmentType type)
    {
        if (EquipmentType == type)
            return;

        EquipmentType = type;
        RefreshUI();
    }

    public void SetInfo(EEquipmentType type)
    {
        EquipmentType = type;
        RefreshUI();
    }

    // 장비 타입을 바꿨을 때 Item 갱신 
    public void RefreshUI()
    {
        // 장비 인벤토리 중 장착된 장비 찾기 
        EquipmentInfoData equippedInfo = BackendManager.Instance.GameData.UserData.EquipmentInventoryDic.Values
            .FirstOrDefault(equipmentInfo => equipmentInfo.Data.EquipmentType == EquipmentType && equipmentInfo.IsEquipped);

        List<EquipmentInfoData> equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(EquipmentType);

        // 장비 인벤과 동기화 
        for (int i = 0; i < equipmentInfos.Count; i++)
        {
            // 만약에 내가 가진 장비 인벤과 동일한 ID일 경우
            if (BackendManager.Instance.GameData.UserData.EquipmentInventoryDic.TryGetValue(equipmentInfos[i].DataTemplateID, out var equipmentInfoData))
            {
                // 동기화
                equipmentInfos[i] = equipmentInfoData; 
            }
        }

        if (equippedInfo != null)
        {
            // 장착된 장비가 있으면 해당 장비를 보여줍니다.
            ShowEquipmentDetailUI(equippedInfo);
        }
        else
        {
            // 장착된 장비가 없으면 소유한 장비나 가장 낮은 등급의 장비를 보여줍니다.
            EquipmentInfoData ownedEquipment = equipmentInfos
                .Where(equipmentInfo => equipmentInfo.OwningState == EOwningState.Owned)
                .LastOrDefault();

            if (ownedEquipment != null)
            {
                ShowEquipmentDetailUI(ownedEquipment);
            }
            else
            {
                // 소유한 장비가 없을 때 가장 낮은 등급의 장비를 보여줍니다.
                EquipmentInfoData lowestRareEquipment = equipmentInfos
                    .OrderBy(equipmentInfo => equipmentInfo.Data.RareType)
                    .FirstOrDefault();

                if (lowestRareEquipment != null)
                {
                    ShowEquipmentDetailUI(lowestRareEquipment);
                }
            }
        }


        for (int i = 0; i < equipmentItems.Count; i++)
        {
            equipmentItems[i].SetEquipmentInfo(equipmentInfos[i]);
        }
    }

    // 장비 Item을 클릭했을 때 보여주는 부분 
    public void ShowEquipmentDetailUI(EquipmentInfoData _equipmentInfo)
    {
        SelectEquipmentInfo = _equipmentInfo;
        if (SelectEquipmentInfo == null)
            return;

        equipmentItem.SetEquipmentInfo(SelectEquipmentInfo);
        GetTMPText((int)Texts.Text_EquipmentName).text = SelectEquipmentInfo.Data.Name;
        GetTMPText((int)Texts.Text_EquipmentLevel).text = $"Lv. {SelectEquipmentInfo.Level}";
        GetTMPText((int)Texts.Text_EquipmentRare).text = Util.GetRareTypeString(SelectEquipmentInfo.Data.RareType);
        GetTMPText((int)Texts.Text_EquipmentValueText).text
        = $"<color=#FFA500>장착 효과 : {SelectEquipmentInfo.Data.EquippedValue}%</color> \n<color=#00FF00>보유 효과 : {SelectEquipmentInfo.Data.OwnedValue}%</color>";

        int currentCount = SelectEquipmentInfo.Count;
        int maxCount = Util.GetUpgradeEquipmentMaxCount(SelectEquipmentInfo.Level);
        GetSlider((int)Sliders.Slider_EquipmentCount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_EquipmentCount).value = currentCount;  // 슬라이더의 현재 값도 업데이트
        GetTMPText((int)Texts.Text_OwendAmount).text = $"{currentCount} / {maxCount}";



        GetButton((int)Buttons.Btn_Equip).interactable = !SelectEquipmentInfo.IsEquipped && SelectEquipmentInfo.OwningState == EOwningState.Owned;
        GetButton((int)Buttons.Btn_Enhance).interactable =
        SelectEquipmentInfo.OwningState == EOwningState.Owned && SelectEquipmentInfo.Count >= Util.GetUpgradeEquipmentMaxCount(SelectEquipmentInfo.Level);
    }


    #region Button Action

    // 장착 
    private void OnEquipEquipment()
    {
        Managers.Equipment.EquipEquipment(SelectEquipmentInfo.DataTemplateID);
        GetButton((int)Buttons.Btn_Equip).interactable = false;
    }

    // 강화
    private void OnEnhanceEquipment()
    {
        int maxCount = Util.GetUpgradeEquipmentMaxCount(SelectEquipmentInfo.Level);
        if(SelectEquipmentInfo.Count >= maxCount)
        {
            BackendManager.Instance.GameData.UserData.EquipmentLevelUp(SelectEquipmentInfo);
            ShowEquipmentDetailUI(SelectEquipmentInfo);
        }
    }

    // 자동 장착 
    private void OnAutoEquipment()
    {
        // 추후에 생각
        List<EquipmentInfoData> equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(EquipmentType);

        EquipmentInfoData bestEquipment = equipmentInfos
        .Where(info => info.OwningState == EOwningState.Owned)
        .OrderByDescending(info => info.Data.RareType)
        .ThenByDescending(info => info.Data.DataId)
        .FirstOrDefault();

        if (bestEquipment != null)
        {
            Managers.Equipment.EquipEquipment(bestEquipment.DataTemplateID);
            ShowEquipmentDetailUI(bestEquipment);
            GetButton((int)Buttons.Btn_Equip).interactable = false;
        }
    }

    // 일괄 강화 
    private void OnBatchEnhanceEquipment()
    {

    }

    #endregion

}
