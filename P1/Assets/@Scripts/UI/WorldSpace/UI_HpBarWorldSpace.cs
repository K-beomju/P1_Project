using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBarWorldSpace : UI_Base
{
    private enum Sliders
    {
        HpSlider,
        SkillCoolSlider,
    }

    private Creature _owner;
    private Slider _hpBarSlider;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindSliders(typeof(Sliders));
        _hpBarSlider = GetSlider((int)Sliders.HpSlider);

        return true;
    }

    public void SetSliderInfo(Creature owner)
    {
        _owner = owner;
    }

    private void LateUpdate()
    {
        if (_owner == null)
        {
            return;
        }

        float hpAmount = _owner.Hp / _owner.MaxHp;
        _hpBarSlider.value = hpAmount;
    }
}
