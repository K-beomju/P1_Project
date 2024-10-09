using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BackendData.GameData;
using static Define;
using static UnityEditor.Progress;

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


    private UI_CompanionItem equipmentItem;
    private List<UI_CompanionItem> equipmentItems = new List<UI_CompanionItem>();

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
        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(OnEquipEquipment);
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(OnEnhanceEquipment);
        GetButton((int)Buttons.Btn_AutoEquip).onClick.AddListener(OnAutoEquipment);
        GetButton((int)Buttons.Btn_BatchEnhance).onClick.AddListener(OnBatchEnhanceEquipment);
        GetObject((int)GameObjects.BG).BindEvent(() =>
        {
            (Managers.UI.SceneUI as UI_GameScene).CloseDrawPopup(this);

        }, EUIEvent.Click);

        equipmentItem = Util.FindChild<UI_CompanionItem>(gameObject, "UI_CompanionItem", true);
        for (int i = 0; i < 24; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_CompanionItem>(GetObject((int)GameObjects.Content_Equipment).transform);
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
        EquipmentInfoData equippedInfo = Managers.Backend.GameData.EquipmentInventory.EquipmentInventoryDic.Values
            .FirstOrDefault(equipmentInfo => equipmentInfo.Data.EquipmentType == EquipmentType && equipmentInfo.IsEquipped);

        List<EquipmentInfoData> equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(EquipmentType, true);

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
        if (SelectEquipmentInfo == _equipmentInfo)
            return;
        SelectEquipmentInfo = _equipmentInfo;

        equipmentItem.SetEquipmentInfo(SelectEquipmentInfo, false);
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
        try
        {
            Managers.Backend.GameData.EquipmentInventory.EquipEquipment(SelectEquipmentInfo.DataTemplateID);
            GetButton((int)Buttons.Btn_Equip).interactable = false;
        }
        catch (Exception e)
        {
            throw new Exception($"OnEquipEquipment({SelectEquipmentInfo.DataTemplateID}) 중 에러가 발생하였습니다\n{e}");
        }
    }

    // 강화
    private void OnEnhanceEquipment()
    {
        int maxCount = Util.GetUpgradeEquipmentMaxCount(SelectEquipmentInfo.Level);
        if (SelectEquipmentInfo.Count >= maxCount)
        {
            try
            {
                Managers.Backend.GameData.EquipmentInventory.EquipmentLevelUp(SelectEquipmentInfo, maxCount);
                ShowEquipmentDetailUI(SelectEquipmentInfo);
            }
            catch (Exception e)
            {
                throw new Exception($"OnEnhanceEquipment({SelectEquipmentInfo}) 중 에러가 발생하였습니다\n{e}");
            }

        }
    }

    // 자동 장착 
    private void OnAutoEquipment()
    {
        List<EquipmentInfoData> equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(EquipmentType, true);

        EquipmentInfoData bestEquipment = equipmentInfos
        .Where(info => info.OwningState == EOwningState.Owned)
        .OrderByDescending(info => info.Data.RareType)
        .ThenByDescending(info => info.Data.DataId)
        .FirstOrDefault();

        if (bestEquipment != null)
        {
            try
            {
                Managers.Backend.GameData.EquipmentInventory.EquipEquipment(bestEquipment.DataTemplateID);
                ShowEquipmentDetailUI(bestEquipment);
                GetButton((int)Buttons.Btn_Equip).interactable = false;
            }
            catch (Exception e)
            {
                throw new Exception($"OnAutoEquipment({bestEquipment}) 중 에러가 발생하였습니다\n{e}");
            }
        }
    }

    // 일괄 강화 - 최대 레벨까지 강화
    private void OnBatchEnhanceEquipment()
    {
        List<EquipmentInfoData> equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(EquipmentType, true);

        int enhancedCount = 0;

        foreach (var equipment in equipmentInfos)
        {
            // 강화 가능한 장비인지 확인
            if (equipment.OwningState == EOwningState.Owned)
            {
                // 강화 가능한 최대 레벨 (예시로 설정한 값, 필요시 실제 최대 레벨 값으로 대체)
                int maxLevel = 100;  // 실제 최대 레벨 값으로 대체하세요.

                // 현재 장비 레벨이 최대 레벨보다 낮고, 강화가 가능한 경우 반복하여 강화 수행
                while (equipment.Level < maxLevel)
                {
                    // 강화에 필요한 최대 개수 확인
                    int maxCount = Util.GetUpgradeEquipmentMaxCount(equipment.Level);

                    // 현재 개수가 최대 강화 개수 이상일 때만 강화 수행
                    if (equipment.Count >= maxCount)
                    {
                        try
                        {
                            // 장비 강화 수행
                            Managers.Backend.GameData.EquipmentInventory.EquipmentLevelUp(equipment, maxCount);
                            enhancedCount++;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"OnBatchEnhanceEquipment({equipment.DataTemplateID}) 중 에러가 발생하였습니다\n{e}");
                            break; // 에러 발생 시 해당 장비 강화 중단
                        }
                    }
                    else
                    {
                        // 현재 개수가 부족하여 더 이상 강화할 수 없으면 루프 종료
                        break;
                    }
                }
            }
        }

        // 강화가 완료되면 UI를 갱신합니다.
        if (enhancedCount > 0)
        {
            Debug.Log($"{enhancedCount}개의 장비가 최대 레벨까지 일괄 강화되었습니다.");
            equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(EquipmentType, true);
            for (int i = 0; i < equipmentItems.Count; i++)
            {
                equipmentItems[i].SetEquipmentInfo(equipmentInfos[i]);
            }
            ShowEquipmentDetailUI(SelectEquipmentInfo);

        }
        else
        {
            Debug.Log("강화할 수 있는 장비가 없습니다.");
        }
    }


    #endregion

}
