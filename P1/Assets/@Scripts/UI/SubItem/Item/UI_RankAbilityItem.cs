using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_RankAbilityItem : UI_Base
{
    public enum Texts
    {
        Text_Ability
    }

    public enum Images 
    {
        Image_ClassIcon
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        return true;
    }
    private ERankType _rankType;
    private ERankAbilityState _rankAbilityState;


    public void SetInfo(ERankType rankType)
    {
        _rankType = rankType;

    }

    public void RefreshUI()
    {
        _rankAbilityState = Util.ParseEnum<ERankAbilityState>(Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RankAbilityState.ToString());
        switch (_rankAbilityState)
        {
            case ERankAbilityState.Locked: // 잠겨있는 상태
                GetTMPText((int)Texts.Text_Ability).text = $"{Managers.Data.RankUpChart[_rankType].Name} 승급시 오픈";
                GetImage((int)Images.Image_ClassIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Lock");
                break; 
            case ERankAbilityState.Unlocked:  // 깨고 나서 비어있는 상태
                GetTMPText((int)Texts.Text_Ability).text = "";
                GetImage((int)Images.Image_ClassIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{_rankType}");
                break;
            case ERankAbilityState.Restricted: // 임의 잠금 상태
              
                break;
        }
    }

}
