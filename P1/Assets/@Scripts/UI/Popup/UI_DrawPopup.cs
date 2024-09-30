using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_DrawPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        UI_DrawEquipmentPanel,
        UI_DrawSkillPanel
    }

    public enum Buttons
    {
        Btn_Equipment,
        Btn_Skill
    }


    public enum EDrawSection
    {
        None,
        Equipment,
        Skill
    }
    
    // State
    private EDrawSection _drawSection = EDrawSection.None;

    // Panel
    UI_DrawEquipmentPanel _drawEquipmentPanel;
    UI_DrawSkillPanel _drawSkillPanel;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        _drawEquipmentPanel = GetObject((int)GameObjects.UI_DrawEquipmentPanel).GetOrAddComponent<UI_DrawEquipmentPanel>();
        _drawSkillPanel = GetObject((int)GameObjects.UI_DrawSkillPanel).GetOrAddComponent<UI_DrawSkillPanel>();

        GetButton((int)Buttons.Btn_Equipment).onClick.AddListener(() => OnClickButton(EDrawSection.Equipment));
        GetButton((int)Buttons.Btn_Skill).onClick.AddListener(() => OnClickButton(EDrawSection.Skill));


        _drawSection = EDrawSection.Equipment;
        return true;
    }

    void OnClickButton(EDrawSection section)
    {
        if (_drawSection == section)
            return;

        _drawSection = section;
        RefreshUI();
    }

    public void RefreshUI()
    {
        GetObject((int)GameObjects.UI_DrawEquipmentPanel).SetActive(false);
        GetObject((int)GameObjects.UI_DrawSkillPanel).SetActive(false);
        GetButton((int)Buttons.Btn_Equipment).interactable = true;
        GetButton((int)Buttons.Btn_Skill).interactable = true;

        switch (_drawSection)
        {
            case EDrawSection.Equipment:
                GetButton((int)Buttons.Btn_Equipment).interactable = false;

                _drawEquipmentPanel.gameObject.SetActive(true);
                _drawEquipmentPanel.RefreshUI();
                break;
            case EDrawSection.Skill:
                GetButton((int)Buttons.Btn_Skill).interactable = false;
                _drawSkillPanel.gameObject.SetActive(true);
                break;
        }
    }




}
