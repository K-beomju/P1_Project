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
        RefreshUI();
        return true;
    }

    public void SetInfo(Data.ShopData shopData)
    {
        ShopData = shopData;

        if(Init() == false)
        {
            RefreshUI();
        }
    }

    private void OnClickButtonBuyItem()
    {
        Debug.Log($"{ShopData.ItemType} : {ShopData.Amount}");
    }

    public void RefreshUI()
    {
        if(ShopData == null)
        {
            Debug.LogWarning("ShopData가 없습니다.");
            return;
        }
        GetTMPText((int)Texts.Text_ItemName).text = ShopData.Remark;
        GetTMPText((int)Texts.Text_Amount).text = ShopData.Amount.ToString();
        GetTMPText((int)Texts.Text_PreAmount).text = (ShopData.Amount * 0.5f).ToString();

        GetTMPText((int)Texts.Text_Price).text = ShopData.Price.ToString();
    }
}
