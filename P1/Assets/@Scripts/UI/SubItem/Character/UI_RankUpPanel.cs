using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_RankUpPanel : UI_Base
{
    public enum Texts 
    {
        Text_MyRankName,
        Text_FeeAmount,
        Text_ChangeAbilityCount
    }

    public enum Images 
    {
        Image_MyRankIcon
    }

    public enum Buttons 
    {

    }

    // 추가 능력 슬롯 
    public enum RankAbility
    {
        UI_RankAbilityItem_Iron,
        UI_RankAbilityItem_Bronze,
        UI_RankAbilityItem_Gold,
        UI_RankAbilityItem_Dia,
        UI_RankAbilityItem_Master,
        UI_RankAbilityItem_GrandMaster
    }

    // 랭킹 도전 아이템 
    public enum RankChallenge
    {
        UI_RankChallengeItem_Iron,
        UI_RankChallengeItem_Bronze,
        UI_RankChallengeItem_Gold,
        UI_RankChallengeItem_Dia,
        UI_RankChallengeItem_Master,
        UI_RankChallengeItem_GrandMaster
    }

    public ERankType RankType { get; private set; } = ERankType.Unknown;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        Bind<UI_RankAbilityItem>(typeof(RankAbility));
        Bind<UI_RankChallengeItem>(typeof(RankChallenge));

        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Iron).SetInfo(ERankType.Iron);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Bronze).SetInfo(ERankType.Bronze);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Gold).SetInfo(ERankType.Gold);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Dia).SetInfo(ERankType.Dia);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Master).SetInfo(ERankType.Master);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_GrandMaster).SetInfo(ERankType.GrandMaster);

        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Iron).SetInfo(ERankType.Iron);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Bronze).SetInfo(ERankType.Bronze);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Gold).SetInfo(ERankType.Gold);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Dia).SetInfo(ERankType.Dia);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Master).SetInfo(ERankType.Master);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_GrandMaster).SetInfo(ERankType.GrandMaster);
        return true;
    }

    private void OnEnable() 
    {
        Managers.Event.AddEvent(EEventType.HeroRankUpdated, new Action(RefreshUI));
    }

    private void OnDisable() 
    {
        Managers.Event.RemoveEvent(EEventType.HeroRankUpdated, new Action(RefreshUI));
    }

    public void RefreshUI()
    {
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Iron).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Bronze).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Gold).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Dia).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Master).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_GrandMaster).RefreshUI();

        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Iron).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Bronze).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Gold).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Dia).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Master).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_GrandMaster).RefreshUI();

        ERankType rankType = Managers.Backend.GameData.RankUpData.GetCurrentRankType();
        GetImage((int)Images.Image_MyRankIcon).gameObject.SetActive(true);
        if(rankType == ERankType.Unknown)
        {
            GetTMPText((int)Texts.Text_MyRankName).text = "랭크 없음";
            GetImage((int)Images.Image_MyRankIcon).gameObject.SetActive(false);

        }
        else
        {
            GetImage((int)Images.Image_MyRankIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{rankType}");
            GetTMPText((int)Texts.Text_MyRankName).text = Managers.Data.RankUpChart[Managers.Backend.GameData.RankUpData.GetCurrentRankType()].Name;
        }
    }
}
