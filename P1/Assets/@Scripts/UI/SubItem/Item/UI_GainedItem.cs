using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GainedItem : UI_Base
{
    public enum Images
    {
        Image_Icon
    }

    public enum Texts
    {
        Text_DrawCount
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        return true;
    }

    public void DisplayItem(Data.RelicInfoData relicData, int count)
    {
        GetImage((int)Images.Image_Icon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{relicData.SpriteKey}");
        GetTMPText((int)Texts.Text_DrawCount).text = count.ToString();
    }
}
