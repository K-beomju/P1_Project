using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UI_NewPetPopup : UI_Popup
{
    public enum GameObjects
    {
        BG
    }

    public enum Texts
    {
        Text_PetName
    }

    public enum Images
    {
        Image_PetObject
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));

        GetObject((int)GameObjects.BG).BindEvent(ClosePopupUI);
        return true;
    }

    public void RefreshUI(PetData petData)
    {
        Image petImage = GetImage((int)Images.Image_PetObject);

        // 바운스 효과
        petImage.rectTransform.localScale = Vector3.zero;
        petImage.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        // 페이드 인 효과
        petImage.DOFade(0, 0).OnComplete(() => petImage.DOFade(1, 0.3f));

        // 이미지 설정
        petImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{Managers.Data.PetChart[petData.PetType].PetObjectSpriteKey}");
        GetTMPText((int)Texts.Text_PetName).text = petData.PetName;

    }
}
