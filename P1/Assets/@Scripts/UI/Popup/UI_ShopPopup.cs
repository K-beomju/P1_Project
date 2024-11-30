using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_40).SetInfo(ProductIDs.Dia40);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_220).SetInfo(ProductIDs.Dia220);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_480).SetInfo(ProductIDs.Dia480);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_1040).SetInfo(ProductIDs.Dia1040);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_2800).SetInfo(ProductIDs.Dia2800);
        Get<UI_ShopPaidItem>((int)ShopPaidItems.UI_ShopPaidItemDia_6400).SetInfo(ProductIDs.Dia6400);
        return true;

    }
}