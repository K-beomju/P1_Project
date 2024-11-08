using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RankingItem : UI_Base
{
    public enum Texts
    {
        Text_Rank,
        Text_NickName,
        Text_TotalDamage
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));

        return true;
    }

    public void RefreshUI(BackendData.Rank.RankUserItem rankUserItem)
    {
        GetTMPText((int)Texts.Text_Rank).text = rankUserItem.rank;
        GetTMPText((int)Texts.Text_NickName).text = rankUserItem.nickname;
        GetTMPText((int)Texts.Text_TotalDamage).text = Util.ConvertToTotalCurrency(long.Parse(rankUserItem.score));
    }
}
