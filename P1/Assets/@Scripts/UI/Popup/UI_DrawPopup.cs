using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public struct EquipmentDrawResult
{
    public ERareType RareType { get; }
    public int EquipmentIndex { get; }

    public EquipmentDrawResult(ERareType rareType, int equipmentIndex)
    {
        RareType = rareType;
        EquipmentIndex = equipmentIndex;
    }
}


public class UI_DrawPopup : UI_Popup
{
    public enum Texts
    {
        Text_DrawLevel,
        Text_DrawValue
    }

    public enum Sliders
    {
        Slider_DrawCount
    }

    public enum Buttons
    {
        Btn_DrawOnce,
        Btn_DrawTen,
        Btn_DrawThirty
    }

    private GameData gameData;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Btn_DrawOnce).onClick.AddListener(() => OnDrawEquipment(1));
        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawEquipment(30));

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.UpdateDrawUI, new Action(RefreshUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.UpdateDrawUI, new Action(RefreshUI));
    }


    public void RefreshUI()
    {
        if (gameData == null)
            gameData = Managers.Game.PlayerGameData;

        int level = gameData.DrawLevel;
        int drawCount = gameData.DrawCount;

        GetText((int)Texts.Text_DrawLevel).text = $"Lv. {Managers.Game.PlayerGameData.DrawLevel}";
        GetText((int)Texts.Text_DrawValue).text = $"{drawCount} / {Managers.Data.GachaDataDic[level].MaxExp}";
        GetSlider((int)Sliders.Slider_DrawCount).maxValue = Managers.Data.GachaDataDic[level].MaxExp;
        GetSlider((int)Sliders.Slider_DrawCount).value = drawCount;
    }


    public void OnDrawEquipment(int count)
    {
        if (gameData == null)
            gameData = Managers.Game.PlayerGameData;

        var popupUI = Managers.UI.ShowPopupUI<UI_DrawResultPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_RESULTPOPUP);
        popupUI.RefreshUI(Util.GetEquipmentDrawResults(count, gameData.DrawLevel));
        Managers.Event.TriggerEvent(EEventType.UpdateDraw, count);
    }




}
