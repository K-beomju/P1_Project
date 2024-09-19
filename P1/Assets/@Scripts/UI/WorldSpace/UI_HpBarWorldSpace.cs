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
    public float smoothSpeed = 0.1f;  // DamageSlider가 천천히 줄어들도록 하는 속도

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindSliders(typeof(Sliders));
        BindImages(typeof(Images));

        _hpBarSlider = GetSlider((int)Sliders.HpSlider);
        _damageSlider = GetSlider((int)Sliders.DamageSlider);

        Canvas canvas = GetComponent<Canvas>();
        // 애매함 
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

        float hpAmount = _owner.Hp / _owner.MaxHp;
        _hpBarSlider.value = hpAmount;

        // DamageSlider는 천천히 HpSlider를 따라가며 감소
        if (_damageSlider.value > _hpBarSlider.value)
        {
            _damageSlider.value = Mathf.Lerp(_damageSlider.value, _hpBarSlider.value, smoothSpeed * Time.deltaTime);
        }
    }
}
