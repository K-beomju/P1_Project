using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;


public class UI_EquipmentDrawItem : UI_Base
{
    public enum Texts
    {
        Text_RareType
    }

    public enum Images
    {
        Image_Equipment
    }
    private Text _rareTypeText;
    private Image _equipmentImage;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
        _rareTypeText = GetText((int)Texts.Text_RareType);
        _equipmentImage = GetImage((int)Images.Image_Equipment);

        return true;
    }

    public void SetInfo(EquipmentDrawResult equipmentData)
    {
        _rareTypeText.text = $"{equipmentData.RareType.ToString()} - 장비 {equipmentData.EquipmentIndex}번";
        // 여기에 장비 인덱스에 맞는 이미지를 설정하는 로직을 추가할 수 있습니다.
    }
}
