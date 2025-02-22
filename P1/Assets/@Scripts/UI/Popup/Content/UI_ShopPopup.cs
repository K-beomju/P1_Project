using BackendData.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ShopPopup : UI_Popup
{
    public enum Buttons
    {
        Btn_AdWatchedDia
    }

    public enum Texts
    {
        Text_AdWatchedDiaCount
    }


    public enum ShopPaidItems
    {
        // Dia
        UI_ShopPaidItemDia_40,
        UI_ShopPaidItemDia_220,
        UI_ShopPaidItemDia_480,
        UI_ShopPaidItemDia_1040,
        UI_ShopPaidItemDia_2800,
        UI_ShopPaidItemDia_6400,
        // Gold 
        UI_ShopPaidItemGold_Ad, // UI가 똑같아서 대체 
        UI_ShopPaidItemGold_10000,
        UI_ShopPaidItemGold_100000
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        //Bind<UI_ShopPaidItem>(typeof(ShopPaidItems));

        GetButton((int)Buttons.Btn_AdWatchedDia).onClick.AddListener(() => OnClickButtonAdWatch(EAdRewardType.Dia));


        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_40).SetInfo(ProductIDs.Dia40);
        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_220).SetInfo(ProductIDs.Dia220);
        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_480).SetInfo(ProductIDs.Dia480);
        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_1040).SetInfo(ProductIDs.Dia1040);
        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_2800).SetInfo(ProductIDs.Dia2800);
        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_6400).SetInfo(ProductIDs.Dia6400);

        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemGold_Ad).SetInfo(ProductIDs.GoldAd);
        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemGold_10000).SetInfo(ProductIDs.Gold10000);
        // Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemGold_100000).SetInfo(ProductIDs.Gold100000);

        return true;

    }

    private void OnClickButtonAdWatch(EAdRewardType rewardType)
    {
        if (Managers.Backend.GameData.ShopData.IsCheckWatch(rewardType))
        {
            Managers.Ad.ShowRewardedInterstitialAd(onRewardEarned =>
            {
                if (onRewardEarned)
                {
                    // 광고 시청 처리
                    Managers.Backend.GameData.ShopData.WatchAd(rewardType);

                    // 보상 지급 
                    if (rewardType == EAdRewardType.Dia)
                    {
                        Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, 500);
                    }
                    else if (rewardType == EAdRewardType.Gold)
                    {

                    }

                    // UI 업데이트 
                    RefreshUI();
                }
                else
                {
                    Debug.LogWarning("광고 시청 실패!");
                }
            });
        }
        else
        {
            ShowAlertUI("광고 시청 횟수가 모두 소진되었습니다");
        }
    }

    public void RefreshUI()
    {
        RewardAdData rewardData =  Managers.Backend.GameData.ShopData.RewardAdDic[EAdRewardType.Dia.ToString()];
        GetTMPText((int)Texts.Text_AdWatchedDiaCount).text =
        $"무료({rewardData.WatchedCount}/{rewardData.MaxCount})";
    }
}