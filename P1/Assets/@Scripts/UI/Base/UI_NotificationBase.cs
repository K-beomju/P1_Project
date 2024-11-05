using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_NotificationBase : UI_Base
{
    public enum Texts
    {
        Text_Notif
    }

    private CanvasGroup _canvasGroup;
    private Sequence _sequence;

    private RectTransform _rectTransform;
    private Vector2 _originalPosition;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Canvas canvas = GetComponent<Canvas>();
        canvas.sortingOrder = SortingLayers.UI_NOTIFICATION;

        BindTMPTexts(typeof(Texts));
        _canvasGroup = GetComponent<CanvasGroup>();


        _rectTransform = Util.FindChild(this.gameObject, "BG", false).GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        return true;
    }

    public void ShowNotification(string text)
    {
        // 현재 트윈이 실행 중이면 중지하고 새로 시작
        if (_sequence != null && _sequence.IsPlaying())
        {
            _sequence.Kill();
        }
        // 시작 위치 초기화 
        _rectTransform.anchoredPosition = new Vector2(_originalPosition.x, _originalPosition.y - 50);
        _canvasGroup.alpha = 0;

        GetTMPText((int)Texts.Text_Notif).text = text;

        // 트윈 애니메이션 설정
        _sequence = DOTween.Sequence();
        _sequence.Append(_canvasGroup.DOFade(1, 0.5f));
        _sequence.Join(_rectTransform.DOAnchorPosY(_originalPosition.y, 0.5f).SetEase(Ease.OutCubic));
        _sequence.AppendInterval(0.5f);
        _sequence.Append(_canvasGroup.DOFade(0, 0.5f));
        _sequence.Join(_rectTransform.DOAnchorPosY(_originalPosition.y + 50, 0.5f).SetEase(Ease.InCubic));
        _sequence.OnComplete(() => gameObject.SetActive(false));
    }
}
