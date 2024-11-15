using BackendData.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_SkillPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        Content_Skill
    }

    public enum Buttons
    {
        Btn_Equip,
        Btn_UnEquip,
        Btn_Enhance,
        Btn_AutoEquip,
        Btn_BatchEnhance
    }

    public enum Texts
    {
        Text_SkillName,
        Text_SkillRare,
        Text_SkillLevel,
        Text_OwendAmount,
        Text_SkillDesc,
        Text_SkillOwnedValue,
        Text_SkillCoolTime
    }

    public enum Sliders
    {
        Slider_SkillCount
    }

    public enum SkillSlots
    {
        UI_SkillSlot_1,
        UI_SkillSlot_2,
        UI_SkillSlot_3,
        UI_SkillSlot_4,
        UI_SkillSlot_5,
        UI_SkillSlot_6
    }


    private List<UI_SkillSlot> _skillSlotList = new List<UI_SkillSlot>();
    private List<UI_CompanionItem> _companionItems = new List<UI_CompanionItem>();
    private UI_CompanionItem _companionItem;
    private UI_CompanionItem _placeCompanionItem;

    private SkillInfoData selectSkillInfo;
    public SkillInfoData SelectSkillInfo
    {
        get { return selectSkillInfo; }
        set
        {
            if (selectSkillInfo != value)
            {
                selectSkillInfo = value;
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
        Bind<UI_SkillSlot>(typeof(SkillSlots));

        GetObject((int)GameObjects.BG).BindEvent(() => (Managers.UI.SceneUI as UI_GameScene).ShowTab(UI_GameScene.PlayTab.Skill));

        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(OnEquipSkill);
        GetButton((int)Buttons.Btn_UnEquip).onClick.AddListener(OnUnEquipSkill);
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(OnEnhanceSkill);
        GetButton((int)Buttons.Btn_AutoEquip).onClick.AddListener(OnAutoEquipSkill);
        GetButton((int)Buttons.Btn_BatchEnhance).onClick.AddListener(OnBatchEnhanceSkill);
        GetButton((int)Buttons.Btn_UnEquip).gameObject.SetActive(false);

        _companionItem = Util.FindChild<UI_CompanionItem>(gameObject, "UI_CompanionItem", true);
        _placeCompanionItem = Util.FindChild<UI_CompanionItem>(gameObject, "UI_CompanionItemPlace", true);
        _placeCompanionItem.gameObject.SetActive(false);
        for (int i = 0; i < Managers.Skill.AllSkillInfos.Count; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_CompanionItem>(GetObject((int)GameObjects.Content_Skill).transform);
            _companionItems.Add(item);
        }

        // 6개의 슬롯을 초기화하면서 각 슬롯을 Lock 타입으로 설정
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            _skillSlotList.Add(Get<UI_SkillSlot>(i));
            _skillSlotList[i].SetInfo(index);
            _skillSlotList[i]._button.onClick.AddListener(() => OnSkillSlotClicked(index));

        }
        return true;
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.SkillItemClick, new Action<SkillInfoData>(ShowSkillDetailUI));
    }

    void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.SkillItemClick, new Action<SkillInfoData>(ShowSkillDetailUI));
    }

    public void RefreshUI()
    {
        //////////////////////////// 스킬 슬롯 부분 
        foreach (var slot in Managers.Backend.GameData.SkillInventory.SkillSlotList)
        {
            _skillSlotList[slot.Index].RefreshUI();
        }


        // 스킬 인벤토리 중 스킬 장비 찾기 
        SkillInfoData equippedInfo = Managers.Backend.GameData.SkillInventory.SkillInventoryDic.Values.FirstOrDefault(skillinfo => skillinfo.IsEquipped);

        List<SkillInfoData> skillInfos = Managers.Skill.GetSkillInfos();

        //////////////////////////// ShowSkillDetailUI 부분 
        if (equippedInfo != null)
        {
            ShowSkillDetailUI(equippedInfo);
        }
        else
        {
            // 소유한 스킬이 없을 때 가장 낮은 등급의 스킬을 보여줍니다.
            SkillInfoData lowestRareSkill = skillInfos
                .OrderBy(skillinfo => skillinfo.Data.RareType)
                .FirstOrDefault();

            if (lowestRareSkill != null)
            {
                ShowSkillDetailUI(lowestRareSkill);
            }
        }

        //////////////////////////// 스킬 아이템 목록 부분 
        for (int i = 0; i < _companionItems.Count; i++)
        {
            _companionItems[i].DisplayItem(skillInfos[i], EItemDisplayType.Basic);
        }
    }

    public void ShowSkillDetailUI(SkillInfoData skillInfo)
    {
        if (SelectSkillInfo == skillInfo)
            return;

        SelectSkillInfo = skillInfo;
        _companionItem.DisplayItem(SelectSkillInfo, EItemDisplayType.ImageOnly);
        GetTMPText((int)Texts.Text_SkillName).text = SelectSkillInfo.Data.Name;
        GetTMPText((int)Texts.Text_SkillLevel).text = $"Lv. {SelectSkillInfo.Level}";
        GetTMPText((int)Texts.Text_SkillRare).text = Util.GetRareTypeString(SelectSkillInfo.Data.RareType);

        string originalString = SelectSkillInfo.Data.Description;
        string replacedString = originalString.Replace("{value}",   SelectSkillInfo.Data.DamageMultiplier.ToString());
        GetTMPText((int)Texts.Text_SkillDesc).text = replacedString;
        GetTMPText((int)Texts.Text_SkillOwnedValue).text = $"보유효과: 공격력{SelectSkillInfo.Data.OwnedValue}% 증가";
        GetTMPText((int)Texts.Text_SkillCoolTime).text = $"{SelectSkillInfo.Data.CoolTime}초";

        int currentCount = SelectSkillInfo.Count;
        int maxCount = Util.GetUpgradeEquipmentMaxCount(SelectSkillInfo.Level);
        GetSlider((int)Sliders.Slider_SkillCount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_SkillCount).value = currentCount;
        GetTMPText((int)Texts.Text_OwendAmount).text = $"{currentCount} / {maxCount}";

        SetEquipButtonState(SelectSkillInfo.IsEquipped);


    }

    private void OnEquipSkill()
    {
        // 모든 슬롯이 잠금 상태인 경우
        if (AreAllSlotsLocked())
        {
            Debug.LogWarning("모든 슬롯이 잠겨 있어 스킬을 장착할 수 없습니다.");
            return;
        }

        // 현재 선택된 스킬이 인벤토리에 없는 경우
        if (!IsSkillOwned(SelectSkillInfo))
        {
            Debug.LogWarning("선택된 스킬이 인벤토리에 없습니다.");
            return;
        }


        if (IsSlotFull())
        {
            ActivateReplaceSkillMode();
            Debug.LogWarning("모든 슬롯이 가득 찼습니다. 교체할 슬롯을 선택하세요.");
            return;
        }

        try
        {
            Managers.Backend.GameData.SkillInventory.EquipSkill(SelectSkillInfo.DataTemplateID, (int slotIndex) =>
            {
                if (slotIndex >= 0)
                {
                    (Managers.UI.SceneUI as UI_GameScene)._equipSkillSlotList[slotIndex].RefreshUI();
                    _skillSlotList[slotIndex].RefreshUI();
                    SetEquipButtonState(true);
                    RefreshUI();
                }
                else
                {
                    Debug.LogWarning($"스킬 장착에 실패했습니다. DataTemplateID{SelectSkillInfo.DataTemplateID}");
                }
            });
        }
        catch (Exception e)
        {
            throw new Exception($"OnEquipSkill({SelectSkillInfo}) 중 에러가 발생하였습니다\n{e}");
        }
    }

    private void OnUnEquipSkill()
    {
        try
        {
            Managers.Backend.GameData.SkillInventory.UnEquipSkill(SelectSkillInfo.DataTemplateID, (int slotIndex) =>
            {
                if (slotIndex >= 0)
                {
                    (Managers.UI.SceneUI as UI_GameScene)._equipSkillSlotList[slotIndex].RefreshUI();
                    _skillSlotList[slotIndex].RefreshUI();
                    SetEquipButtonState(false);
                    RefreshUI();
                }
            });
        }
        catch (Exception e)
        {
            throw new Exception($"OnUnEquipSkill({SelectSkillInfo}) 중 에러가 발생하였습니다\n{e}");
        }
    }

    private void ActivateReplaceSkillMode()
    {
        _placeCompanionItem.gameObject.SetActive(true);
        _placeCompanionItem.PlayShakeAnimation();
        _placeCompanionItem.DisplayItem(SelectSkillInfo, EItemDisplayType.ImageOnly);
        GetObject((int)GameObjects.Content_Skill).SetActive(false);

        // 모든 슬롯의 버튼을 비활성화하여 교체 대기 상태로 전환
        foreach (var slot in Managers.Backend.GameData.SkillInventory.SkillSlotList
        .Where(slot => slot.SlotType == ESkillSlotType.Equipped))
        {
            int index = slot.Index; // 슬롯의 인덱스 가져오기
            _skillSlotList[index].EnableButton(false);
        }
    }

    private void OnSkillSlotClicked(int slotIndex)
    {
        var skillSlot = Managers.Backend.GameData.SkillInventory.SkillSlotList[slotIndex];

        // 잠긴 슬롯은 선택 불가
        if (skillSlot.SlotType == ESkillSlotType.Lock)
        {
            Debug.LogWarning($"{slotIndex + 1}번 째 슬롯은 잠겨있습니다. 활성화 된 슬롯을 선택하세요.");
            return;
        }

        // _placeCompanionItem이 활성화되어 있고, 교체할 스킬이 있는 상태인지 확인
        if (_placeCompanionItem.gameObject.activeSelf && SelectSkillInfo != null)
        {
            // 기존 슬롯의 스킬을 교체하고 슬롯 UI를 업데이트
            Managers.Backend.GameData.SkillInventory.UnEquipSkill(skillSlot.SkillInfoData.DataTemplateID, unEquipResult =>
            {
                if (unEquipResult < 0)
                {
                    Debug.LogWarning($"스킬 해제에 실패했습니다. DataTemplateID: {skillSlot.SkillInfoData.DataTemplateID}");
                    return;
                }

                // 기존 스킬 해제 후 새 스킬 장착
                Managers.Backend.GameData.SkillInventory.EquipSkill(SelectSkillInfo.DataTemplateID, equipResult =>
                {
                    if (equipResult >= 0)
                    {
                        (Managers.UI.SceneUI as UI_GameScene)._equipSkillSlotList[equipResult].RefreshUI();
                        _skillSlotList[slotIndex].RefreshUI();
                        SetEquipButtonState(true);

                        _placeCompanionItem.gameObject.SetActive(false);
                        GetObject((int)GameObjects.Content_Skill).SetActive(true);
                        foreach (var slot in Managers.Backend.GameData.SkillInventory.SkillSlotList.Where(slot => slot.SlotType == ESkillSlotType.Equipped))
                        {
                            int index = slot.Index; // 슬롯의 인덱스 가져오기
                            _skillSlotList[index].EnableButton(true);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"슬롯 {slotIndex}에 스킬 배치에 실패했습니다. DataTemplateID: {SelectSkillInfo.DataTemplateID}");
                    }
                });
            });
        }
    }

    private void OnEnhanceSkill()
    {

    }

    private void OnAutoEquipSkill()
    {

    }

    private void OnBatchEnhanceSkill()
    {

    }

    private void SetEquipButtonState(bool isEquipped)
    {
        GetButton((int)Buttons.Btn_Equip).gameObject.SetActive(!isEquipped);
        GetButton((int)Buttons.Btn_UnEquip).gameObject.SetActive(isEquipped);
    }

    private bool IsSlotFull()
    {
        return Managers.Backend.GameData.SkillInventory.SkillSlotList.All(slot => slot.SlotType != ESkillSlotType.None);
    }


    private bool IsSkillOwned(SkillInfoData skillInfo)
    {
        // 해당 스킬이 인벤토리에 있는지 확인
        return Managers.Backend.GameData.SkillInventory.SkillInventoryDic.ContainsKey(skillInfo.DataTemplateID);
    }

    private bool AreAllSlotsLocked()
    {
        // 모든 슬롯이 잠금 상태인지 확인
        return Managers.Backend.GameData.SkillInventory.SkillSlotList.All(slot => slot.SlotType == ESkillSlotType.Lock);
    }

}
