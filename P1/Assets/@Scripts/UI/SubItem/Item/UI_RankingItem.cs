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

    public enum Images
    {
        Image_Medal,
        Image_RankOutLine
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        return true;
    }

    public void RefreshUI(BackendData.Rank.RankUserItem rankUserItem)
    {
        GetTMPText((int)Texts.Text_Rank).text = rankUserItem.rank;
        GetTMPText((int)Texts.Text_NickName).text = rankUserItem.nickname;
        GetTMPText((int)Texts.Text_TotalDamage).text = Util.ConvertToTotalCurrency(long.Parse(rankUserItem.score));

        // 3위권이라면 
        switch (int.Parse(rankUserItem.rank))
        {
            case 1:
                GetImage((int)Images.Image_RankOutLine).gameObject.SetActive(true);
                GetImage((int)Images.Image_RankOutLine).sprite = Managers.Resource.Load<Sprite>("Sprites/Rank/FirstTap");

                GetImage((int)Images.Image_Medal).sprite = Managers.Resource.Load<Sprite>("Sprites/Rank/medal1");
                GetImage((int)Images.Image_Medal).rectTransform.sizeDelta = new Vector2(36, 50);

                break;
            case 2:
                GetImage((int)Images.Image_RankOutLine).gameObject.SetActive(true);
                GetImage((int)Images.Image_RankOutLine).sprite = Managers.Resource.Load<Sprite>("Sprites/Rank/SecondTap");

                GetImage((int)Images.Image_Medal).sprite = Managers.Resource.Load<Sprite>("Sprites/Rank/medal2");
                GetImage((int)Images.Image_Medal).rectTransform.sizeDelta = new Vector2(36, 50);

                break;
            case 3:
                GetImage((int)Images.Image_RankOutLine).gameObject.SetActive(true);
                GetImage((int)Images.Image_RankOutLine).sprite = Managers.Resource.Load<Sprite>("Sprites/Rank/ThirdTap");

                GetImage((int)Images.Image_Medal).sprite = Managers.Resource.Load<Sprite>("Sprites/Rank/medal3");
                GetImage((int)Images.Image_Medal).rectTransform.sizeDelta = new Vector2(36, 50);

                break;
            default:
                GetImage((int)Images.Image_RankOutLine).gameObject.SetActive(false);
                GetImage((int)Images.Image_Medal).sprite = Managers.Resource.Load<Sprite>("Sprites/Rank/helmet");
                GetImage((int)Images.Image_Medal).rectTransform.sizeDelta = new Vector2(36, 40);
                break;
        }
    }
}
