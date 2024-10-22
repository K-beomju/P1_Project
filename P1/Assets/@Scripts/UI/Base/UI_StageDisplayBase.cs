using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UI_StageDisplayBase : UI_Base
{
    enum Texts
    {
        StageDisplayText
    }

    private Sequence _stageSequence; // 시퀀스를 멤버 변수로 관리
    private Vector2 _originalAnchoredPosition; // 초기 위치 저장

    // 초기화 함수
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        _originalAnchoredPosition = GetTMPText((int)Texts.StageDisplayText).rectTransform.anchoredPosition;
        return true;
    }

    // 스테이지 표시 UI를 업데이트하는 함수
    public void RefreshShowDisplayStage(int stage)
    {
        // 실행 중인 시퀀스가 있으면 중지
        if (_stageSequence != null && _stageSequence.IsPlaying())
        {
            _stageSequence.Kill();
        }

        TMP_Text text = GetTMPText((int)Texts.StageDisplayText);
        text.text = $"스테이지 {stage} !!";

        // 텍스트 초기 상태 설정 (알파값 0, 크기 축소)
        text.alpha = 0;
        text.rectTransform.anchoredPosition = _originalAnchoredPosition; // anchoredPosition으로 위치 설정
        text.rectTransform.localScale = Vector3.zero;

        // 새로운 시퀀스 생성
        _stageSequence = DOTween.Sequence();

        // 등장 연출: 텍스트를 페이드 인 및 크기 확대
        _stageSequence.Append(text.DOFade(1, 0.5f)) // 페이드 인 (0.5초 동안 알파값 1)
                     .Join(text.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)) // 크기 확대 (원래 크기로 0.5초 동안 확장)
                     .AppendInterval(1.0f) // 1초 동안 유지
                                           // 사라지는 연출: 텍스트를 위로 이동하며 페이드 아웃
                     .Append(text.DOFade(0, 0.5f)) // 페이드 아웃 (0.5초 동안 알파값 0)
                     .Join(text.rectTransform.DOAnchorPosY(_originalAnchoredPosition.y + 50, 0.5f).SetEase(Ease.InCubic)) // anchoredPosition.y 기준으로 위로 50만큼 이동
                     .OnComplete(() =>
                     {
                        gameObject.SetActive(false);
                     });

    }
}
