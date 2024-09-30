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

    private List<UI_EquipmentItem> _drawItems = new List<UI_EquipmentItem>();
    private EEquipmentType _type;

    private int _level;
    private int _drawCount;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() =>
        {
            ClosePopupUI();
            Managers.Event.TriggerEvent(EEventType.DrawEquipmentUIUpdated);
        });
        GetButton((int)Buttons.Btn_RetryDraw).onClick.AddListener(() => RetryDrawEquipment());
        for (int i = 0; i < 30; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentItem>(GetObject((int)GameObjects.DrawItemGroup).transform);
            item.gameObject.SetActive(false);
            _drawItems.Add(item);
        }

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {level}"));
    }

    private void OnDisable()
    {
        Managers.Event.AddEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
         GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {level}"));
    }

    public void RefreshUI(EEquipmentType type, int drawCount, List<int> resultList)
    {
        _type = type;
        _level = Managers.Game.PlayerGameData.DrawData[_type].Level;  // 최신 레벨 가져오기
        _drawCount = drawCount;

        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {_level}";
        GetTMPText((int)Texts.Text_Retry).text = $"{_drawCount}회";

        InteractiveButtons(false);
        StartCoroutine(CreateEquipmentItem(resultList));
    }

    private void RetryDrawEquipment()
    {
        List<int> resultList = Util.GetEquipmentDrawResults(_type, _drawCount, _level);
        RefreshUI(_type, _drawCount, resultList);
    }

    private IEnumerator CreateEquipmentItem(List<int> resultList)
    {
        for (int i = 0; i < _drawItems.Count; i++)
        {
            _drawItems[i].gameObject.SetActive(false);
        }

        WaitForSeconds wait = new WaitForSeconds(CREATRE_EQUIPMENT_DELAY);

        for (int i = 0; i < resultList.Count; i++)
        {
            UI_EquipmentItem drawItem = _drawItems[i];
            drawItem.gameObject.SetActive(true);
            drawItem.SetDrawInfo(Managers.Equipment.GetEquipmentInfo(resultList[i]));
            Managers.Equipment.AddEquipment(resultList[i]);

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
