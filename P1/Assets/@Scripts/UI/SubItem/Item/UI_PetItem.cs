using BackendData.GameData;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_PetItem : UI_Base
{
    public enum Images
    {
        Image_PetIcon,
        Image_EggCraftItemIcon
    }

    public enum Texts
    {
        Text_PetName,
        Text_PetLevel,
        Text_Amount
    }

    public enum Sliders
    {
        Slider_Amount
    }

    private EPetType _petType;
    private Button _button;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => 
        {

        });
        return true;

    }

    private void OnEnable() {
        Managers.Event.AddEvent(EEventType.PetItemUpdated, new Action(RefreshUI));
    }

    private void OnDisable() {
        Managers.Event.RemoveEvent(EEventType.PetItemUpdated, new Action(RefreshUI));
    }

    public void SetInfo(EPetType petType)
    {
        _petType = petType;
    }

    public void RefreshUI()
    {
        PetData petData = Managers.Data.PetChart[_petType];
        if(petData == null)
        {
            Debug.LogWarning("펫 데이터가 없음");
            return;
        }

        //TODO LEVEL
        PetInfoData petInfoData = Managers.Backend.GameData.PetInventory.PetInventoryDic[petData.PetType.ToString()];
        int currentCount = petInfoData.Count;
        int maxCount = petInfoData.Level * petData.MaxCount;

        GetTMPText((int)Texts.Text_PetLevel).text = $"Lv. {petInfoData.Level}";
        GetTMPText((int)Texts.Text_PetName).text = petData.PetName;
        GetTMPText((int)Texts.Text_Amount).text = $"{currentCount} / {maxCount}";
        GetSlider((int)Sliders.Slider_Amount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Amount).value = currentCount;

    }
}
