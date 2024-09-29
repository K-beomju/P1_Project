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

    // 초기화 함수
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        return true;
    }

    // 스테이지 표시 UI를 업데이트하는 함수
    public void RefreshShowDisplayStage(int stage)
    {
        TMP_Text text = GetTMPText((int)Texts.StageDisplayText);
        text.text = $"스테이지 {stage} !!";

        // 애니메이션 효과 설정
        Sequence stageSequence = DOTween.Sequence();

        // 텍스트 초기 상태 설정 (알파값 0, 크기 축소)
        text.alpha = 0;
        text.rectTransform.localScale = Vector3.zero;

        // 등장 연출: 텍스트를 페이드 인 및 크기 확대
        stageSequence.Append(text.DOFade(1, 0.5f)) // 페이드 인 (0.5초 동안 알파값 1)
                     .Join(text.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)) // 크기 확대 (원래 크기로 0.5초 동안 확장)
                     .AppendInterval(1.0f) // 1초 동안 유지
                                           // 사라지는 연출: 텍스트를 위로 이동하며 페이드 아웃
                     .Append(text.DOFade(0, 0.5f)) // 페이드 아웃 (0.5초 동안 알파값 0)
                     .Join(text.rectTransform.DOMoveY(text.rectTransform.position.y + 50, 0.5f).SetEase(Ease.InCubic)) // 위로 이동 (0.5초 동안 y 축 방향으로 50만큼 이동)
                     .OnComplete(() =>
                     {
                         Managers.Resource.Destroy(this.gameObject); // 애니메이션 종료 후 객체 삭제
                     });
    }
}
