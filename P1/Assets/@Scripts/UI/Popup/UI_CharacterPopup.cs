using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterPopup : UI_Popup
{
    public enum GameObjects
    {
        Panel_Character,
        Panel_Equipment,
        Panel_Attribute,
        Panel_Relics
    }

    public enum Buttons 
    {
        Btn_Character,
        Btn_Equipment,
        Btn_Attribute,
        Btn_Relics
    }

    public enum UI_HeroGrowthInvenSlots
    {
        UI_HeroGrowthInvenSlot_Atk,
        UI_HeroGrowthInvenSlot_Hp
    }

    public enum CharacterTab
    {
        None = -1,
        Character,
        Equipment,
        Attribute,
        Relics
    }

    private CharacterTab _tab = CharacterTab.None;
    private Dictionary<CharacterTab, (Button, GameObject)> _characterPanels;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        Bind<UI_HeroGrowthInvenSlot>(typeof(UI_HeroGrowthInvenSlots));


        // 버튼과 패널을 함께 딕셔너리로 관리
        _characterPanels = new Dictionary<CharacterTab, (Button, GameObject)>
        {
            { CharacterTab.Character, (GetButton((int)Buttons.Btn_Character), GetObject((int)GameObjects.Panel_Character)) },
            { CharacterTab.Equipment, (GetButton((int)Buttons.Btn_Equipment), GetObject((int)GameObjects.Panel_Equipment)) },
            { CharacterTab.Attribute, (GetButton((int)Buttons.Btn_Attribute),  GetObject((int)GameObjects.Panel_Attribute)) },
            { CharacterTab.Relics, (GetButton((int)Buttons.Btn_Relics),  GetObject((int)GameObjects.Panel_Relics)) }
        };


        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Atk).SetInfo(Define.EHeroUpgradeType.Growth_Atk);
        Get<UI_HeroGrowthInvenSlot>((int)UI_HeroGrowthInvenSlots.UI_HeroGrowthInvenSlot_Hp).SetInfo(Define.EHeroUpgradeType.Growth_Hp);

        // 버튼에 클릭 이벤트 할당
        foreach (var (tab, (button, _)) in _characterPanels)
        {
            button.onClick.AddListener(() => ShowTab(tab));
        }
        return true;
    }

    public void RefreshUI()
    {
        _tab = CharacterTab.Character;
        ShowTab(_tab);
    }

    public void ShowTab(CharacterTab tab)
    {
        foreach (var (button, panel) in _characterPanels.Values)
        {
            button.interactable = true;
            panel.SetActive(false);
        }
        var (selectedButton, selectedPanel) = _characterPanels[tab];
        selectedButton.interactable = false;
        selectedPanel.SetActive(true);

        
    }

    // private void CreateCompanionSlot(int defaultCount = 15)
    // {
    //     var slot = GetObject((int)GameObjects.Content);
    //     for (int i = 0; i < defaultCount; i++)
    //     {
    //         Managers.UI.MakeSubItem<UI_CompanionSlot>(slot.transform);
    //     }
    // }
}
