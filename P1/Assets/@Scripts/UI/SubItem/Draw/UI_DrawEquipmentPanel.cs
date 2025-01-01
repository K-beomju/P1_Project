using BackendData.GameData;
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
        Text_DrawEquipmentLevel,
        Text_DrawValue,
        Text_AdWatchedDrawEquipmentCount
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

    public enum Toggles
    {
        Toggle_DrawDirection
    }

    private Dictionary<EDrawType, Image> _iconImages;
    private Dictionary<EDrawType, Sprite> _portalEqIconDic;

    private DrawData _drawData;
    private EDrawType _drawType = EDrawType.Skill;
    private EAdRewardType _rewardType;


    private Image _portalImage;

    private int _drawLevel;
    private int _totalCount;
    private bool _drawDirection = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        Bind<Toggle>(typeof(Toggles));

        _iconImages = new Dictionary<EDrawType, Image>
        {
            { EDrawType.Weapon, GetImage((int)Images.BG_IconSword) },
            { EDrawType.Armor, GetImage((int)Images.BG_IconArmor) },
            { EDrawType.Ring, GetImage((int)Images.BG_IconRing) }
        };

        _portalEqIconDic = new Dictionary<EDrawType, Sprite>
        {
            { EDrawType.Weapon,   Managers.Resource.Load<Sprite>($"Sprites/WeaponIcon") },
            { EDrawType.Armor,  Managers.Resource.Load<Sprite>($"Sprites/Armor/Armor_24") },
            { EDrawType.Ring,  Managers.Resource.Load<Sprite>($"Sprites/RingIcon") }
        };
        _portalImage = GetImage((int)Images.Image_PortalEquipment);

        GetButton((int)Buttons.Btn_GachaProbability).onClick.AddListener(() => ShowProbabilityPopup());

        GetButton((int)Buttons.Btn_DrawTenAd).onClick.AddListener(() =>
        {
            if (Managers.Backend.GameData.ShopData.IsCheckWatch(_rewardType))
            {
                Managers.Ad.ShowRewardedInterstitialAd(onRewardEarned =>
                {
                    if (onRewardEarned)
                    {
                        StartCoroutine(Managers.Ad.ExecuteAfterFrame(() => 
                        {
                            // 광고 시청 처리
                            Managers.Backend.GameData.ShopData.WatchAd(_rewardType);

                            // 보상 지급 
                            OnDrawEquipment(10);

                            // UI 업데이트 
                            RefreshUI();
                        }));
                    }
                    else
                    {
                        Debug.LogWarning("광고 시청 실패!");
                    }
                });
            }
            else
            {
                ShowAlertUI("광고 시청 횟수가 모두 소진되었습니다");
            }
        });
        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() =>
        {
            int price = DrawPrice.DrawTenPrice;
            if(CanDraw(price))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, -price);
                OnDrawEquipment(10);
            }
            else
            ShowAlertUI("다이아가 부족합니다");
        });
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => 
        {
            int price = DrawPrice.DrawThirtyPrice;
            if(CanDraw(price))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, -price);
                OnDrawEquipment(30);
            }
            else
            ShowAlertUI("다이아가 부족합니다");
        });

        GetButton((int)Buttons.Btn_Sword).onClick.AddListener(() => OnClickButton(EDrawType.Weapon, EAdRewardType.DrawWeapon));
        GetButton((int)Buttons.Btn_Armor).onClick.AddListener(() => OnClickButton(EDrawType.Armor, EAdRewardType.DrawArmor));
        GetButton((int)Buttons.Btn_Ring).onClick.AddListener(() => 
        {
            ShowAlertUI("컨텐츠 준비중입니다");
            //OnClickButton(EDrawType.Ring, EAdRewardType.DrawRing);
        });

        // 버튼 클릭 시 Toggle의 값을 변경합니다.
        Toggle drawDirectionToggle = Get<Toggle>((int)Toggles.Toggle_DrawDirection);
        GetButton((int)Buttons.Btn_SkipDrawVisual).onClick.AddListener(() => drawDirectionToggle.isOn = !drawDirectionToggle.isOn);
        drawDirectionToggle.onValueChanged.AddListener((bool isOn) => _drawDirection = isOn);

        _drawType = EDrawType.Weapon;
        _rewardType = EAdRewardType.DrawWeapon;
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

    void OnClickButton(EDrawType type, EAdRewardType rewardType)
    {
        if (_drawType == type)
            return;

        _drawType = type;
        _rewardType = rewardType;
        RefreshUI();
    }


    public void RefreshUI()
    {
        foreach (var icon in _iconImages)
        {
            icon.Value.color = Util.HexToColor("#848484");
        }
        _iconImages[_drawType].color = Color.white;

        _portalImage.sprite = _portalEqIconDic[_drawType];

        UpdateEquipmentUI();
    }

    public void UpdateEquipmentUI()
    {
        _drawData = Managers.Backend.GameData.DrawLevelData.DrawDic[_drawType.ToString()];

        if (_drawData == null)
        {
            Debug.LogWarning("장비 게임 데이터가 없음");
            return;
        }
        _drawLevel = _drawData.DrawLevel;
        _totalCount = _drawData.DrawCount;

        GetTMPText((int)Texts.Text_DrawEquipmentLevel).text = $"{Util.GetDrawTypeString(_drawType)} 뽑기 Lv. {_drawLevel}";

        // 최고 레벨이라면
        if (_drawLevel == 10)
        {
            GetTMPText((int)Texts.Text_DrawValue).text = "최고레벨 도달";
            GetSlider((int)Sliders.Slider_DrawCount).maxValue = 0;
            GetSlider((int)Sliders.Slider_DrawCount).value = 0;
        }
        else
        {
            GetTMPText((int)Texts.Text_DrawValue).text = $"{_totalCount} / {Managers.Data.DrawEquipmentChart[_drawLevel].MaxExp}";

            GetSlider((int)Sliders.Slider_DrawCount).maxValue = Managers.Data.DrawEquipmentChart[_drawLevel].MaxExp;
            GetSlider((int)Sliders.Slider_DrawCount).value = _totalCount;
        }


        RewardAdData rewardData =  Managers.Backend.GameData.ShopData.RewardAdDic[_rewardType.ToString()];
        GetTMPText((int)Texts.Text_AdWatchedDrawEquipmentCount).text =
        $"({rewardData.WatchedCount}/{rewardData.MaxCount})";
    }



    #region Draw Logic

    private void OnDrawEquipment(int drawCount)
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DrawResultPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_RESULTPOPUP);

        var equipmentIdList = Util.GetDrawSystemResults(_drawType, drawCount, _drawLevel);
        popupUI.RefreshUI(_drawType, drawCount, equipmentIdList, _drawDirection);
    }

    private void ShowProbabilityPopup()
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DrawProbabilityPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
        popupUI.RefreshUI(_drawType);

    }

    bool CanDraw(float cost)
    {
        if (!Managers.Backend.GameData.CharacterData.PurseDic.TryGetValue(EItemType.Dia.ToString(), out float amount))
            return false;

        return amount >= cost;
    }

    #endregion

}
