using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageClearBase : UI_Base
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _originalAnchoredPosition;
    private Sequence _sequence;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _rectTransform = Util.FindChild(this.gameObject, "BG", false).GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _originalAnchoredPosition = _rectTransform.anchoredPosition;

        return true;
    }

    public void ShowStageClearUI()
    {
        if (_sequence != null && _sequence.IsPlaying())
        {
            _sequence.Kill();
        }

        // 초기화
        _canvasGroup.alpha = 0;
        _rectTransform.anchoredPosition = _originalAnchoredPosition - new Vector2(0, 50);
        _rectTransform.localScale = Vector3.zero;

        // 트윈 연출
        _sequence = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(1, 0.5f))
            .Join(_rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack))
            .Join(_rectTransform.DOAnchorPosY(_originalAnchoredPosition.y, 0.5f).SetEase(Ease.OutCubic))
            .AppendInterval(1.0f)
            .Append(_canvasGroup.DOFade(0, 0.5f))
            .Join(_rectTransform.DOAnchorPosY(_originalAnchoredPosition.y + 50, 0.5f).SetEase(Ease.InCubic))
            .OnComplete(() => gameObject.SetActive(false));
    }
}