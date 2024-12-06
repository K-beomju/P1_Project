using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_BossStageDisplayBase : UI_Base
{
    public enum RawImages
    {
        Image_BossComing
    }

    public enum Images
    {
        Image_BossIcon
    }

    private RawImage _bossComingImage;
    private Image _bossIconImage;
    private float _scrollSpeed = 0.5f; // UV Rect X 증가 속도
    private Sequence _iconSequence; // 보스 아이콘 애니메이션 시퀀스
    private Sequence _comingSequence; // 보스 커밍 애니메이션 시퀀스

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
            
        BindImages(typeof(Images));
        Bind<RawImage>(typeof(RawImages));

        _bossComingImage = Get<RawImage>((int)RawImages.Image_BossComing);
        _bossIconImage = GetImage((int)Images.Image_BossIcon);

        return true;
    }

    private void Update()
    {
        if (_bossComingImage != null)
        {
            // 현재 UV Rect 값을 가져옵니다.
            Rect currentRect = _bossComingImage.uvRect;

            // X 값을 증가시켜 UV Rect를 업데이트합니다.
            currentRect.x -= _scrollSpeed * Time.deltaTime;

            // 변경된 UV Rect를 적용합니다.
            _bossComingImage.uvRect = currentRect;
        }
    }

    public void ShowDisplay()
    {
        PlayBossComingAnimation();
        PlayBossIconAnimation();
    }

    // 보스 커밍 연출을 시작하는 함수
    public void PlayBossComingAnimation()
    {
        if (_comingSequence != null && _comingSequence.IsPlaying())
        {
            _comingSequence.Kill();
        }

        _bossComingImage.gameObject.SetActive(true);

        // 초기 상태 설정
        _bossComingImage.color = new Color(1, 1, 1, 0); // 투명
        _bossComingImage.rectTransform.localScale = Vector3.zero; // 크기 0

        // 보스 커밍 연출 시퀀스 생성
        _comingSequence = DOTween.Sequence()
            .Append(_bossComingImage.DOFade(1, 0.5f)) // 페이드 인
            .Join(_bossComingImage.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)) // 크기 확대
            .AppendInterval(1.0f) // 잠시 대기
            .Append(_bossComingImage.DOFade(0, 0.5f)) // 페이드 아웃
            .Join(_bossComingImage.rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)) // 크기 축소
            .OnComplete(() => gameObject.SetActive(false));
    }

    // 보스 아이콘 연출을 시작하는 함수
    public void PlayBossIconAnimation()
    {
        if (_iconSequence != null && _iconSequence.IsPlaying())
        {
            _iconSequence.Kill();
        }

        _bossIconImage.gameObject.SetActive(true);

        // 아이콘 초기 상태 설정
        _bossIconImage.color = new Color(1, 1, 1, 0); // 투명
        _bossIconImage.rectTransform.localScale = Vector3.zero; // 크기 0

        // 아이콘 연출 시퀀스 생성
        _iconSequence = DOTween.Sequence()
            .Append(_bossIconImage.DOFade(1, 0.5f)) // 페이드 인
            .Join(_bossIconImage.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)) // 크기 확대
            .AppendInterval(1.0f) // 잠시 대기
            .Append(_bossIconImage.DOFade(0, 0.5f)) // 페이드 아웃
            .Join(_bossIconImage.rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)) // 크기 축소
            .OnComplete(() => gameObject.SetActive(false));
    }
}
