using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
public class UI_FadeInBase : UI_Base
{
    public enum Images
    {
        Image_Fade
    }

    private Image fadeImage; 

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        fadeImage = GetImage((int)Images.Image_Fade);
        return true;
    }

    public void ShowFadeIn(float time, Action CallBack = null)
    {
        fadeImage.DOFade(1,0);
        fadeImage.DOFade(0, time).OnComplete(() => 
        {
            CallBack?.Invoke();
            Managers.Resource.Destroy(this.gameObject);
        });
    }
}
