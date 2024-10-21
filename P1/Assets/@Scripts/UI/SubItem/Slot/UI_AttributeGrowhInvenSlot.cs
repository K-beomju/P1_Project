using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_AttributeGrowhInvenSlot : UI_Base
{
    public enum Texts
    {
        Text_AbLevel
    }

    public enum Images
    {
        Image_AbIcon
    }

    private TMP_Text levelText;
    private Image iconImage;

    private EHeroAttrType _heroAttrType;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        levelText = GetTMPText((int)Texts.Text_AbLevel);
        iconImage = GetImage((int)Images.Image_AbIcon);

        UpdateSlotInfoUI();
        return true;
    }

    public void SetInfo(EHeroAttrType attrType)
    {
        _heroAttrType = attrType;

        if (Init() == false)
        {
            UpdateSlotInfoUI();
        }
    }

    public void UpdateSlotInfoUI()
    {
        if (!Managers.Backend.GameData.UserData.UpgradeAttrDic.TryGetValue(_heroAttrType.ToString(), out int level))
        {
            Debug.LogWarning($"UpdateSlotInfoUI도중 {level}이 없습니다");
            return;
        }
        levelText.text =  $"Lv {level}";

    }
}
