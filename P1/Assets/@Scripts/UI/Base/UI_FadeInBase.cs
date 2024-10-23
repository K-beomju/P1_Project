using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using static Define;

public class UI_FadeInBase : UI_Base
{
    public enum Images
    {
        Image_Fade
    }

    private Image fadeImage;
    private Canvas canvas;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        canvas = GetComponent<Canvas>();
        canvas.sortingOrder = SortingLayers.UI_POPUP;

        BindImages(typeof(Images));
        fadeImage = GetImage((int)Images.Image_Fade);
        return true;
    }

    public void ShowFadeIn(float fadeOutTime, float fadeInTime, Action fadeOutCallBack = null, Action fadeInCallBack = null)
    {
        fadeImage.DOFade(0, 0);
        fadeImage.DOFade(1, fadeOutTime).OnComplete(() =>
        {
            fadeOutCallBack?.Invoke();
            fadeImage.DOFade(0, fadeInTime).OnComplete(() =>
            {
                fadeInCallBack?.Invoke();
                gameObject.SetActive(false);
            });
        });
    }
}
