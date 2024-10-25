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

    private enum RectTransforms
    {
        GoodIcon
    }

    private EGoodType goodType;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        Bind<RectTransform>(typeof(RectTransforms));
        return true;
    }

    public void SetInfo(EGoodType _goodType)
    {
        goodType = _goodType;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.CurrencyUpdated, new Action(RefreshGoodDisplayUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.CurrencyUpdated, new Action(RefreshGoodDisplayUI));
    }

    public void RefreshGoodDisplayUI()
    {
        if (goodType == EGoodType.None)
            return;

        if (!Managers.Backend.GameData.UserData.PurseDic.TryGetValue(goodType.ToString(), out float amount))
            return;

        GetTMPText((int)Texts.GoodText).text = Util.ConvertToTotalCurrency((long)amount);
    }

    public RectTransform GetGoodIcon()
    {
        return Get<RectTransform>((int)RectTransforms.GoodIcon);
    }

    public Vector2 GetGoodIconWorldToCanvasLocalPosition()
    {
        Vector2 canvasLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (Managers.UI.SceneUI).transform as RectTransform,                // 기준이 되는 캔버스의 RectTransform
            RectTransformUtility.WorldToScreenPoint(Camera.main, GetGoodIcon().position), // 월드 좌표를 스크린 좌표로 변환
            Camera.main,                                                  // 사용되는 카메라
            out canvasLocalPos);                                          // 결과로 얻는 캔버스 기준 로컬 좌표

        return canvasLocalPos;
    }
}
