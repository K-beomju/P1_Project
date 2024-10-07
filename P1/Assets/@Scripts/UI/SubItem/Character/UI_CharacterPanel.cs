using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackendData.GameData;
using static Define;

public class UI_CharacterPanel : UI_Base
{
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

        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_Atk).SetInfo(Define.EHeroUpgradeType.Growth_Atk);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_Hp).SetInfo(Define.EHeroUpgradeType.Growth_Hp);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_Recovery).SetInfo(Define.EHeroUpgradeType.Growth_Recovery);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_CriRate).SetInfo(Define.EHeroUpgradeType.Growth_CriRate);
        Get<UI_CharacterGrowthInvenSlot>((int)UI_CharacterGrowthSlots.UI_CharacterGrowthInvenSlot_CriDmg).SetInfo(Define.EHeroUpgradeType.Growth_CriDmg);

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

    public void RefreshUI()
    {
        UpdateEquipmentSlot();
    }

    private void HandleEquipmentPopup(EEquipmentType equipmentType)
    {
        Managers.UI.ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_EquipmentPopup>().SetInfo(equipmentType);
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
        var equipmentItem = Get<UI_CompanionItem>((int)uiItem);

        //만약에 장착된 장비가 있다면
        if (Managers.Backend.GameData.EquipmentInventory.EquipmentEquipDic.TryGetValue(type.ToString(), out EquipmentInfoData equipmentInfoData))
        {
            int dataId = equipmentInfoData.DataTemplateID;
            equipmentItem.gameObject.SetActive(true);
            equipmentItem.SetEquipmentInfo(equipmentInfoData);

        }
        else
        {
            equipmentItem.gameObject.SetActive(false);
        }
    }


}
