using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;
using System;
using UnityEngine.UI;

public class UI_ItemIconBase : UI_Base
{
    enum RectTransforms
    {
        Icon
    }

    private RectTransform _icon;
    private Image itemImage;
    private Canvas canvas;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        canvas = GetComponent<Canvas>();

        Bind<RectTransform>(typeof(RectTransforms));
        _icon = Get<RectTransform>((int)RectTransforms.Icon);
        itemImage = _icon.GetComponent<Image>();
        return true;
    }

    // 스크린 좌표 
    public void SetItemIconAtPosition(EItemType itemType, Vector3 startPosition, Action EndCallBack = null)
    {
        canvas.sortingOrder = SortingLayers.DAMAGE_FONT;
        itemImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{Managers.Data.ItemChart[itemType].SpriteKey}");
        _icon.position = Camera.main.WorldToScreenPoint(startPosition);
        Vector2 targetPosition = (Managers.UI.SceneUI as UI_GameScene).GetGoodItem(itemType).GetGoodIconWorldToCanvasLocalPosition();
        Explosion(_icon.anchoredPosition, targetPosition, 100f, EndCallBack);
    }

    // UI 좌표
    public void SetItemIconAtCanvasPosition(EItemType itemType, Vector2 startPosition, Action EndCallBack = null)
    {
        canvas.sortingOrder = 1000;
        itemImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{Managers.Data.ItemChart[itemType].SpriteKey}");
        _icon.anchoredPosition = startPosition;
        Vector2 targetPosition = (Managers.UI.SceneUI as UI_GameScene).GetGoodItem(itemType).GetGoodIconWorldToCanvasLocalPosition();
        Explosion(startPosition, targetPosition, 100f, EndCallBack);
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