using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopPaidItem : UI_Base
{
    public enum Texts
    {
        Text_Amount,
        Text_PreAmount,
        Text_ItemName,
        Text_Price
    }


    private Data.ShopData ShopData;
    private Button buyButton;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));
        buyButton = GetComponent<Button>();
        buyButton.onClick.AddListener(OnClickButtonBuyItem);
        return true;
    }

    public void SetInfo(string ShopItemName)
    {
        ShopData = Managers.Data.ShopChart[ShopItemName];

        if(Init() == false)
        {
            RefreshUI();
        }
    }

    private void OnClickButtonBuyItem()
    {
        Managers.IAP.Purchase(ShopData.ShopItemName);
    }

    public void RefreshUI()
    {
        if(ShopData == null)
            return;
        
        GetTMPText((int)Texts.Text_ItemName).text = ShopData.Remark;
        GetTMPText((int)Texts.Text_Amount).text = ShopData.Amount.ToString();
        GetTMPText((int)Texts.Text_PreAmount).text = (ShopData.Amount * 0.5f).ToString();

        GetTMPText((int)Texts.Text_Price).text = ShopData.Price.ToString();
    }
}
