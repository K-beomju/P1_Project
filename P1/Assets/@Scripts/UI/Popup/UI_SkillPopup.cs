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

        GetObject((int)GameObjects.BG).BindEvent(() =>
        {
            (Managers.UI.SceneUI as UI_GameScene).CloseDrawPopup(this);

        }, EUIEvent.Click);
        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(OnEquipSkill);
        GetButton((int)Buttons.Btn_UnEquip).onClick.AddListener(OnUnEquipSkill);
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(OnEnhanceSkill);
        GetButton((int)Buttons.Btn_AutoEquip).onClick.AddListener(OnAutoEquipSkill);
        GetButton((int)Buttons.Btn_BatchEnhance).onClick.AddListener(OnBatchEnhanceSkill);
        GetButton((int)Buttons.Btn_UnEquip).gameObject.SetActive(false);

        _companionItem = Util.FindChild<UI_CompanionItem>(gameObject, "UI_CompanionItem", true);
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
        var skillSlots = Managers.Backend.GameData.SkillInventory.SkillSlotList;
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (skillSlots[i] != null)
            {
                _skillSlotList[i].RefreshUI();
            }
        }

        // 스킬 인벤토리 중 스킬 장비 찾기 
        SkillInfoData equippedInfo = Managers.Backend.GameData.SkillInventory.SkillInventoryDic.Values
            .FirstOrDefault(skillinfo => skillinfo.IsEquipped);

        List<SkillInfoData> skillInfos = Managers.Skill.GetSkillInfos(true);

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
            _companionItems[i].SetSkillInfo(skillInfos[i]);
        }
    }

    public void ShowSkillDetailUI(SkillInfoData _skillInfo)
    {
        if (SelectSkillInfo == _skillInfo)
            return;

        SelectSkillInfo = _skillInfo;
        _companionItem.SetSkillInfo(SelectSkillInfo, false);
        GetTMPText((int)Texts.Text_SkillName).text = SelectSkillInfo.Data.Name;
        GetTMPText((int)Texts.Text_SkillLevel).text = $"Lv. {SelectSkillInfo.Level}";
        GetTMPText((int)Texts.Text_SkillRare).text = Util.GetRareTypeString(SelectSkillInfo.Data.RareType);

        string originalString = SelectSkillInfo.Data.Description;
        string replacedString = originalString.Replace("{value}", SelectSkillInfo.Data.UsedValue.ToString());
        GetTMPText((int)Texts.Text_SkillDesc).text = replacedString;
        GetTMPText((int)Texts.Text_SkillOwnedValue).text = $"보유효과: 공격력{SelectSkillInfo.Data.OwnedValue}% 증가";
        GetTMPText((int)Texts.Text_SkillCoolTime).text = $"{SelectSkillInfo.Data.CoolTime}초";

        int currentCount = SelectSkillInfo.Count;
        int maxCount = Util.GetUpgradeEquipmentMaxCount(SelectSkillInfo.Level);
        GetSlider((int)Sliders.Slider_SkillCount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_SkillCount).value = currentCount;
        GetTMPText((int)Texts.Text_OwendAmount).text = $"{currentCount} / {maxCount}";

        SetEquipButtonState(Managers.Backend.GameData.SkillInventory.SkillInventoryDic[_skillInfo.DataTemplateID].IsEquipped);


    }

    private void OnEquipSkill()
    {
        try
        {
            Managers.Backend.GameData.SkillInventory.EquipSkill(SelectSkillInfo.DataTemplateID, (int slotIndex) =>
            {
                if (slotIndex >= 0)
                {
                    _skillSlotList[slotIndex].RefreshUI();
                   SetEquipButtonState(true);
                }
                else
                {
                    Debug.LogWarning($"스킬 장착에 실패했습니다. DataTemplateID: {SelectSkillInfo.DataTemplateID}");
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
                    _skillSlotList[slotIndex].RefreshUI();
                    SetEquipButtonState(false);
                }
                else
                {

                }
            });
        }
        catch (Exception e)
        {
            throw new Exception($"OnUnEquipSkill({SelectSkillInfo}) 중 에러가 발생하였습니다\n{e}");
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
}
