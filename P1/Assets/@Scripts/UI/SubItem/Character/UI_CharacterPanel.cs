using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackendData.GameData;
using static Define;

public class UI_CharacterPanel : UI_Base
{
    public enum Texts 
    {
        Text_TotalPower
    }

    public enum Buttons
    {
        Btn_RingSlot,
        Btn_ArmorSlot,
        Btn_WeaponSlot
    }

    public enum EquipmentItems
    {
        UI_EquipmentItem_Ring,
        UI_EquipmentItem_Armor,
        UI_EquipmentItem_Weapon
    }

    public enum UI_CharacterGrowthSlots
    {
        UI_CharacterGrowthInvenSlot_Atk,
        UI_CharacterGrowthInvenSlot_Hp,
        UI_CharacterGrowthInvenSlot_Recovery,
        UI_CharacterGrowthInvenSlot_CriRate,
        UI_CharacterGrowthInvenSlot_CriDmg
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        Bind<UI_CharacterGrowthInvenSlot>(typeof(UI_CharacterGrowthSlots));
        Bind<UI_CompanionItem>(typeof(EquipmentItems));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_Atk).SetInfo(EHeroUpgradeType.Growth_Atk);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_Hp).SetInfo(EHeroUpgradeType.Growth_Hp);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_Recovery).SetInfo(EHeroUpgradeType.Growth_Recovery);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_CriRate).SetInfo(EHeroUpgradeType.Growth_CriRate);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_CriDmg).SetInfo(EHeroUpgradeType.Growth_CriDmg);

        GetButton((int)Buttons.Btn_RingSlot).onClick.AddListener(() => HandleEquipmentPopup(EEquipmentType.Ring));
        GetButton((int)Buttons.Btn_ArmorSlot).onClick.AddListener(() => HandleEquipmentPopup(EEquipmentType.Armor));
        GetButton((int)Buttons.Btn_WeaponSlot).onClick.AddListener(() => HandleEquipmentPopup(EEquipmentType.Weapon));

        foreach (EquipmentItems item in Enum.GetValues(typeof(EquipmentItems)))
        {
            var equipmentItem = Get<UI_CompanionItem>((int)item);
            if (equipmentItem != null)
            {
                equipmentItem.gameObject.SetActive(false);
            }
        }
        return true;
    }

    private void OnEnable() 
    {
        Managers.Event.AddEvent(EEventType.HeroTotalPowerUpdated, new Action(ShowTotalPowerText));    
    }

    private void OnDisable() 
    {
        Managers.Event.RemoveEvent(EEventType.HeroTotalPowerUpdated, new Action(ShowTotalPowerText));    
    }

    public void RefreshUI()
    {
        UpdateEquipmentSlot();
        ShowTotalPowerText();
    }

    // 전투력 표시 텍스트 함수 
    private void ShowTotalPowerText()
    {
        GetTMPText((int)Texts.Text_TotalPower).text = Util.ConvertToTotalCurrency((long)Managers.Hero.PlayerHeroInfo.CurrentTotalPower);
    }

    // 장비 슬롯 클릭시 장비 팝업으로 이동하는 함수 
    private void HandleEquipmentPopup(EEquipmentType equipmentType)
    {
        (Managers.UI.SceneUI as UI_GameScene).ShowTab(UI_GameScene.PlayTab.Equipment);
        Managers.UI.ShowPopupUI<UI_EquipmentPopup>().SetInfo(equipmentType);
    }

    // 장비 슬롯 업데이트 함수 
    public void UpdateEquipmentSlot()
    {
        UpdateEquipmentUI(EEquipmentType.Weapon, EquipmentItems.UI_EquipmentItem_Weapon);
        UpdateEquipmentUI(EEquipmentType.Armor, EquipmentItems.UI_EquipmentItem_Armor);
        UpdateEquipmentUI(EEquipmentType.Ring, EquipmentItems.UI_EquipmentItem_Ring);
    }

    private void UpdateEquipmentUI(EEquipmentType type, EquipmentItems uiItem)
    {
        var equipmentItem = Get<UI_CompanionItem>((int)uiItem);

        //만약에 장착된 장비가 있다면
        if (Managers.Backend.GameData.EquipmentInventory.EquipmentEquipDic.TryGetValue(type.ToString(), out EquipmentInfoData equipmentInfoData))
        {
            equipmentItem.gameObject.SetActive(true);
            equipmentItem.DisplayItem(equipmentInfoData, EItemDisplayType.ImageOnly);

        }
        else
        {
            equipmentItem.gameObject.SetActive(false);
        }
    }
}
