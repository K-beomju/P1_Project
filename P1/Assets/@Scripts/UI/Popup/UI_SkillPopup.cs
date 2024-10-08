using BackendData.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using static UI_CharacterPanel;

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

    private UI_CompanionItem _companionItem;
    private List<UI_CompanionItem> _companionItems = new List<UI_CompanionItem>();

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

    private Dictionary<UI_SkillSlot, ESkillSlotType> _skillSlotDic = new Dictionary<UI_SkillSlot, ESkillSlotType>();

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

        _companionItem = Util.FindChild<UI_CompanionItem>(gameObject, "UI_CompanionItem", true);
        for (int i = 0; i < 3; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_CompanionItem>(GetObject((int)GameObjects.Content_Skill).transform);
            _companionItems.Add(item);
        }
        
        // 6개의 슬롯을 초기화하면서 각 슬롯을 Lock 타입으로 설정
        for (int i = 0; i < 6; i++)
        {
            _skillSlotDic.Add(Get<UI_SkillSlot>(i), ESkillSlotType.Lock);
        }



        return true;
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.SkillItemClick, new Action<SkillInfoData>(ShowEquipmentDetailUI));
    }

    void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.SkillItemClick, new Action<SkillInfoData>(ShowEquipmentDetailUI));
    }

    public void RefreshUI()
    {
        List<SkillInfoData> skillInfos = Managers.Skill.AllSkillInfos.Values.ToList();

        for (int i = 0; i < _companionItems.Count; i++)
        {
            _companionItems[i].SetSkillInfo(skillInfos[i]);
        }
    }

    public void ShowEquipmentDetailUI(SkillInfoData _skillInfo)
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

    }

    private void OnEquipSkill()
    {

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
}
