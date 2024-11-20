using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_RankUpClearPopup : UI_Popup
{
    public enum GameObjects
    {
        BG
    }

    public enum Images
    {
        Image_RankIcon
    }

    public enum Texts
    {
        Text_MyRank
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, false, SortingLayers.UI_SCENE + 1);
        
        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        
        GetObject((int)GameObjects.BG).BindEvent(ClosePopupUI, EUIEvent.Click);
        return true;
    }


    public void RefreshUI()
    {
        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Current);
        GetTMPText((int)Texts.Text_MyRank).text = Managers.Data.RankUpChart[rankType].Name;
        GetImage((int)Images.Image_RankIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{rankType}");
    }
}
