using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_RelicGrowInvenSlot : UI_Base
{
    public enum Texts
    {
        Text_RelicNameInCount,
        Text_RelicRemarkInValue,
        Text_BaseDecs
    }

    public enum Images
    {
        Image_Relic
    }

    private TMP_Text _nameInCountText;
    private TMP_Text _remarkInValueText;
    private TMP_Text _baseDecsText;
    private Image _iconImage;
    private EHeroRelicType _heroRelicType;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        _nameInCountText = GetTMPText((int)Texts.Text_RelicNameInCount);
        _remarkInValueText = GetTMPText((int)Texts.Text_RelicRemarkInValue);
        _baseDecsText = GetTMPText((int)Texts.Text_BaseDecs);
        _iconImage = GetImage((int)Images.Image_Relic);
        UpdateSlotInfoUI();
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.HeroRelicUpdated, new Action(UpdateSlotInfoUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.HeroRelicUpdated, new Action(UpdateSlotInfoUI));
    }


    public void SetInfo(EHeroRelicType relicType)
    {
        _heroRelicType = relicType;

        if (Init() == false)
        {
            UpdateSlotInfoUI();
        }
    }

    public void UpdateSlotInfoUI()
    {
        if (!Managers.Backend.GameData.CharacterData.OwnedRelicDic.TryGetValue(_heroRelicType.ToString(), out int count))
        {
            Debug.LogWarning($"UpdateSlotInfoUI도중 {count}이 없습니다");
            return;
        }


        Data.RelicInfoData relicData = Managers.Data.RelicChart[_heroRelicType];

        if (relicData == null)
        {
            Debug.LogWarning($"HeroRelicInfoData가 없습니다");
            return;
        }

        if (_iconImage.sprite == null)
            _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{relicData.SpriteKey}");


        _nameInCountText.text = $"{relicData.Name} [ <color=#6BFF9F>{count}</color> / {relicData.MaxCount} ]";
        _remarkInValueText.text = $"{relicData.Remark} {relicData.IncreaseValue * count}%";
        _baseDecsText.text = $"{relicData.Remark}\n {relicData.IncreaseValue}% x 보유 갯수";



    }

}
