
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_IdleRewardPopup : UI_Popup
{
    public enum Buttons
    {
        Btn_Receive,
        Btn_Receive2xAd
    }

    public enum GameObjects
    {
        ContentItem
    }

    public enum Texts
    {
        Text_IdleTime,
        Text_Gold
    }

    private Dictionary<EItemType, double> _itemDic = new();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_Receive).onClick.AddListener(() => OnClickButtonReceive(false));
        GetButton((int)Buttons.Btn_Receive2xAd).onClick.AddListener(() => OnClickButtonReceive(true));
        return true;
    }

    private void OnClickButtonReceive(bool adFlag)
    {
        if (adFlag)
        {
            Managers.Ad.ShowRewardedInterstitialAd(onRewardEarned =>
            {
                if (onRewardEarned)
                {
                    StartCoroutine(Managers.Ad.ExecuteAfterFrame(() =>
                    {                           
                        foreach (var item in _itemDic)
                        {
                           if (item.Key != EItemType.Exp)
                               Managers.Backend.GameData.CharacterData.AddAmount(item.Key, item.Value * 2);
                           else
                               Managers.Backend.GameData.CharacterData.AddExp(item.Value * 2);
                        }
                        ClosePopupUI();
                    }));

                }
                else
                {
                    ShowAlertUI("광고 시청 실패!");
                }
            });
        }
        else
        {
            foreach (var item in _itemDic)
            {
                if (item.Key != EItemType.Exp)
                    Managers.Backend.GameData.CharacterData.AddAmount(item.Key, item.Value);
                else
                    Managers.Backend.GameData.CharacterData.AddExp(item.Value);

                ClosePopupUI();
            }
        }


    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();

        // Clear 보상 재화 연출 
        for (int i = 0; i < 10; i++)
        {
            UI_ItemIconBase itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
            itemIcon.SetItemIconAtCanvasPosition(EItemType.Dia, Vector2.zero);
        }

        for (int i = 0; i < 10; i++)
        {
            UI_ItemIconBase itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
            itemIcon.SetItemIconAtCanvasPosition(EItemType.Gold, Vector2.zero);
        }

        // 데이터 저장
        Managers.Backend.UpdateAllGameData(callback => { });
    }


    public void ShowIdleReward(Dictionary<EItemType, double> itemDic, int idleMinutes)
    {
        GetTMPText((int)Texts.Text_IdleTime).text = $"{idleMinutes}분 동안 수집";
        GetTMPText((int)Texts.Text_Gold).text = $"{Util.ConvertToTotalCurrency(itemDic[EItemType.Gold])}";

        _itemDic = itemDic;

        foreach (var item in itemDic)
        {
            if (item.Key == EItemType.Gold)
                continue;

            var clearItem = Managers.UI.MakeSubItem<UI_ClearItem>(GetObject((int)GameObjects.ContentItem).transform);
            clearItem.DisplayItem(Managers.Data.ItemChart[item.Key], item.Value);
        }
    }
}
