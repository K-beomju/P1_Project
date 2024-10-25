using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BackendData.GameData;
using static Define;
using Unity.VisualScripting.Antlr3.Runtime.Tree;


public class UI_CompanionItem : UI_Base
{


    public enum Images
    {
        Image_Icon,
        Image_Fade,
        Image_Unowned,
        BG_Rare
    }

    public enum Texts
    {
        Text_Level,
        Text_ValueText
    }

    public enum Sliders 
    {
        Slider_Count
    }

    private Image _iconImage;
    private Image _fadeImage;
    private Button _companionButton;
    private Sequence _shakeSequence;

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
        return true;
    }

    public void SetItemInfo(Item itemData, bool showLvText = true, bool showCtSlider = true, bool isSlot = false)
    {
        if (itemData == null)
        {
            Debug.LogWarning("SetItemInfo: 아이템 데이터가 null입니다.");
            return;
        }

        _fadeImage.gameObject.SetActive(false);
        _companionButton.onClick.RemoveAllListeners();

        if (itemData is SkillInfoData)
            _companionButton.onClick.AddListener(() => Managers.Event.TriggerEvent(EEventType.SkillItemClick, itemData));
        else if (itemData is EquipmentInfoData)
            _companionButton.onClick.AddListener(() => Managers.Event.TriggerEvent(EEventType.EquipmentItemClick, itemData));

        bool isOwned = itemData.OwningState == EOwningState.Owned;
        GetSlider((int)Sliders.Slider_Count).gameObject.SetActive(showCtSlider);
        GetImage((int)Images.Image_Unowned).gameObject.SetActive(!isOwned);
        GetTMPText((int)Texts.Text_Level).gameObject.SetActive(showLvText && isOwned);
        if (isOwned)
            GetTMPText((int)Texts.Text_Level).text = $"Lv. {itemData.Level}";

        // 아이템의 희귀도 및 스프라이트 설정
        if (itemData is SkillInfoData skillInfo)
            GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(skillInfo.Data.RareType);
        else if (itemData is EquipmentInfoData equipmentInfo)
            GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(equipmentInfo.Data.RareType);

        _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{itemData.SpriteKey}");

        int currentCount = itemData.Count; 
        int maxCount =  Util.GetUpgradeEquipmentMaxCount(itemData.Level);
        GetSlider((int)Sliders.Slider_Count).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Count).value = currentCount;
        GetTMPText((int)Texts.Text_ValueText).text = $"{currentCount}/{maxCount}";

        // 슬롯이 아닌 경우 아이콘 크기 조정
        if (!isSlot && itemData is SkillInfoData)
        {
            _iconImage.rectTransform.sizeDelta = new Vector2(120, 120);
        }
    }

    // 아이템 뽑기 UI 설정 메서드 (장비 전용)
    public void SetDrawInfo(Item itemData)
    {
        _fadeImage.color = Color.white;
        _fadeImage.DOFade(0, 0.1f); // 아이콘 페이드 효과

        GetImage((int)Images.Image_Unowned).gameObject.SetActive(false);
        GetTMPText((int)Texts.Text_Level).gameObject.SetActive(false);
        GetSlider((int)Sliders.Slider_Count).gameObject.SetActive(false);

        if (itemData is SkillInfoData skillInfo)
            GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(skillInfo.Data.RareType);
        else if (itemData is EquipmentInfoData equipmentInfo)
            GetImage((int)Images.BG_Rare).color = Util.GetRareTypeColor(equipmentInfo.Data.RareType);
        _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{itemData.SpriteKey}");
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

    public void EnableButton(bool enable)
    {
        _companionButton.enabled = enable;
    }


}
