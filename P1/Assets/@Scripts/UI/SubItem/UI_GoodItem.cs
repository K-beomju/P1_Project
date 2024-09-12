using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GoodItem : UI_Base
{
    private enum Texts
    {
        GoodText
    }

    private EGoodType goodType;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        return true;
    }

    public void SetInfo(EGoodType _goodType)
    {
        goodType = _goodType;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.UpdateCurrency, new Action(RefreshGoodDisplayUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.UpdateCurrency, new Action(RefreshGoodDisplayUI));
    }

    public void RefreshGoodDisplayUI()
    {
        if (goodType == EGoodType.None)
            return;

        GetText((int)Texts.GoodText).text = $"{Managers.Game.GetAmount(goodType):N0}";
    }
}
