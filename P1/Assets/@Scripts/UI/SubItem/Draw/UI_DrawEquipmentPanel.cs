using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_DrawEquipmentPanel : UI_Base
{
    public enum Texts
    {
        Text_DrawLevel,
        Text_DrawValue
    }

    public enum Sliders
    {
        Slider_DrawCount
    }

    public enum Buttons
    {
        Btn_GachaProbability,
        Btn_SkipDrawVisual,
        Btn_DrawTenAd,
        Btn_DrawTen,
        Btn_DrawThirty,
        Btn_Sword,
        Btn_Armor,
        Btn_Ring,
    }

    public enum Images
    {
        BG_IconSword,
        BG_IconArmor,
        BG_IconRing,
        Image_PortalEquipment
    }

    private Dictionary<EEquipmentType, Image> _iconImages;
    private Dictionary<EEquipmentType, Sprite> _portalEqIconDic;

    private EquipmentDrawData _equipmentData;
    private EEquipmentType _currentEquipmentType = EEquipmentType.None;

    private Image _portalImage;

    private int _drawLevel;
    private int _totalCount;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));

        _iconImages = new Dictionary<EEquipmentType, Image>
        {
            { EEquipmentType.Weapon, GetImage((int)Images.BG_IconSword) },
            { EEquipmentType.Armor, GetImage((int)Images.BG_IconArmor) },
            { EEquipmentType.Ring, GetImage((int)Images.BG_IconRing) }
        };

        _portalEqIconDic = new Dictionary<EEquipmentType, Sprite>
        {
            { EEquipmentType.Weapon,   Managers.Resource.Load<Sprite>($"Sprites/WeaponIcon") },
            { EEquipmentType.Armor,  Managers.Resource.Load<Sprite>($"Sprites/Armor/Armor_24") },
            { EEquipmentType.Ring,  Managers.Resource.Load<Sprite>($"Sprites/RingIcon") }
        };
        _portalImage = GetImage((int)Images.Image_PortalEquipment);
 
        GetButton((int)Buttons.Btn_GachaProbability).onClick.AddListener(() => ShowProbabilityPopup());

        GetButton((int)Buttons.Btn_DrawTenAd).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawEquipment(30));

        GetButton((int)Buttons.Btn_Sword).onClick.AddListener(() => OnClickButton(EEquipmentType.Weapon));
        GetButton((int)Buttons.Btn_Armor).onClick.AddListener(() => OnClickButton(EEquipmentType.Armor));
        GetButton((int)Buttons.Btn_Ring).onClick.AddListener(() => OnClickButton(EEquipmentType.Ring));

        _currentEquipmentType = EEquipmentType.Weapon;
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.DrawEquipmentUIUpdated, new Action(UpdateEquipmentUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.DrawEquipmentUIUpdated, new Action(UpdateEquipmentUI));
    }

    void OnClickButton(EEquipmentType type)
    {
        if (_currentEquipmentType == type)
            return;

        _currentEquipmentType = type;
        RefreshUI();
    }


    public void RefreshUI()
    {
        foreach (var icon in _iconImages)
        {
            icon.Value.color = Util.HexToColor("#848484");
        }
        _iconImages[_currentEquipmentType].color = Color.white;

        _portalImage.sprite = _portalEqIconDic[_currentEquipmentType];

        UpdateEquipmentUI();
    }

    public void UpdateEquipmentUI()
    {
        _equipmentData = Managers.Game.PlayerGameData.DrawData[_currentEquipmentType];

        if (_equipmentData == null)
        {
            Debug.LogWarning("장비 게임 데이터가 없음");
            return;
        }
        _drawLevel = _equipmentData.Level;
        _totalCount = _equipmentData.DrawCount;

        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_currentEquipmentType)} 뽑기 Lv. {_drawLevel}";
        GetTMPText((int)Texts.Text_DrawValue).text = $"{_totalCount} / {Managers.Data.GachaDataDic[_drawLevel].MaxExp}";

        GetSlider((int)Sliders.Slider_DrawCount).value = _totalCount;
        GetSlider((int)Sliders.Slider_DrawCount).maxValue = Managers.Data.GachaDataDic[_drawLevel].MaxExp;
    }



    #region Draw Logic

    private void OnDrawEquipment(int drawCount)
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DrawResultPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_RESULTPOPUP);

        var equipmentIdList = Util.GetEquipmentDrawResults(_currentEquipmentType, drawCount, _drawLevel);
        popupUI.RefreshUI(_currentEquipmentType, drawCount, equipmentIdList);
    }

    private void ShowProbabilityPopup()
    {
        Managers.UI.ShowPopupUI<UI_DrawProbabilityPopup>().RefreshUI(_currentEquipmentType);
    }

    #endregion

}
