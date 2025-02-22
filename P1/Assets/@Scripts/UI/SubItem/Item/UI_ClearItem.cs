using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class UI_ClearItem : UI_Base
{
    public enum Images
    {
        Image_Icon
    }

    public enum Texts
    {
        Text_ItemCount
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        return true;
    }

    public void DisplayItem(Data.ItemData itemData,double count)
    {
        GetImage((int)Images.Image_Icon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{itemData.SpriteKey}");
        GetTMPText((int)Texts.Text_ItemCount).text = Util.ConvertToTotalCurrency(count);

        if(itemData.ItemType == Define.EItemType.AbilityPoint)
        {
            // offsetMin: Left, Bottom
            GetImage((int)Images.Image_Icon).rectTransform.offsetMin = new UnityEngine.Vector2(40, 25);
            GetImage((int)Images.Image_Icon).rectTransform.offsetMax = new UnityEngine.Vector2(-40, -15);
        }
    }
}
