using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI_LevelUpBase : UI_Base
{
    public enum RectTransforms
    {
        BG
    }

    public enum Texts
    {
        Text_Level
    }

    private CanvasGroup _canvasGroup;
    private TMP_Text _levelText;
    private RectTransform _rectTransform;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));
        Bind<RectTransform>(typeof(RectTransforms));
        _canvasGroup = GetComponent<CanvasGroup>();
        _levelText = GetTMPText((int)Texts.Text_Level);
        _rectTransform = Get<RectTransform>((int)RectTransforms.BG);

        return true;
    }

    public void ShowLevelUpUI(int level)
    {
        Debug.Log(level);
        if (level == 0)
            return;

        _levelText.text = $"Lv.{level}";
        // 시작 위치 설정 (아래에서 시작)
        Vector3 startPos = _rectTransform.anchoredPosition;
        _rectTransform.anchoredPosition = new Vector2(startPos.x, startPos.y - 50);
        _canvasGroup.alpha = 0;

        Sequence sequence = DOTween.Sequence();

        // 등장 애니메이션: 페이드 인과 함께 위로 이동
        sequence.Append(_canvasGroup.DOFade(1, 0.5f));
        sequence.Join(_rectTransform.DOAnchorPosY(startPos.y, 0.5f).SetEase(Ease.OutCubic));

        // 유지 시간
        sequence.AppendInterval(1f);

        // 사라지는 애니메이션: 페이드 아웃과 함께 위로 이동
        sequence.Append(_canvasGroup.DOFade(0, 0.5f));
        sequence.Join(_rectTransform.DOAnchorPosY(startPos.y + 50, 0.5f).SetEase(Ease.InCubic));

        // 애니메이션 완료 후 처리
        sequence.OnComplete(() =>
        {
            Managers.Object.Despawn(this);
        });
    }
}
