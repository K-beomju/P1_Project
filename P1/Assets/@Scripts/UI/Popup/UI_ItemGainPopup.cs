using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class UI_ItemGainPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        Content_Item
    }

    private List<UI_GainedItem> _relicItems = new List<UI_GainedItem>();
    private List<UI_ClearItem> _clearItems = new List<UI_ClearItem>();


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        // GetObject((int)GameObjects.BG).BindEvent(() => 
        // {
        //     ClosePopupUI();
        //     (Managers.UI.SceneUI as UI_GameScene).ShowPopupActiveGameUI(true);

        // });
        for (int i = 0; i < 30; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_GainedItem>(GetObject((int)GameObjects.Content_Item).transform);
            item.gameObject.SetActive(false);
            _relicItems.Add(item);
        }

        for (int i = 0; i < 10; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_ClearItem>(GetObject((int)GameObjects.Content_Item).transform);
            item.gameObject.SetActive(false);
            _clearItems.Add(item);
        }

        return true;
    }

    public void ShowCreateRelicItem(Dictionary<EHeroRelicType, int> relicDic)
    {
        //(Managers.UI.SceneUI as UI_GameScene).ShowPopupActiveGameUI(false);
        StartCoroutine(CreateRelicItems(relicDic));
    }

    public void ShowCreateClearItem(Dictionary<EItemType, int> itemDic)
    {
        //(Managers.UI.SceneUI as UI_GameScene).ShowPopupActiveGameUI(false);
        StartCoroutine(CreateClearItems(itemDic));
    }

    /// <summary>
    /// 유물 아이템 생성
    /// </summary>
    private IEnumerator CreateRelicItems(Dictionary<EHeroRelicType, int> relicDic)
    {
        _relicItems.ForEach(item => item.gameObject.SetActive(false));

        int relicIndex = 0;
        foreach (var item in relicDic)
        {
            if (relicIndex < _relicItems.Count)
            {
                DisplayRelicItem(_relicItems[relicIndex], item);
                relicIndex++;
            }
        }

        Managers.Event.TriggerEvent(EEventType.HeroRelicUpdated);
        yield return null;
    }

    /// <summary>
    /// 클리어 아이템 (골드, 다이아) 생성
    /// </summary>
    private IEnumerator CreateClearItems(Dictionary<EItemType, int> itemDic)
    {
        _clearItems.ForEach(item => item.gameObject.SetActive(false));

        int clearIndex = 0;
        foreach (var item in itemDic)
        {
            if (clearIndex < _clearItems.Count)
            {
                DisplayClearItem(_clearItems[clearIndex], item);
                clearIndex++;
            }
        }

        yield return null;
    }

    private void DisplayRelicItem(UI_GainedItem relicItem, KeyValuePair<EHeroRelicType, int> item)
    {
        relicItem.DisplayItem(Managers.Data.HeroRelicChart[item.Key], item.Value);
        relicItem.gameObject.SetActive(true);
    }

    private void DisplayClearItem(UI_ClearItem clearItem, KeyValuePair<EItemType, int> item)
    {
        clearItem.DisplayItem(Managers.Data.ItemChart[item.Key], item.Value);
        clearItem.gameObject.SetActive(true);
    }
}


