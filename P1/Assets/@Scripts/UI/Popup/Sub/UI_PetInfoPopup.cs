using BackendData.GameData;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_PetInfoPopup : UI_Popup
{
    public enum Images
    {
        Image_PetIcon
    }

    public enum Texts
    {
        Text_PetName,
        Text_PetLevel,
        Text_OwnedDesc,
        Text_Amount
    }

    public enum Buttons
    {
        Btn_Equip,
        Btn_UnEquip,
        Btn_Enhance,
        Btn_Make,
        Btn_Exit
    }

    public enum Sliders
    {
        Slider_Amount
    }

    public enum GameObjects
    {
        BtnGroup_Owned,
        BtnGroup_Unowned
    }

    private PetData _petData;
    private PetInfoData _petInfoData;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindSliders(typeof(Sliders));
        BindObjects(typeof(GameObjects));

        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(OnClickEquipButton);
        GetButton((int)Buttons.Btn_UnEquip).onClick.AddListener(OnClickUnEquipButton);
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(OnClickEnhanceButton);
        GetButton((int)Buttons.Btn_Make).onClick.AddListener(OnClickMakeButton);
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() =>
        {
            Managers.UI.GetOpenedPopup<UI_PetPopup>().RefreshUI();
            ClosePopupUI();
        });
        return true;
    }

    private void OnClickEquipButton()
    {
        Managers.Backend.GameData.PetInventory.EquipPet(_petData.PetType);
        RefreshUI(_petData, _petInfoData);
    }

    private void OnClickUnEquipButton()
    {
        Managers.Backend.GameData.PetInventory.UnEquipPet(_petData.PetType);
        RefreshUI(_petData, _petInfoData);
    }


    private void OnClickEnhanceButton()
    {
        int maxCount = _petInfoData.Level * _petData.MaxCount;
        Managers.Backend.GameData.PetInventory.PetLevelUp(_petData.PetType, maxCount);
        RefreshUI(_petData, _petInfoData);
    }

    private void OnClickMakeButton()
    {
        Managers.Backend.GameData.PetInventory.MakePet(_petData.PetType);
        Managers.Backend.GameData.PetInventory.AddPetCraft(_petData.PetType, -_petData.MaxCount);
        Debug.Log(_petData.MaxCount);
        RefreshUI(_petData, _petInfoData);
    }

    public void RefreshUI(PetData petData, PetInfoData petInfoData)
    {
        _petData = petData;
        _petInfoData = petInfoData;

        int currentCount = petInfoData.Count;
        int maxCount = petInfoData.Level * petData.MaxCount;

        GetImage((int)Images.Image_PetIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{Managers.Data.PetChart[petData.PetType].PetEggSpriteKey}"); 
        GetTMPText((int)Texts.Text_PetLevel).text = $"Lv. {petInfoData.Level}";
        GetTMPText((int)Texts.Text_PetName).text = petData.PetName;
        GetTMPText((int)Texts.Text_Amount).text = $"{currentCount} / {maxCount}";
        GetSlider((int)Sliders.Slider_Amount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Amount).value = currentCount;

        GetObject((int)GameObjects.BtnGroup_Owned).SetActive(petInfoData.OwningState == EOwningState.Owned);
        GetObject((int)GameObjects.BtnGroup_Unowned).SetActive(petInfoData.OwningState == EOwningState.Unowned);
        GetButton((int)Buttons.Btn_Make).interactable = IsMakePet(petData, petInfoData);
        GetButton((int)Buttons.Btn_Enhance).interactable = IsMakePet(petData, petInfoData);
        GetButton((int)Buttons.Btn_Equip).gameObject.SetActive(!Managers.Backend.GameData.PetInventory.IsEquipPet(petData.PetType));
        GetButton((int)Buttons.Btn_UnEquip).gameObject.SetActive(Managers.Backend.GameData.PetInventory.IsEquipPet(petData.PetType));

    }

    private bool IsMakePet(PetData petData, PetInfoData petInfoData)
    {
        if(petInfoData.Count >= Managers.Data.PetChart[petData.PetType].MaxCount * petInfoData.Level)
        {
            return true;
        }

        return false;
    }
}
