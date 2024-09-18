using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class UI_HeroGrowthInvenSlot : UI_Base
{
    public enum Texts
    {
        Text_Level,
        Text_Amount,
        Text_Value,

        Text_Title,
        Text_UpgradeDesc
    }

    public enum Buttons
    {
        Btn_Upgrade
    }

    private EHeroUpgradeType _heroUpgradeType;
    private Coroutine _coolTime;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_Upgrade).gameObject.BindEvent(OnPressUpgradeButton, EUIEvent.Pressed);
        GetButton((int)Buttons.Btn_Upgrade).gameObject.BindEvent(OnPointerUp, EUIEvent.PointerUp);

        UpdateSlotInfoUI();
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.UpdateHeroUpgrade, new Action(UpdateSlotInfoUI));

        UpdateSlotInfoUI();
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.UpdateHeroUpgrade, new Action(UpdateSlotInfoUI));
    }

    public void SetInfo(EHeroUpgradeType statType)
    {
        _heroUpgradeType = statType;
        Debug.Log(_heroUpgradeType);

        if (Init() == false)
        {
            UpdateSlotInfoUI();
        }
    }

    public void UpdateSlotInfoUI()
    {
        string titleText = string.Empty;
        string levelText = string.Empty;
        string valueText = string.Empty;
        string amountText = string.Empty;

        if (Managers.Hero.HeroGrowthUpgradeLevelDic.TryGetValue(_heroUpgradeType, out int level))
        {
            titleText = $"{Util.GetHeroUpgradeString(_heroUpgradeType)}";
            levelText = $"Lv {level}";
            valueText = $"{Managers.Data.HeroUpgradeInfoDataDic[_heroUpgradeType].Value * (level + 1)}";
            amountText = $"{Util.GetUpgradeCost(_heroUpgradeType, level + 1):N0}";
        }
        else
        {
            titleText = $"{Util.GetHeroUpgradeString(_heroUpgradeType)}";
            levelText = $"Lv {0}";
            valueText = $"{Managers.Data.HeroUpgradeInfoDataDic[_heroUpgradeType].Value * (1)}";
            amountText = $"{Util.GetUpgradeCost(_heroUpgradeType, 1):N0}";
        }
        GetText((int)Texts.Text_Title).text = titleText;
        GetText((int)Texts.Text_Level).text = levelText;
        GetText((int)Texts.Text_Value).text = valueText;
        GetText((int)Texts.Text_Amount).text = amountText;
    }

    private void OnPressUpgradeButton()
    {
        Managers.Hero.LevelUpHeroUpgrade(_heroUpgradeType);

        // if (_coolTime == null)
        // {

        //     Managers.Hero.LevelUpHeroUpgrade(_heroUpgradeType);

        //     _coolTime = StartCoroutine(CoStartUpgradeCoolTime(0.3f));

        // }
    }

    private void OnPointerUp()
    {
        if (_coolTime != null)
        {
            StopCoroutine(_coolTime);
            _coolTime = null;
        }
    }


    private IEnumerator CoStartUpgradeCoolTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coolTime = null;
    }
}
