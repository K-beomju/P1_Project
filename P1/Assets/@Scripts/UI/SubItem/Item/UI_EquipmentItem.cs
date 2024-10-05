using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BackendData.GameData.EquipmentInventory;
using static Define;


public class UI_EquipmentItem : UI_Base
{


    public enum Images
    {
        Image_Equipment,
        Image_Fade,
        Image_Unowned,
        BG_Rare
    }
    private Image _equipmentImage;
    private Image _fadeImage;
    public Button _equipmentButton { get; private set; }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        _equipmentImage = GetImage((int)Images.Image_Equipment);
        _fadeImage = GetImage((int)Images.Image_Fade);
        _equipmentButton = GetComponent<Button>();
        return true;
    }


    public void SetDrawInfo(EquipmentInfoData equipmentData)
    {
        GetImage((int)Images.Image_Unowned).gameObject.SetActive(false);
        
        _fadeImage.color = Color.white;
        _fadeImage.DOFade(0, 0.1f);

         GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(equipmentData.Data.RareType);
        _equipmentImage.sprite =  Managers.Resource.Load<Sprite>($"Sprites/{equipmentData.Data.SpriteKey}");
    }

    public void SetEquipmentInfo(EquipmentInfoData equipmentData)
    {
        _fadeImage.gameObject.SetActive(false);
        _equipmentButton.onClick.RemoveAllListeners();
        _equipmentButton.onClick.AddListener(() => Managers.Event.TriggerEvent(EEventType.EquipmentItemClick, equipmentData));

        bool isOwned = equipmentData.OwningState == EOwningState.Owned;
        GetImage((int)Images.Image_Unowned).gameObject.SetActive(!isOwned);
        GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(equipmentData.Data.RareType);

        _equipmentImage.sprite =  Managers.Resource.Load<Sprite>($"Sprites/{equipmentData.Data.SpriteKey}");

    }
}
