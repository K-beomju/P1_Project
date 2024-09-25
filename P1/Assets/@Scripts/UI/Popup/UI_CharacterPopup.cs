using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_CharacterPopup : UI_Popup
{
    public enum GameObjects
    {
        Character,
        Attribute,
        Relics
    }

    public enum CharacterInven
    {
        None = -1,
        Character,
        Attribute,
        Relics
    }

    public enum Buttons
    {
        Btn_RingSlot,
        Btn_ArmorSlot,
        Btn_WeaponSlot,

        Btn_Character,
        Btn_Attribute,
        Btn_Relics
    }

    public enum Texts
    {
        Text_CharacterTitle
    }

    public enum EquipmentItems
    {
        UI_EquipmentItem_Ring,
        UI_EquipmentItem_Armor,
        UI_EquipmentItem_Weapon
    }


    private CharacterInven _inven = CharacterInven.None;
    private Dictionary<CharacterInven, (Button, GameObject)> _characterPanels;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        Bind<UI_EquipmentItem>(typeof(EquipmentItems));

        GetButton((int)Buttons.Btn_RingSlot).onClick.AddListener(() => HandleEquipmentPopup(EEquipmentType.Ring));
        GetButton((int)Buttons.Btn_ArmorSlot).onClick.AddListener(() => HandleEquipmentPopup(EEquipmentType.Armor));
        GetButton((int)Buttons.Btn_WeaponSlot).onClick.AddListener(() => HandleEquipmentPopup(EEquipmentType.Weapon));

        // 버튼과 패널을 함께 딕셔너리로 관리
        _characterPanels = new Dictionary<CharacterInven, (Button, GameObject)>
        {
            { CharacterInven.Character, (GetButton((int)Buttons.Btn_Character), GetObject((int)GameObjects.Character)) },
            { CharacterInven.Attribute, (GetButton((int)Buttons.Btn_Attribute),  GetObject((int)GameObjects.Attribute)) },
            { CharacterInven.Relics, (GetButton((int)Buttons.Btn_Relics),  GetObject((int)GameObjects.Relics)) }
        };

        // 버튼에 클릭 이벤트 할당
        foreach (var (inven, (button, _)) in _characterPanels)
        {
            button.onClick.AddListener(() => ShowTab(inven));
        }

        foreach (EquipmentItems item in Enum.GetValues(typeof(EquipmentItems)))
        {
            var equipmentItem = Get<UI_EquipmentItem>((int)item);
            if (equipmentItem != null)
            {
                equipmentItem.gameObject.SetActive(false);
            }
        }
        return true;
    }


    public void RefreshUI()
    {
        _inven = CharacterInven.Character;
        ShowTab(_inven);
        UpdateEquipmentSlot();
    }

    public void ShowTab(CharacterInven inven)
    {
        foreach (var (button, panel) in _characterPanels.Values)
        {
            button.interactable = true;
            panel.SetActive(false);
        }
        var (selectedButton, selectedPanel) = _characterPanels[inven];
        selectedButton.interactable = false;
        selectedPanel.SetActive(true);
        GetText((int)Texts.Text_CharacterTitle).text = GetCharacterTabString(inven);

    }

    private string GetCharacterTabString(CharacterInven inven)
    {
        return inven switch
        {
            CharacterInven.Character => "캐릭터",
            CharacterInven.Attribute => "특성",
            CharacterInven.Relics => "유물",
            _ => throw new ArgumentException($"Unknown rare type String: {inven}")
        };
    }

    #region Equipment Slot 

    private void HandleEquipmentPopup(EEquipmentType equipmentType)
    {
        Managers.UI.ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_EquipmentPopup>().RefreshUI(equipmentType);
        (Managers.UI.SceneUI as UI_GameScene)._tab = UI_GameScene.PlayTab.Equipment;
    }

    public void UpdateEquipmentSlot()
    {
        UpdateEquipmentUI(EEquipmentType.Weapon, EquipmentItems.UI_EquipmentItem_Weapon);
        UpdateEquipmentUI(EEquipmentType.Armor, EquipmentItems.UI_EquipmentItem_Armor);
        UpdateEquipmentUI(EEquipmentType.Ring, EquipmentItems.UI_EquipmentItem_Ring);
    }

    private void UpdateEquipmentUI(EEquipmentType type, EquipmentItems uiItem)
    {
        int equipmentId = Managers.Equipment.GetEquippedId(type);
        var equipmentItem = Get<UI_EquipmentItem>((int)uiItem);

        if (equipmentId != 0)
        {
            equipmentItem.gameObject.SetActive(true);
            equipmentItem.SetEquipmentInfo(Managers.Equipment.GetEquipmentInfo(equipmentId));
        }
        else
        {
            equipmentItem.gameObject.SetActive(false);
        }
    }

    #endregion

}
