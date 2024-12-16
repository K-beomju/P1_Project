using BackendData.GameData;
using Data;
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
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(OnClickEnhanceButton);
        GetButton((int)Buttons.Btn_Make).onClick.AddListener(OnClickMakeButton);
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        return true;
    }

    private void OnClickEquipButton()
    {

    }

    private void OnClickEnhanceButton()
    {

    }

    private void OnClickMakeButton()
    {

    }

    public void RefreshUI(PetData petData, PetInfoData petInfoData)
    {
        int currentCount = petInfoData.Count;
        int maxCount = petInfoData.Level * petData.MaxCount;

        //GetImage((int)Images.Image_PetIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{Managers.Data.PetChart[petData.PetType].PetEggSpriteKey}"); 
        GetTMPText((int)Texts.Text_PetLevel).text = $"Lv. {petInfoData.Level}";
        GetTMPText((int)Texts.Text_PetName).text = petData.PetName;
        GetTMPText((int)Texts.Text_Amount).text = $"{currentCount} / {maxCount}";
        GetSlider((int)Sliders.Slider_Amount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Amount).value = currentCount;

        GetObject((int)GameObjects.BtnGroup_Owned).SetActive(petInfoData.OwningState == EOwningState.Owned);
        GetObject((int)GameObjects.BtnGroup_Unowned).SetActive(petInfoData.OwningState == EOwningState.Unowned);
        //GetButton((int)Buttons.Btn_Equip).
    
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
