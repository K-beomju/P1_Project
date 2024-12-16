using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PetItem : UI_Base
{
    public enum Images
    {
        Image_PetIcon,
        Image_EggCraftItemIcon
    }

    public enum Texts
    {
        Text_PetName,
        Text_PetLevel,
        Text_Amount
    }

    public enum Sliders
    {
        Slider_Amount
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));

        return true;

    }

    public void RefreshUI()
    {
        
    }
}
