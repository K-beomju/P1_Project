using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using Data;
using UnityEngine;
using static Define;

public class UI_CharacterGrowthInvenSlot : UI_Base
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

        BindTMPTexts(typeof(Texts));
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

        if(Init() == false)
        {
            UpdateSlotInfoUI();
        }
    }

    public void UpdateSlotInfoUI()
    {
        if (!Managers.Hero.HeroGrowthUpgradeLevelDic.TryGetValue(_heroUpgradeType, out int level))
        {
            level = 0; // 레벨이 없으면 0으로 초기화
        }

        CheckUpgradeInteractive();
        string percentString = "%";
        string titleText = $"{Util.GetHeroUpgradeString(_heroUpgradeType)}";
        string levelText = $"Lv {level}";
        string valueText = 
            $"{Managers.Data.HeroUpgradeInfoDataDic[_heroUpgradeType].Value + (Managers.Data.HeroUpgradeInfoDataDic[_heroUpgradeType].IncreaseValue) * (level - 1)}";
        string amountText = $"{Util.GetUpgradeCost(_heroUpgradeType, level + 1):N0}";

        GetTMPText((int)Texts.Text_Title).text = titleText;
        GetTMPText((int)Texts.Text_Level).text = levelText;
        if (_heroUpgradeType == EHeroUpgradeType.Growth_CriRate || _heroUpgradeType == EHeroUpgradeType.Growth_CriDmg)
            valueText += percentString;

        GetTMPText((int)Texts.Text_Value).text = valueText;
        GetTMPText((int)Texts.Text_Amount).text = amountText;
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
                try
                {
                    Managers.Backend.GameData.UserData.AddAmount(EGoodType.Gold, -price);
                    Managers.Hero.LevelUpHeroUpgrade(_heroUpgradeType);
                }
                catch (Exception e)
                {
                    throw new Exception($"OnPressUpgradeButton({EGoodType.Gold}, {-price}) 중 에러가 발생하였습니다\n{e}");
                }
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

    bool CanUpgrade(float cost)
    {
        if (!Managers.Backend.GameData.UserData.PurseDic.TryGetValue(EGoodType.Gold.ToString(), out float amount))
            return false;

          return amount >= cost;
    }

    private IEnumerator CoStartUpgradeCoolTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coolTime = null;
    }
}
