using System;
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
    private EEquipmentType _type;

    private int _level;
    private int _count;

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
            Managers.Event.TriggerEvent(EEventType.DrawUIUpdated);
        });
        GetButton((int)Buttons.Btn_RetryDraw).onClick.AddListener(() => RetryDrawEquipment());
        for (int i = 0; i < 30; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentDrawItem>(GetObject((int)GameObjects.DrawItemGroup).transform);
            item.gameObject.SetActive(false);
            _drawItems.Add(item);
        }

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
        GetText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {level}"));
    }

    private void OnDisable()
    {
        Managers.Event.AddEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
         GetText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {level}"));
    }

    public void RefreshUI(EEquipmentType type, int level, int count, List<EquipmentDrawResult> resultList)
    {
        _type = type;
        _level = level;
        _count = count;

        Managers.Event.TriggerEvent(EEventType.DrawLevelUpUIUpdated, level);
        GetText((int)Texts.Text_Retry).text = $"{_count}회";

        InteractiveButtons(false);
        StartCoroutine(CreateEquipmentItem(resultList));
    }

    private void RetryDrawEquipment()
    {
        List<EquipmentDrawResult> resultList = Util.GetEquipmentDrawResults(_type, _count, _level);
        RefreshUI(_type, _level, _count, resultList);
    }

    private IEnumerator CreateEquipmentItem(List<EquipmentDrawResult> resultList)
    {
        for (int i = 0; i < _drawItems.Count; i++)
        {
            _drawItems[i].gameObject.SetActive(false);
        }

        WaitForSeconds wait = new WaitForSeconds(CREATRE_EQUIPMENT_DELAY);

        for (int i = 0; i < resultList.Count; i++)
        {
            UI_EquipmentDrawItem drawItem = _drawItems[i];
            drawItem.gameObject.SetActive(true);
            drawItem.SetInfo(resultList[i]);

            // 이벤트 호출
            Managers.Event.TriggerEvent(EEventType.DrawDataUpdated, _type);

            yield return wait;
        }
        InteractiveButtons(true);
    }

    public void InteractiveButtons(bool active)
    {
        for (int i = 0; i < 3; i++)
        {
            GetButton(i).interactable = active;
        }
    }


}
