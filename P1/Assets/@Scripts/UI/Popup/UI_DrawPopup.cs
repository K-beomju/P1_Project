using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_DrawPopup : UI_Popup
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
        Btn_DrawTenAd,
        Btn_DrawTen,
        Btn_DrawThirty,
        Btn_Sword,
        Btn_Armor,
        Btn_Ring
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
    private EEquipmentType _type = EEquipmentType.None;

    private Color _selectedColor;
    private Image _portalImage;

    private int _drawlevel;
    private int _totalCount;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
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
            { EEquipmentType.Weapon,   Managers.Resource.Load<Sprite>($"Sprites/PortalWeaponIcon") },
            { EEquipmentType.Armor,  Managers.Resource.Load<Sprite>($"Sprites/PortalArmorIcon") },
            { EEquipmentType.Ring,  Managers.Resource.Load<Sprite>($"Sprites/PortalRingIcon") }
        };
        _portalImage = GetImage((int)Images.Image_PortalEquipment);

        GetButton((int)Buttons.Btn_DrawTenAd).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawEquipment(30));
        GetButton((int)Buttons.Btn_Sword).onClick.AddListener(() => OnSelectEquipmentIcon(EEquipmentType.Weapon));
        GetButton((int)Buttons.Btn_Armor).onClick.AddListener(() => OnSelectEquipmentIcon(EEquipmentType.Armor));
        GetButton((int)Buttons.Btn_Ring).onClick.AddListener(() => OnSelectEquipmentIcon(EEquipmentType.Ring));

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.DrawUIUpdated, new Action(UpdateUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.DrawUIUpdated, new Action(UpdateUI));
    }


    public void RefreshUI()
    {
        if (_init == false)
            return;

        OnSelectEquipmentIcon(EEquipmentType.Weapon);
    }

    public void UpdateUI()
    {
        _equipmentData = Managers.Game.PlayerGameData.DrawData[_type];

        if (_equipmentData == null)
        {
            Debug.LogWarning("장비 게임 데이터가 없음");
            return;
        }
        _drawlevel = _equipmentData.Level;
        _totalCount = _equipmentData.DrawCount;

        GetText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {_drawlevel}";
        GetText((int)Texts.Text_DrawValue).text = $"{_totalCount} / {Managers.Data.GachaDataDic[_drawlevel].MaxExp}";

        GetSlider((int)Sliders.Slider_DrawCount).value = _totalCount;
        GetSlider((int)Sliders.Slider_DrawCount).maxValue = Managers.Data.GachaDataDic[_drawlevel].MaxExp;
    }


    public void OnDrawEquipment(int drawCount)
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DrawResultPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_RESULTPOPUP);

        List<int> equipmentIdList = Util.GetEquipmentDrawResults(_type, drawCount, _drawlevel);

        for (int i = 0; i < equipmentIdList.Count; i++)
        {
            Managers.Equipment.AddEquipment(equipmentIdList[i]);
        }

        popupUI.RefreshUI(_type, drawCount, equipmentIdList);
    }


    // 어떤 타입의 장비를 뽑는지 설정 
    public void OnSelectEquipmentIcon(EEquipmentType type)
    {
        foreach (var icon in _iconImages)
        {
            icon.Value.color = Color.white;
        }
        _iconImages[type].color = _selectedColor;
        _portalImage.sprite = _portalEqIconDic[type];
        _type = type;
        UpdateUI();
    }




}
