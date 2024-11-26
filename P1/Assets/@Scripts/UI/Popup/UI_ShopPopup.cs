using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPopup : UI_Popup
{
    public enum Buttons
    {
        Btn_AdFreeDia200
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.Ad.LoadRewardedInterstitialAd();

        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Btn_AdFreeDia200).onClick.AddListener(() => 
        {
            Managers.Ad.ShowRewardedInterstitialAd(onRewardEarned: (success) =>
            {
                if(success)
                {
                    Debug.Log("보상형 광고 시청 다이아 200개 지급");
                }
                else
                {

                }
            });
        });

        return true;

    }
}