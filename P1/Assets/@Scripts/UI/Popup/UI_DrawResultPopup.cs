using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DrawResultPopup : UI_Popup
{
    public enum GameObjects
    {
        DrawItemGroup
    }

    public enum Texts
    {
        Text_DrawLevel,
        Text_Retry
    }

    public enum Buttons
    {
        Btn_Exit,
        Btn_AutoDraw,
        Btn_RetryDraw
    }

    private List<UI_EquipmentDrawItem> _drawItems = new List<UI_EquipmentDrawItem>();
    private int retryValue;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() => 
        {
            ClosePopupUI();
            Managers.Event.TriggerEvent(EEventType.UpdateDrawUI);
        });
        GetButton((int)Buttons.Btn_RetryDraw).onClick.AddListener(() => RetryDrawEquipment());
        for (int i = 0; i < 30; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentDrawItem>(GetObject((int)GameObjects.DrawItemGroup).transform);
            item.gameObject.SetActive(false);
            _drawItems.Add(item);
        }
        retryValue = 0;

        return true;
    }

    public void RefreshUI(List<EquipmentDrawResult> resultList)
    {
        GetText((int)Texts.Text_DrawLevel).text = $"Lv. {Managers.Game.PlayerGameData.DrawLevel}";
        GetText((int)Texts.Text_Retry).text = $"{resultList.Count}회";

        retryValue = resultList.Count;

        StartCoroutine(CreatreEquipmentItem(resultList));
    }

    private void RetryDrawEquipment()
    {
        List<EquipmentDrawResult> resultList = Util.GetEquipmentDrawResults(retryValue, Managers.Game.PlayerGameData.DrawLevel);
        RefreshUI(resultList);
    }

    private IEnumerator CreatreEquipmentItem(List<EquipmentDrawResult> resultList)
    {
        _drawItems.ForEach((item) => item.gameObject.SetActive(false));
        WaitForSeconds wait = new WaitForSeconds(CREATRE_EQUIPMENT_DELAY);
        for (int i = 0; i < resultList.Count; i++)
        {
            UI_EquipmentDrawItem drawItem = _drawItems[i];
            drawItem.gameObject.SetActive(true);
            drawItem.SetInfo(resultList[i]);
            yield return wait;
        }
    }


}
