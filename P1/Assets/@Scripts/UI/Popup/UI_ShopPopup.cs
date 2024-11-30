using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPopup : UI_Popup
{
    public enum ShopPaidItems 
    {
        // Ad 
        // ex : UI_ShopAdItem

        /// Paid 
        // Dia
        UI_ShopPaidItemDia_40,
        UI_ShopPaidItemDia_220,
        UI_ShopPaidItemDia_480,
        UI_ShopPaidItemDia_1040,
        UI_ShopPaidItemDia_2800,
        UI_ShopPaidItemDia_6400,
        // Gold 
        // UI_ShopPaidItemGold_10000,
        // UI_ShopPaidItemGold_100000
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_ShopPaidItem>(typeof(ShopPaidItems));

        var shopChart = Managers.Data.ShopChart;

        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_40).SetInfo(shopChart["Dia40"]);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_220).SetInfo(shopChart["Dia220"]);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_480).SetInfo(shopChart["Dia480"]);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_1040).SetInfo(shopChart["Dia1040"]);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_2800).SetInfo(shopChart["Dia2800"]);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_6400).SetInfo(shopChart["Dia6400"]);
        return true;

    }
}