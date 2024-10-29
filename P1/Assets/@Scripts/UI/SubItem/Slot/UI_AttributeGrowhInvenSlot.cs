using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_AttributeGrowhInvenSlot : UI_Base
{
    public enum Texts
    {
        Text_AbLevel
    }

    public enum Images
    {
        Image_AbIcon
    }

    private TMP_Text _levelText;
    private Image _iconImage;
    private Button _slotButton;
    private EHeroAttrType _heroAttrType;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        _levelText = GetTMPText((int)Texts.Text_AbLevel);
        _iconImage = GetImage((int)Images.Image_AbIcon);
        _slotButton = GetComponent<Button>();
        UpdateSlotInfoUI();
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.HeroAttributeUpdated, new Action(UpdateSlotInfoUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.HeroAttributeUpdated, new Action(UpdateSlotInfoUI));
    }

    public void SetInfo(EHeroAttrType attrType)
    {
        _heroAttrType = attrType;

        if (Init() == false)
        {
            UpdateSlotInfoUI();
        }
    }

    public void UpdateSlotInfoUI()
    {
        if (!Managers.Backend.GameData.UserData.UpgradeAttrDic.TryGetValue(_heroAttrType.ToString(), out int level))
        {
            Debug.LogWarning($"UpdateSlotInfoUI도중 {level}이 없습니다");
            return;
        }

        Data.HeroAttributeInfoData attriData = Managers.Data.HeroAttributeChart[_heroAttrType];

        if (attriData == null)
        {
            Debug.LogWarning($"HeroAttributeInfoData가 없습니다");
            return;
        }

        if (_iconImage.sprite == null)
            _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{attriData.SpriteKey}");


        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(() => Managers.Event.TriggerEvent(EEventType.AttributeItemClick, _heroAttrType));

        _levelText.text = $"Lv {level}";
    }
}
