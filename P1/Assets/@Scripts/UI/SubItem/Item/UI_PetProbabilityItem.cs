using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PetProbabilityItem : UI_Base
{
    public enum Images 
    {
        PetFragmentIcon
    }

    public enum Texts 
    {
        Text_Chapter,
        Text_Rate
    }

    private int _chapterLevel;
    private string _spriteKey;
    private float _dropRate;

    protected override bool Init()
    {
        if(base.Init() == false)
        return false;

        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        return true;
    }

    public void SetInfo(int chpaterLevel, string spriteKey, float dropRate)
    {
        _chapterLevel = chpaterLevel;
        _spriteKey = spriteKey; 
        _dropRate = dropRate;
    }

    public void RefreshUI()
    {
        GetTMPText((int)Texts.Text_Chapter).text = $"챕터 {_chapterLevel}";
        GetTMPText((int)Texts.Text_Rate).text = $"{_dropRate}%";
        GetImage((int)Images.PetFragmentIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{_spriteKey}");
    }
}
