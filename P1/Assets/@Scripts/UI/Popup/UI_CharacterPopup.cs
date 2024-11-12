using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_CharacterPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        UI_ChracterPanel,
        UI_AttributePanel,
        UI_RelicsPanel,
        UI_RankUpPanel
    }

    public enum Buttons
    {
        Btn_Character,
        Btn_Attribute,
        Btn_Relics,
        Btn_RankUp
    }

    public enum ECharacterSection
    {
        None = -1,
        Character,
        Attribute,
        Relics,
        RankUp
    }

    public enum Texts
    {
        Text_CharacterTitle
    }

    // State
    private ECharacterSection _characterSection = ECharacterSection.None;

    // Panel
    UI_CharacterPanel _characterPanel;
    UI_AttributePanel _attributePanel;
    UI_RelicsPanel _relicsPanel;
    UI_RankUpPanel _rankUpPanel;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        _characterPanel = GetObject((int)GameObjects.UI_ChracterPanel).GetOrAddComponent<UI_CharacterPanel>();
        _attributePanel = GetObject((int)GameObjects.UI_AttributePanel).GetOrAddComponent<UI_AttributePanel>();
        _relicsPanel = GetObject((int)GameObjects.UI_RelicsPanel).GetOrAddComponent<UI_RelicsPanel>();
        _rankUpPanel = GetObject((int)GameObjects.UI_RankUpPanel).GetOrAddComponent<UI_RankUpPanel>();

        GetObject((int)GameObjects.BG).BindEvent(() => (Managers.UI.SceneUI as UI_GameScene).ShowTab(UI_GameScene.PlayTab.Character));
        GetButton((int)Buttons.Btn_Character).onClick.AddListener(() => OnClickButton(ECharacterSection.Character));
        GetButton((int)Buttons.Btn_Attribute).onClick.AddListener(() => OnClickButton(ECharacterSection.Attribute));
        GetButton((int)Buttons.Btn_Relics).onClick.AddListener(() => OnClickButton(ECharacterSection.Relics));
        GetButton((int)Buttons.Btn_RankUp).onClick.AddListener(() => OnClickButton(ECharacterSection.RankUp));

        _characterSection = ECharacterSection.Character;
        return true;
    }

    void OnClickButton(ECharacterSection section)
    {
        if (_characterSection == section)
            return;

        _characterSection = section;
        RefreshUI();
    }


    public void RefreshUI()
    {
        _characterPanel.gameObject.SetActive(false);
        _attributePanel.gameObject.SetActive(false);
        _relicsPanel.gameObject.SetActive(false);
        _rankUpPanel.gameObject.SetActive(false);

        GetButton((int)Buttons.Btn_Character).interactable = _characterSection != ECharacterSection.Character;
        GetButton((int)Buttons.Btn_Attribute).interactable = _characterSection != ECharacterSection.Attribute;
        GetButton((int)Buttons.Btn_Relics).interactable =  _characterSection != ECharacterSection.Relics;
        GetButton((int)Buttons.Btn_RankUp).interactable =  _characterSection != ECharacterSection.RankUp;

        (Managers.UI.SceneUI as UI_GameScene).GetGoodItem(Define.EItemType.ExpPoint).gameObject.SetActive(_characterSection == ECharacterSection.Attribute);
        Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);

        switch (_characterSection)
        {
            case ECharacterSection.Character:
                _characterPanel.gameObject.SetActive(true);
                _characterPanel.RefreshUI();
                break;
            case ECharacterSection.Attribute:
                _attributePanel.gameObject.SetActive(true);
                break;
            case ECharacterSection.Relics:
                _relicsPanel.gameObject.SetActive(true);
                break;
            case ECharacterSection.RankUp:
                _rankUpPanel.gameObject.SetActive(true);
                break;
        }

        GetTMPText((int)Texts.Text_CharacterTitle).text = GetCharacterPanelString(_characterSection);
    }

    private string GetCharacterPanelString(ECharacterSection section)
    {
        return section switch
        {
            ECharacterSection.Character => "캐릭터",
            ECharacterSection.Attribute => "특성",
            ECharacterSection.Relics => "유물",
            ECharacterSection.RankUp => "승급",

            _ => throw new ArgumentException($"Unknown rare type String: {section}")
        };
    }


}
