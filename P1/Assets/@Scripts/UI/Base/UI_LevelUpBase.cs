using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI_LevelUpBase : UI_Base
{

    public enum Texts
    {
        Text_Level
    }

    private CanvasGroup _canvasGroup;
    private TMP_Text _levelText;
    private RectTransform _rectTransform;
    private Vector2 _originalPosition; // 초기 위치 저장
    private Sequence _sequence;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));
        _canvasGroup = GetComponent<CanvasGroup>();
        _levelText = GetTMPText((int)Texts.Text_Level);
        _rectTransform = Util.FindChild(this.gameObject, "BG", false).GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;

        return true;
    }

    public void ShowLevelUpUI(int level)
    {
        if (level == 0)
            return;

         // 현재 트윈이 실행 중이면 중지하고 새로 시작
        if (_sequence != null && _sequence.IsPlaying())
        {
            _sequence.Kill();
        }


        // 시작 위치 초기화 
        _rectTransform.anchoredPosition = new Vector2(_originalPosition.x, _originalPosition.y - 50);
        _canvasGroup.alpha = 0;

        _levelText.text = $"Lv.{level}";

        // 애니메이션 구성
        _sequence = DOTween.Sequence();
        _sequence.Append(_canvasGroup.DOFade(1, 0.5f));
        _sequence.Join(_rectTransform.DOAnchorPosY(_originalPosition.y, 0.5f).SetEase(Ease.OutCubic));
        _sequence.AppendInterval(1f);
        _sequence.Append(_canvasGroup.DOFade(0, 0.5f));
        _sequence.Join(_rectTransform.DOAnchorPosY(_originalPosition.y + 50, 0.5f).SetEase(Ease.InCubic));
        // 애니메이션 완료 후 처리
        _sequence.OnComplete(() => gameObject.SetActive(false));

    }
}
