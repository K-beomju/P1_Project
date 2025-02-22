using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_RankAbilityItem : UI_Base
{
    public enum Texts
    {
        Text_Ability
    }

    public enum Images
    {
        Image_ClassIcon,
        Image_LockAbility
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        _lockAbilityBtn = GetComponent<Button>();
        _lockAbilityBtn.onClick.AddListener(OnButtonClick);
        return true;
    }

    private ERankType _rankType;
    private ERankAbilityState _rankAbilityState;
    private Button _lockAbilityBtn;
    
    public void SetInfo(ERankType rankType)
    {
        _rankType = rankType;
    }

    private void OnButtonClick()
    {
        switch (Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RankAbilityState)
        {
            case ERankAbilityState.Restricted:
                Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RankAbilityState = ERankAbilityState.Acquired;
                break;
            case ERankAbilityState.Acquired:
                Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RankAbilityState = ERankAbilityState.Restricted;
                break;
        }

        RefreshUI();
        Managers.Event.TriggerEvent(EEventType.HeroRankUpdated);

    }

    public void RefreshUI()
    {
        _lockAbilityBtn.interactable = true;
        _rankAbilityState = Util.ParseEnum<ERankAbilityState>(Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RankAbilityState.ToString());
        switch (_rankAbilityState)
        {
            case ERankAbilityState.Locked:  // 잠긴 상태, 능력을 획득할 수 없음
                GetTMPText((int)Texts.Text_Ability).text = $"{Managers.Data.RankUpChart[_rankType].Name} 승급시 오픈";
                GetImage((int)Images.Image_LockAbility).gameObject.SetActive(false);
                GetTMPText((int)Texts.Text_Ability).color = Color.white;
                GetImage((int)Images.Image_ClassIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Lock");
                _lockAbilityBtn.interactable = false;
                break;
            case ERankAbilityState.Unlocked:  // 해제된 상태, 능력을 획득할 수 있음
                GetTMPText((int)Texts.Text_Ability).text = string.Empty;   
                GetImage((int)Images.Image_LockAbility).gameObject.SetActive(false);
                GetImage((int)Images.Image_ClassIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{_rankType}");
                break;
            case ERankAbilityState.Restricted:   // 임의로 잠긴 상태, 능력을 변경할 수 없음,
            case ERankAbilityState.Acquired:     // 능력이 존재하며 활성화된 상태
                GetImage((int)Images.Image_ClassIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{_rankType}");
                GetImage((int)Images.Image_LockAbility).gameObject.SetActive(_rankAbilityState == ERankAbilityState.Restricted);
                string statName = Managers.Data.DrawRankUpChart[Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RankStatType].Name;
                int increaseValue = Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].Value;
                GetTMPText((int)Texts.Text_Ability).text = $"{statName} {increaseValue}";
                GetTMPText((int)Texts.Text_Ability).color = Util.GetRareTypeColor(Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RareType);
                break;
        }
    }

}
