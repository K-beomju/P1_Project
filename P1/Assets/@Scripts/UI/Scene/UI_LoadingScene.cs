using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LoadingScene : UI_Scene
{
    public enum Sliders
    {
        Slider_Loading
    }

    public enum Texts
    {
        Text_Loading
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindSliders(typeof(Sliders));
        BindTMPTexts(typeof(Texts));

        return true;
    }

    public void ShowDataName(string info)
    {
        GetTMPText((int)Texts.Text_Loading).text = info;
        Debug.Log(info);
    }

    public void ShowDataSlider(int currentCount, int maxCount)
    {
        GetSlider((int)Sliders.Slider_Loading).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Loading).value = currentCount;
    }


}
