using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_TotalPowerBase : UI_Base
{
    public enum AdJustType 
    {
        Increase,
        Decrease,
    }


    public enum Texts
    {
        Text_CurrentTotalPower,
        Text_AdjustTotalPower
    }

    public enum Images
    {
        Image_Adjustment
    }


    private TMP_Text _currentTotalPowerText;
    private TMP_Text _adJustTotalPowerText;
    private Image _adJustmentImage;

    private CanvasGroup _canvasGroup;
    private Sequence _sequence;

    private RectTransform _rectTransform;
    private Vector2 _originalPosition;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Canvas canvas = GetComponent<Canvas>();
        canvas.sortingOrder = SortingLayers.UI_TOTALPOWER;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        _canvasGroup = GetComponent<CanvasGroup>();

        _currentTotalPowerText = GetTMPText((int)Texts.Text_CurrentTotalPower);
        _adJustTotalPowerText = GetTMPText((int)Texts.Text_AdjustTotalPower);
        _adJustmentImage = GetImage((int)Images.Image_Adjustment);

        _rectTransform = Util.FindChild(this.gameObject, "BG", false).GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        return true;
    }

    public void ShowTotalPowerUI()
    {
        float currentTotalPower = Managers.Hero.PlayerHeroInfo.CurrentTotalPower;
        float adjustTotalPower = Managers.Hero.PlayerHeroInfo.AdjustTotalPower;

        // 현재 트윈이 실행 중이면 중지하고 새로 시작
        if (_sequence != null && _sequence.IsPlaying())
        {
            _sequence.Kill();
            StopCoroutine(CaculateTotalPower(currentTotalPower, adjustTotalPower));
        }


        // 시작 위치 초기화 
        _rectTransform.anchoredPosition = new Vector2(_originalPosition.x, _originalPosition.y - 50);
        _canvasGroup.alpha = 0;

        // adjustTotalPower 소수 첫째 자리로 반올림 및 작은 값 처리
        float roundedAdjustTotalPower = Mathf.Round(adjustTotalPower * 10) / 10;
        if (roundedAdjustTotalPower < 1f && roundedAdjustTotalPower > 0f)
        {
            roundedAdjustTotalPower = 1f; // 1로 설정
        }

        AdJustType type = adjustTotalPower > 0f ? AdJustType.Increase : AdJustType.Decrease;
        _adJustmentImage.transform.localRotation = Quaternion.Euler(0, 0, type == AdJustType.Increase ? 0 : 180);

        _adJustTotalPowerText.gameObject.SetActive(true);
        _adJustTotalPowerText.text = Mathf.Abs(roundedAdjustTotalPower).ToString("N0");
        _currentTotalPowerText.text = Util.ConvertToTotalPower(currentTotalPower - adjustTotalPower);

        // 트윈 애니메이션 설정
        _sequence = DOTween.Sequence();
        _sequence.Append(_canvasGroup.DOFade(1, 0.5f));
        _sequence.Join(_rectTransform.DOAnchorPosY(_originalPosition.y, 0.5f).SetEase(Ease.OutCubic));

        // adjustTotalPower 값을 소수 첫째 자리로 반올림하여 표시
        _sequence.AppendCallback(() => StartCoroutine(CaculateTotalPower(currentTotalPower, roundedAdjustTotalPower)));
        _sequence.AppendInterval(0.5f);
        _sequence.Append(_canvasGroup.DOFade(0, 0.5f));
        _sequence.Join(_rectTransform.DOAnchorPosY(_originalPosition.y + 50, 0.5f).SetEase(Ease.InCubic));
        _sequence.OnComplete(() => gameObject.SetActive(false));

    }

    private IEnumerator CaculateTotalPower(float currentTotalPower, float adjustTotalPower)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        float beforeTotalPower = currentTotalPower - adjustTotalPower;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            beforeTotalPower = Mathf.Lerp(beforeTotalPower, currentTotalPower, t);
            adjustTotalPower = Mathf.Lerp(adjustTotalPower, 0, t);

            _currentTotalPowerText.text = Util.ConvertToTotalPower(beforeTotalPower);
            _adJustTotalPowerText.text = Mathf.Abs(adjustTotalPower).ToString("N0");

            // 0에 거의 근접했을 때 종료
            if (Mathf.Abs(adjustTotalPower) <= 0.2f)
            {
                _currentTotalPowerText.text = Util.ConvertToTotalPower(currentTotalPower);
                _adJustTotalPowerText.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }
}
