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
        Image_EggCraftItemIcon,
        Image_Lock
    }

    public enum Texts
    {
        Text_PetName,
        Text_PetLevel,
        Text_Amount,
        Text_Lock
    }

    public enum Sliders
    {
        Slider_Amount
    }

    private EPetType _petType;
    private Button _button;

    private PetData _petData;
    private PetInfoData _petInfoData;

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
            if(_petData == null || _petInfoData == null)
            {
                Debug.LogWarning("Data is Null");
                return;
            }
            Managers.UI.ShowPopupUI<UI_PetInfoPopup>().RefreshUI(_petData, _petInfoData);
        });

        RefreshUI();
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

        if(Init() == false)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        _petData = Managers.Data.PetChart[_petType];
        if(_petData == null)
        {
            Debug.LogWarning("펫 데이터가 없음");
            return;
        }
        // 챕터 검사
        int currentChapter = Managers.Scene.GetCurrentScene<GameScene>().ChapterLevel;
        GetImage((int)Images.Image_Lock).gameObject.SetActive(currentChapter != _petData.ChapterLevel);
        GetTMPText((int)Texts.Text_Lock).text = $"챕터 {_petData.ChapterLevel}에서 획득가능";

        //TODO LEVEL
        _petInfoData = Managers.Backend.GameData.PetInventory.PetInventoryDic[_petData.PetType.ToString()];
        int currentCount = _petInfoData.Count;
        int maxCount = _petInfoData.Level * _petData.MaxCount;

        GetTMPText((int)Texts.Text_PetLevel).text = $"Lv. {_petInfoData.Level}";
        GetTMPText((int)Texts.Text_PetName).text = _petData.PetName;
        GetTMPText((int)Texts.Text_Amount).text = $"{currentCount} / {maxCount}";
        GetSlider((int)Sliders.Slider_Amount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Amount).value = currentCount;

    }
}
