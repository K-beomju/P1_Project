using BackendData.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DungeonClearPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        Content_Item
    }

    public enum Buttons
    {
        Btn_Exit,
        Btn_NextLevel,
    }

    public enum Images 
    {
        Image_FeeIcon
    }

    public enum Texts 
    {
        Text_FeeAmount
    }

    
    private List<UI_ClearItem> _clearItems = new List<UI_ClearItem>();
    private EDungeonType DungeonType;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;            

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));

        GetObject((int)GameObjects.BG).BindEvent(OnClickButton);
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(OnClickButton);

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

        fadeUI.ShowFadeInOut(EFadeType.FadeInOut, 1, 1, 
        fadeOutCallBack: () => 
        {
            Managers.Scene.LoadScene(EScene.GameScene);
        });
    }

    public void RefreshUI(EDungeonType dungeonType, Dictionary<EItemType, int> clearItemDic)
    {
        DungeonType = dungeonType;
        StartCoroutine(CreateClearItem(clearItemDic));

        var dungeonData = Managers.Backend.GameData.DungeonData;
        GetButton((int)Buttons.Btn_NextLevel).onClick.RemoveAllListeners();
        GetButton((int)Buttons.Btn_NextLevel).onClick.AddListener(() =>
        {
            if(dungeonData.DungeonKeyDic[DungeonType.ToString()] < 1)
            {
                Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification($"{Util.GetDungeonType(DungeonType)}키가 부족합니다.");
                return;
            }
            Managers.Scene.LoadScene(EScene.DungeonScene);
            dungeonData.AddKey(DungeonType, -1);
        });

        EItemType itemType = DungeonType switch
        {
            EDungeonType.Gold => EItemType.GoldDungeonKey,
            EDungeonType.Dia => EItemType.DiaDungeonKey,
            _ => throw new ArgumentException($"Unknown DungeonKey: {DungeonType}")
        };

        GetImage((int)Images.Image_FeeIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{Managers.Data.ItemChart[itemType].SpriteKey}");
        GetTMPText((int)Texts.Text_FeeAmount).text = $"{dungeonData.DungeonKeyDic[DungeonType.ToString()]}";
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
