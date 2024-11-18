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
    public bool sceneMove { get; set; } = false;
    private bool _fadeInOutComplete = true;

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

    // Fade In = 밝아짐, Out = 어두어짐
    public void ShowFadeInOut(EFadeType fadeType, float fadeOutTime, float fadeInTime, float delay = 0f, Action fadeOutCallBack = null, Action fadeInCallBack = null)
    {
        switch (fadeType)
        {
            case EFadeType.FadeInOut:
                _fadeInOutComplete = false;
                ExecuteFade(0, 1, fadeOutTime, 0, () =>
                {
                    fadeOutCallBack?.Invoke();
                    ExecuteFade(1, 0, fadeInTime, delay, () =>
                    {
                        _fadeInOutComplete = true;
                        fadeInCallBack?.Invoke();
                    });
                });
                break;
            case EFadeType.FadeOut:
                ExecuteFade(0, 1, fadeOutTime, delay, fadeOutCallBack);
                break;
            case EFadeType.FadeIn:
                ExecuteFade(1, 0, fadeInTime, delay, fadeInCallBack);
                break;
        }
    }

    private void ExecuteFade(float startAlpha, float endAlpha, float duration, float delay, Action onComplete)
    {
        fadeImage.DOFade(startAlpha, 0);
        fadeImage.DOFade(endAlpha, duration)
                 .SetDelay(delay) // 딜레이 추가
                 .OnComplete(() => HandleFadeComplete(onComplete, sceneMove ? false : true));
    }

    private void HandleFadeComplete(Action callback, bool setInactive = true)
    {
        callback?.Invoke();
        if (setInactive && _fadeInOutComplete == true)
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DOTween.Kill(fadeImage);
    }
}
