using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_HpBarWorldSpace : UI_Base
{
    private enum Sliders
    {
        HpSlider,
        DamageSlider,
    }

    private enum Images
    {
        HpFill
    }

    private Creature _owner;
    private Slider _hpBarSlider;
    private Slider _damageSlider;
    private CanvasGroup _canvasGroup;
    public Vector3 _offset { get; set; }
    private float _smoothSpeed = 0.3f;
    private bool _isHpBarVisible = false; // 슬라이더가 현재 보이는 상태인지 확인하는 플래그

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindSliders(typeof(Sliders));
        BindImages(typeof(Images));
        _canvasGroup = GetComponent<CanvasGroup>();

        _hpBarSlider = GetSlider((int)Sliders.HpSlider);
        _damageSlider = GetSlider((int)Sliders.DamageSlider);

        Canvas canvas = GetComponent<Canvas>();
        canvas.sortingOrder = SortingLayers.UI_HPBAR;
        return true;
    }

    public void SetSliderInfo(Creature owner)
    {
        _owner = owner;
        if (owner.ObjectType == EObjectType.Hero)
            GetImage((int)Images.HpFill).color = Util.HexToColor("#58FF58");
        else if (owner.ObjectType == EObjectType.Monster)
            GetImage((int)Images.HpFill).color = Util.HexToColor("#FF5858");
    }

    private void LateUpdate()
    {
        if (_owner == null)
        {
            return;
        }

        if (_owner.ObjectType == EObjectType.Hero)
        {
            Vector3 worldPosition = _owner.transform.position + _offset;
            transform.position = worldPosition;  // 월드 좌표로 HP바 위치 설정
        }
        if (_owner.ObjectType == EObjectType.Monster)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(_owner.transform.localScale.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        double hpAmount = _owner.Hp / _owner.MaxHp;
        _hpBarSlider.value = (float)hpAmount;

        // DamageSlider는 천천히 HpSlider를 따라가며 감소
        if (_damageSlider.value > _hpBarSlider.value)
        {
            _damageSlider.DOValue(_hpBarSlider.value, _smoothSpeed);
        }
    }

    public void DoFadeSlider(Action CallBackAction = null)
    {
        if (_owner.ObjectType != EObjectType.Hero)
            return;

        if (!_isHpBarVisible)
        {
            _canvasGroup.alpha = 1; // 슬라이더 보이게 설정
            _isHpBarVisible = true; // 상태 플래그 업데이트
        }

        // 슬라이더 페이드 아웃 처리
        _canvasGroup.DOKill(); // 기존 페이드 아웃 애니메이션 중단
        _canvasGroup.DOFade(0, 0.5f).SetDelay(1f).OnComplete(() =>
        {
            _isHpBarVisible = false; // 슬라이더가 사라진 후 상태 플래그 업데이트
            CallBackAction?.Invoke();
        });
    }
}
