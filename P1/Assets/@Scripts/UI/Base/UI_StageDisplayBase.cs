using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using static Define;

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
    public void ShowDisplayStage(int stage)
    {
        ShowDisplay($"스테이지 {stage} !!");
    }

    // 던전 표시 UI를 업데이트하는 함수
    public void ShowDisplayDungeon(EDungeonType type, int dungeonLevel)
    {
        ShowDisplay($"{Util.GetDungeonType(type)} {dungeonLevel} !!");
    }

    // 스테이지 표시 UI를 업데이트하는 함수
    private void ShowDisplay(string textContent)
    {
        // 실행 중인 시퀀스가 있으면 중지
        if (_stageSequence != null && _stageSequence.IsPlaying())
        {
            _stageSequence.Kill();
        }

        TMP_Text text = GetTMPText((int)Texts.StageDisplayText);
        text.text = textContent;

        // 텍스트 초기 상태 설정 (알파값 0, 크기 축소)
        InitializeTextState(text);

        // 등장 연출: 텍스트를 페이드 인 및 크기 확대
        _stageSequence = CreateDisplaySequence(text);
    }

    private void InitializeTextState(TMP_Text text)
    {
        text.alpha = 0;
        text.rectTransform.anchoredPosition = _originalAnchoredPosition;
        text.rectTransform.localScale = Vector3.zero;
    }

    private Sequence CreateDisplaySequence(TMP_Text text)
    {
        return DOTween.Sequence()
            .Append(text.DOFade(1, 0.5f))
            .Join(text.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack))
            .AppendInterval(1.0f)
            .Append(text.DOFade(0, 0.5f))
            .Join(text.rectTransform.DOAnchorPosY(_originalAnchoredPosition.y + 50, 0.5f).SetEase(Ease.InCubic))
            .OnComplete(() => gameObject.SetActive(false));
    }
}
