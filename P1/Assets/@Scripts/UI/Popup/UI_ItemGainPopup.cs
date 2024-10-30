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

    private List<UI_GainedItem> _gainItems = new List<UI_GainedItem>();
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
            _gainItems.Add(item);
        }


        return true;
    }

    public void RefreshUI(EItemType type, Dictionary<Enum, int> gainItemDic)
    {
        _type = type;
        StartCoroutine(CreateGainItem(gainItemDic));
    }

    // Enum으로 획득할 아이템, Value 값은 갯수 -> 범용적으로 사용해야함 
    private IEnumerator CreateGainItem(Dictionary<Enum, int> gainItemDic)
    {
        _gainItems.ForEach(item => item.gameObject.SetActive(false));

        int index = 0;
        foreach (var gainItem in gainItemDic)
        {
            if (index >= _gainItems.Count) break;

            UI_GainedItem gainedItem = _gainItems[index];

            if (_type == EItemType.Relic)
            {
                gainedItem.DisplayItem(Managers.Data.HeroRelicChart[(EHeroRelicType)gainItem.Key], gainItem.Value);
            }
            gainedItem.gameObject.SetActive(true);
            index++;
        }

        yield return null;
    }
}
