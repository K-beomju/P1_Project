using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Text;

public class UI_DialoguePopup : UI_Popup
{
    public enum GameObjects
    {
        BG
    }

    public enum Texts
    {
        Text_Desc
    }

    public enum Images
    {
        Image_Hero
    }

    private Queue<string> textQueue = new();
    private TMP_Text currentText; // 현재 텍스트 UI
    private Tween typingTween;   // 타이핑 애니메이션 Tween

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));

        currentText = GetTMPText((int)Texts.Text_Desc);            

        currentText.text = string.Empty;
        textQueue.Enqueue("마을이 위험해! 내가 나서지 않으면 모두 위험해질 거야.");
        textQueue.Enqueue("몬스터들을 막아야 해! 한 발짝도 더는 허락하지 않겠다!!");

        GetObject((int)GameObjects.BG).BindEvent(OnClickBG, Define.EUIEvent.Click);
        return true;
    }

    private void OnClickBG()
    {
        // 타이핑 중에 클릭하면 즉시 완료
        if (typingTween != null && typingTween.IsActive())
        {
            typingTween.Complete();
            return;
        }

        // 텍스트 큐에서 문자열 꺼내고 UI 갱신
        if (textQueue.Count > 0)
        {
            RefreshUI();
        }
        else
        {
            // 모든 텍스트가 출력되면 팝업 닫기
            Managers.Scene.GetCurrentScene<GameScene>().SetupStage();
            ClosePopupUI();
        }
    }

    public void RefreshUI()
    {
        // 큐에서 문자열 가져오기
        if (textQueue.Count > 0)
        {
            string nextText = textQueue.Dequeue();
            PlayTypingAnimation(nextText);
        }
        else
        {
            // 큐가 비어 있을 때 텍스트 비우기
            currentText.text = string.Empty;
        }
    }

    private void PlayTypingAnimation(string text)
    {
        // 이전 Tween이 활성화되어 있으면 종료
        if (typingTween != null && typingTween.IsActive())
        {
            typingTween.Kill();
        }

        // 텍스트를 점진적으로 추가하기 위해 StringBuilder 사용
        StringBuilder builder = new StringBuilder();
        int length = text.Length;

        typingTween = DOTween.To(() => builder.Length,
                                 x =>
                                 {
                                     builder.Length = x;
                                     builder.Clear();
                                     builder.Append(text.Substring(0, x));
                                     currentText.text = builder.ToString();
                                 },
                                 length, 1.5f) // 1.5초 동안 타이핑
            .SetEase(Ease.Linear)
            .OnComplete(() => PlayHeroShakeAnimation()); // 타이핑 완료 후 호출
    }


    private void PlayHeroShakeAnimation()
    {
        var heroImage = GetImage((int)Images.Image_Hero);

        if (heroImage != null)
        {
            // DOShakePosition 또는 DOPunchPosition을 사용하여 흔들리는 효과 추가
            heroImage.rectTransform.DOPunchPosition(new Vector3(0, 20, 0), 0.3f, 10, 1f)
                .SetEase(Ease.OutQuad); // 부드러운 흔들림
        }
    }
}
