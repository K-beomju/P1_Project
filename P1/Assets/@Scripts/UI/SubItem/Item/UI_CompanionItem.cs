using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BackendData.GameData;
using static Define;


public class UI_CompanionItem : UI_Base
{


    public enum Images
    {
        Image_Icon,
        Image_Fade,
        Image_Unowned,
        BG_Rare
    }

    public enum Texts 
    {
        Text_Level
    }

    private Image _iconImage;
    private Image _fadeImage;
    public Button _companionButton { get; private set; }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));

        _iconImage = GetImage((int)Images.Image_Icon);
        _fadeImage = GetImage((int)Images.Image_Fade);
        _companionButton = GetComponent<Button>();
        return true;
    }


    public void SetDrawInfo(EquipmentInfoData equipmentData)
    {
        GetImage((int)Images.Image_Unowned).gameObject.SetActive(false);
        GetTMPText((int)Texts.Text_Level).gameObject.SetActive(false);

        _fadeImage.color = Color.white;
        _fadeImage.DOFade(0, 0.1f);

         GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(equipmentData.Data.RareType);
        _iconImage.sprite =  Managers.Resource.Load<Sprite>($"Sprites/{equipmentData.Data.SpriteKey}");
    }

    public void SetEquipmentInfo(EquipmentInfoData equipmentData, bool showLvText = true)
    {
        _fadeImage.gameObject.SetActive(false);
        _companionButton.onClick.RemoveAllListeners();
        _companionButton.onClick.AddListener(() => Managers.Event.TriggerEvent(EEventType.EquipmentItemClick, equipmentData));

        bool isOwned = equipmentData.OwningState == EOwningState.Owned;
        GetImage((int)Images.Image_Unowned).gameObject.SetActive(!isOwned);
        GetTMPText((int)Texts.Text_Level).gameObject.SetActive(showLvText && isOwned);
        if(isOwned)
            GetTMPText((int)Texts.Text_Level).text = $"Lv. {equipmentData.Level}";

        
        GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(equipmentData.Data.RareType);

        _iconImage.sprite =  Managers.Resource.Load<Sprite>($"Sprites/{equipmentData.Data.SpriteKey}");

    }
}
