using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;
using System;

public class UI_GoldIconBase : UI_Base
{
    enum RectTransforms
    {
        Icon
    }

    private RectTransform _icon;
    private Canvas canvas;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        canvas = GetComponent<Canvas>();
        canvas.sortingOrder = SortingLayers.DAMAGE_FONT;

        Bind<RectTransform>(typeof(RectTransforms));
        _icon = Get<RectTransform>((int)RectTransforms.Icon);
        return true;
    }

    public void SetGoldIconAtPosition(Vector3 enemyPosition,  Action EndCallBack = null)
    {
        // 적의 월드 좌표에서 골드 아이콘 생성 위치로 설정
        _icon.position = Camera.main.WorldToScreenPoint(enemyPosition);
        Vector2 targetPosition = (Managers.UI.SceneUI as UI_GameScene).GetGoodItem(EGoodType.Gold).GetGoodIconWorldToCanvasLocalPosition();
        Explosion(_icon.anchoredPosition, targetPosition, 100f, EndCallBack); 
    }

    public void Explosion(Vector2 from, Vector2 to, float explo_range, Action EndCallBack = null)
    {
        _icon.anchoredPosition = from;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_icon.DOAnchorPos(from + UnityEngine.Random.insideUnitCircle * explo_range, 0.25f).SetEase(Ease.OutCubic));
        sequence.Append(_icon.DOAnchorPos(to, 0.5f).SetEase(Ease.InCubic));
        sequence.AppendCallback(() => EndCallBack());
        sequence.AppendCallback(() => { Managers.Pool.Push(this.gameObject); });
    }
}
