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
        Managers.Event.AddEvent(EEventType.CurrencyUpdated, new Action(CheckUpgradeInteractive));
        Managers.Event.AddEvent(EEventType.HeroUpgradeUpdated, new Action(UpdateSlotInfoUI));

        UpdateSlotInfoUI();
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.CurrencyUpdated, new Action(CheckUpgradeInteractive));
        Managers.Event.RemoveEvent(EEventType.HeroUpgradeUpdated, new Action(UpdateSlotInfoUI));
    }

    public void SetInfo(EHeroUpgradeType statType)
    {
        _heroUpgradeType = statType;
        if (_init == false)
            return;
        UpdateSlotInfoUI();

    }

    public void UpdateSlotInfoUI()
    {
        if (!Managers.Hero.HeroGrowthUpgradeLevelDic.TryGetValue(_heroUpgradeType, out int level))
        {
            level = 0; // 레벨이 없으면 0으로 초기화
        }
        CheckUpgradeInteractive();
        string titleText = $"{Util.GetHeroUpgradeString(_heroUpgradeType)}";
        string levelText = $"Lv {level}";
        string valueText = 
            $"{Managers.Data.HeroUpgradeInfoDataDic[_heroUpgradeType].Value + (Managers.Data.HeroUpgradeInfoDataDic[_heroUpgradeType].IncreaseValue) * (level - 1)}";
        string amountText = $"{Util.GetUpgradeCost(_heroUpgradeType, level + 1):N0}";

        GetText((int)Texts.Text_Title).text = titleText;
        GetText((int)Texts.Text_Level).text = levelText;
        GetText((int)Texts.Text_Value).text = valueText;
        GetText((int)Texts.Text_Amount).text = amountText;
    }

    private void OnPressUpgradeButton()
    {
        if (_coolTime != null)
            return;


        if (Managers.Hero.HeroGrowthUpgradeLevelDic.TryGetValue(_heroUpgradeType, out int level))
        {
            int price = Util.GetUpgradeCost(_heroUpgradeType, level + 1);
            if (CanUpgrade(price))
            {
                Managers.Purse.AddAmount(EGoodType.Gold, -price);
                Managers.Hero.LevelUpHeroUpgrade(_heroUpgradeType);
            }
        }
        _coolTime = StartCoroutine(CoStartUpgradeCoolTime(0.3f));

    }

    private void OnPointerUp()
    {
        if (_coolTime != null)
        {
            StopCoroutine(_coolTime);
            _coolTime = null;
        }
    }

    private void CheckUpgradeInteractive()
    {
        if (Managers.Hero.HeroGrowthUpgradeLevelDic.TryGetValue(_heroUpgradeType, out int level))
        {
            int price = Util.GetUpgradeCost(_heroUpgradeType, level + 1);
            GetButton((int)Buttons.Btn_Upgrade).interactable = CanUpgrade(price);
        }
    }

    bool CanUpgrade(int cost)
    {
        return Managers.Purse.GetAmount(EGoodType.Gold) >= cost;
    }

    private IEnumerator CoStartUpgradeCoolTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coolTime = null;
    }
}
