using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ClearPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        Content_Item
    }

    public enum Buttons
    {
        Btn_Check
    }

    
    private List<UI_ClearItem> _clearItems = new List<UI_ClearItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetObject((int)GameObjects.BG).BindEvent(OnClickButton);
        GetButton((int)Buttons.Btn_Check).onClick.AddListener(OnClickButton);

        for (int i = 0; i < 10; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_ClearItem>(GetObject((int)GameObjects.Content_Item).transform);
            item.gameObject.SetActive(false);
            _clearItems.Add(item);
        }
        return true;
    }

    private void OnClickButton()
    {
        var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
        fadeUI.sceneMove = true;
        Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_FADEPOPUP);

        fadeUI.ShowFadeInOut(Define.EFadeType.FadeInOut, 1, 1, 
        fadeOutCallBack: () => 
        {
            Managers.Scene.LoadScene(Define.EScene.GameScene);
        });
    }

    public void RefreshUI(Dictionary<EItemType, int> clearItemDic)
    {
        StartCoroutine(CreateClearItem(clearItemDic));
    }

    
    // Enum으로 획득할 아이템, Value 값은 갯수 -> 범용적으로 사용해야함 
    private IEnumerator CreateClearItem(Dictionary<EItemType, int> clearItemDic)
    {
        _clearItems.ForEach(item => item.gameObject.SetActive(false));

        int index = 0;
        foreach (var Item in clearItemDic)
        {
            if (index >= _clearItems.Count) break;

            UI_ClearItem clearItem = _clearItems[index];

            clearItem.DisplayItem(Managers.Data.ItemChart[Item.Key], Item.Value);
            clearItem.gameObject.SetActive(true);
            index++;
        }

        yield return null;
    }
}
