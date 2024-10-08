using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillSlot : UI_Base
{
    private UI_CompanionItem _companionItem;
    public Image _lockImage;

    public enum Images 
    {
        Image_Lock
    }

    protected override bool Init()
    {
        if(base.Init() == false)
        return false;
        BindImages(typeof(Images));

        _lockImage = GetImage((int)Images.Image_Lock);
        _companionItem = GetComponentInChildren<UI_CompanionItem>();
        _companionItem.gameObject.SetActive(false);
        return true;
    }
}
