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

    private EItemType _type;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        GetObject((int)GameObjects.BG).BindEvent(() =>
        {
            if (_type == EItemType.Relic)
                Managers.Event.TriggerEvent(EEventType.HeroRelicUpdated);

            ClosePopupUI();

        }, Define.EUIEvent.Click);

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

    public void RefreshUI(EItemType type, Dictionary<Enum, int> itemDic)
    {
        _type = type;
        StartCoroutine(CreateGainItem(itemDic));
    }

    // Enum으로 획득할 아이템, Value 값은 갯수 -> 범용적으로 사용해야함 
    private IEnumerator CreateGainItem(Dictionary<Enum, int> itemDic)
    {
        _relicItems.ForEach(item => item.gameObject.SetActive(false));
        _clearItems.ForEach(item => item.gameObject.SetActive(false));

        int index = 0;
        foreach (var item in itemDic)
        {
            if (_type == EItemType.Relic && index < _relicItems.Count)
            {
                DisplayRelicItem(_relicItems[index], item);
            }
            else if ((_type == EItemType.Gold || _type == EItemType.Dia) && index < _clearItems.Count)
            {
                DisplayClearItem(_clearItems[index], item);
            }
            index++;
        }

        yield return null;
    }

    private void DisplayRelicItem(UI_GainedItem relicItem, KeyValuePair<Enum, int> item)
    {
        relicItem.DisplayItem(Managers.Data.HeroRelicChart[(EHeroRelicType)item.Key], item.Value);
        relicItem.gameObject.SetActive(true);
    }

    private void DisplayClearItem(UI_ClearItem clearItem, KeyValuePair<Enum, int> item)
    {
        clearItem.DisplayItem(Managers.Data.ItemChart[(EItemType)item.Key], item.Value);
        clearItem.gameObject.SetActive(true);
    }
}


