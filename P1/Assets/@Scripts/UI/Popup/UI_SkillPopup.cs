using System.Collections;
using System.Collections.Generic;
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
        Button_SkillSlot_1,
        Button_SkillSlot_2,
        Button_SkillSlot_3,
        Button_SkillSlot_4,
        Button_SkillSlot_5,
        Button_SkillSlot_6,

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
        Text_SkillDesc
    }

    public enum Sliders
    {
        Slider_SkillCount
    }

    private UI_CompanionItem _companionItem;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        GetObject((int)GameObjects.BG).BindEvent(() =>
        {
            (Managers.UI.SceneUI as UI_GameScene).CloseDrawPopup(this);

        }, EUIEvent.Click);

        _companionItem = Util.FindChild<UI_CompanionItem>(gameObject, "UI_CompanionItem", true);
        return true;
    }

    public void SetInfo()
    {

    }

    public void RefreshUI()
    {

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
