using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using DG;
using DG.Tweening;

public class UI_PointerBase : UI_Base
{
    public enum RectTransforms
    {
        Pointer
    }

    private RectTransform _pointer;
    private Tween _pointerTween;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        GetComponent<Canvas>().sortingOrder = SortingLayers.UI_TOTALPOWER;

        Bind<RectTransform>(typeof(RectTransforms));
        _pointer = Get<RectTransform>((int)RectTransforms.Pointer);
        return true;
    }
    
    public void SetPosition(Vector2 position)
    {
        _pointer.anchoredPosition = position;

        // 기존 애니메이션 중지
        _pointerTween?.Kill();

        // 위아래로 움직이는 애니메이션 추가
        _pointerTween = _pointer.DOAnchorPosY(position.y + 10f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
