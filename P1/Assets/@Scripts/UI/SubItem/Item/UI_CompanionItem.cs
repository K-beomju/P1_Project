using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BackendData.GameData;
using static Define;

public class UI_CompanionItem : UI_Base
{


    public enum Images
    {
        Image_Icon,
        Image_Fade,
        Image_Unowned,
        Image_Equip,
        BG_Rare
    }

    public enum Texts
    {
        Text_Level,
        Text_ValueText,
        Text_EnhanceLevel
    }

    public enum Sliders
    {
        Slider_Count
    }

    private Image _iconImage;
    private Image _fadeImage;
    private Button _companionButton;
    private Sequence _shakeSequence;
    private RectTransform _rectTransform;

    public Item ItemData { get; private set; }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));

        _iconImage = GetImage((int)Images.Image_Icon);
        _fadeImage = GetImage((int)Images.Image_Fade);
        _companionButton = GetComponent<Button>();
        _rectTransform = GetComponent<RectTransform>();
        return true;
    }

    public void DisplayItem(Item itemData, EItemDisplayType displayType)
    {
        if (itemData == null)
        {
            Debug.LogWarning("SetItemInfo: 아이템 데이터가 null입니다.");
            return;
        }
        ItemData = itemData;

        // Item 공통 초기화 
        _companionButton.onClick.RemoveAllListeners();
        _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{ItemData.SpriteKey}");

        if (ItemData is SkillInfoData skillInfo)
        {
            _companionButton.onClick.AddListener(() => Managers.Event.TriggerEvent(EEventType.SkillItemClick, ItemData));
            GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(skillInfo.Data.RareType);

            // 스킬 슬롯 아이템 -> 이미지 사이즈 조정
            if (displayType == EItemDisplayType.ImageOnly)
                _iconImage.rectTransform.sizeDelta = new Vector2(160, 160);

            if (displayType == EItemDisplayType.SlotItem)
                _iconImage.rectTransform.sizeDelta = new Vector2(130, 130);

        }
        else if (ItemData is EquipmentInfoData equipmentInfo)
        {
            _companionButton.onClick.AddListener(() => Managers.Event.TriggerEvent(EEventType.EquipmentItemClick, ItemData));
            GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(equipmentInfo.Data.RareType);

            //_iconImage.rectTransform.anchoredPosition = new Vector2(0, 5);
        }

        bool isOwned = ItemData.OwningState == EOwningState.Owned;
        bool isEquipped = ItemData.IsEquipped;

        GetImage((int)Images.Image_Unowned).gameObject.SetActive(!isOwned);
        switch (displayType)
        {
            // 목록 아이템 -> 모든 UI 데이터 표시 
            case EItemDisplayType.Basic:
                // Active
                _fadeImage.gameObject.SetActive(false);
                GetTMPText((int)Texts.Text_EnhanceLevel).gameObject.SetActive(false);

                GetSlider((int)Sliders.Slider_Count).gameObject.SetActive(true);
                GetTMPText((int)Texts.Text_Level).gameObject.SetActive(true);
                GetImage((int)Images.Image_Equip).gameObject.SetActive(isEquipped);
                // DATA 
                if (isOwned)
                    GetTMPText((int)Texts.Text_Level).text = $"Lv.{ItemData.Level}";
                int currentCount = ItemData.Count;
                int maxCount = Util.GetUpgradeEquipmentMaxCount(ItemData.Level);
                GetSlider((int)Sliders.Slider_Count).maxValue = maxCount;
                GetSlider((int)Sliders.Slider_Count).value = currentCount;
                GetTMPText((int)Texts.Text_ValueText).text = $"{currentCount}/{maxCount}";
                break;
            // 아이템 디테일 -> 이미지만 표시 
            case EItemDisplayType.ImageOnly:
            case EItemDisplayType.SlotItem:

                _fadeImage.gameObject.SetActive(false);
                GetTMPText((int)Texts.Text_EnhanceLevel).gameObject.SetActive(false);
                GetSlider((int)Sliders.Slider_Count).gameObject.SetActive(false);
                GetTMPText((int)Texts.Text_Level).gameObject.SetActive(false);
                GetImage((int)Images.Image_Equip).gameObject.SetActive(false);
                break;
            case EItemDisplayType.Enhance:
                GetTMPText((int)Texts.Text_EnhanceLevel).gameObject.SetActive(true);

                _fadeImage.gameObject.SetActive(false);
                GetSlider((int)Sliders.Slider_Count).gameObject.SetActive(false);
                GetTMPText((int)Texts.Text_Level).gameObject.SetActive(false);
                GetImage((int)Images.Image_Equip).gameObject.SetActive(false);
                break;
            case EItemDisplayType.Draw:
                _fadeImage.gameObject.SetActive(true);
                _fadeImage.color = Color.white;
                _fadeImage.DOFade(0, 0.1f); // 아이콘 페이드 효과

                GetImage((int)Images.Image_Equip).gameObject.SetActive(false);
                GetImage((int)Images.Image_Unowned).gameObject.SetActive(false);
                GetTMPText((int)Texts.Text_Level).gameObject.SetActive(false);
                GetSlider((int)Sliders.Slider_Count).gameObject.SetActive(false);
                GetTMPText((int)Texts.Text_EnhanceLevel).gameObject.SetActive(false);

                _iconImage.rectTransform.anchoredPosition = new Vector2(0, 0);
                break;
        }
    }

    public void DisplayEnhanceLevel(int prevLevel, EquipmentInfoData equipmentInfoData)
    {
        DisplayItem(equipmentInfoData, EItemDisplayType.Enhance);
        // 텍스트 변경 효과
        var textComponent = GetTMPText((int)Texts.Text_EnhanceLevel);
        textComponent.color = Color.yellow; // 텍스트 색상 변경 (강화 시 색상)
        textComponent.rectTransform.DOScale(1.2f, 0.15f).OnComplete(() =>
        {
            textComponent.DOText($"{prevLevel}   >  {equipmentInfoData.Level}", 0.3f)
                .OnComplete(() =>
                {
                    textComponent.color = Color.white; // 색상 원상복구
                    textComponent.rectTransform.DOScale(1f, 0.15f); // 크기 원상복구
                });
        });
    }


    // Z축 기준으로 랜덤하게 흔드는 애니메이션 실행 (DOShakeRotation 사용)
    public void PlayShakeAnimation(float duration = 1f, float strength = 30, int vibrato = 15, float randomness = 90)
    {
        if (_shakeSequence != null && _shakeSequence.IsPlaying())
        {
            _shakeSequence.Kill(); // 시퀀스를 중지하고 초기화
            _shakeSequence = null;
        }

        // 시퀀스를 생성하여 애니메이션 반복 재생 설정
        _shakeSequence = DOTween.Sequence()
            .Append(transform.DOShakeRotation(duration, new Vector3(0, 0, strength), vibrato, randomness, fadeOut: true))
            .AppendInterval(1f)  // 1초 동안 대기
            .SetLoops(-1, LoopType.Restart)  // 무한 반복 재생
            .SetEase(Ease.OutCubic);
    }

    public void StopShakeAnimation()
    {
        if (_shakeSequence != null && _shakeSequence.IsPlaying())
        {
            _shakeSequence.Kill(); // 시퀀스를 중지하고 초기화
            _shakeSequence = null;
        }

        // 기본 회전 상태로 복구
        transform.rotation = Quaternion.identity;
    }

    public void EnableButton(bool enable)
    {
        _companionButton.enabled = enable;
    }

    public Vector2 ItemPosition()
    {
        return _rectTransform.anchoredPosition;
    }


}
