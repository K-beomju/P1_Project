using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
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
    private bool isUpgraded = false;

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

        if (Init() == false)
        {
            UpdateSlotInfoUI();
        }
    }

    public void UpdateSlotInfoUI()
    {
        if (!Managers.Backend.GameData.UserData.UpgradeStatDic.TryGetValue(_heroUpgradeType.ToString(), out int level))
        {
            Debug.LogWarning($"UpdateSlotInfoUI도중 {level}이 없습니다");
            return;
        }

        CheckUpgradeInteractive();
        string percentString = "%";
        string titleText = $"{Util.GetHeroUpgradeString(_heroUpgradeType)}";
        string levelText = $"Lv {level}";
        string valueText =
            $"{Managers.Data.HeroUpgradeChart[_heroUpgradeType].Value + (Managers.Data.HeroUpgradeChart[_heroUpgradeType].IncreaseValue) * (level - 1)}";
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

        try
        {

            if (Managers.Backend.GameData.UserData.UpgradeStatDic.TryGetValue(_heroUpgradeType.ToString(), out int level))
            {
                int price = Util.GetUpgradeCost(_heroUpgradeType, level + 1);
                if (CanUpgrade(price))
                {
                    Managers.Backend.GameData.UserData.AddAmount(EGoodType.Gold, -price);
                    Managers.Backend.GameData.UserData.LevelUpHeroUpgrade(_heroUpgradeType);

                    if (_heroUpgradeType == EHeroUpgradeType.Growth_Atk || _heroUpgradeType == EHeroUpgradeType.Growth_Hp)
                        isUpgraded = true;
                }
            }
            _coolTime = StartCoroutine(CoStartUpgradeCoolTime(0.1f));
        }
        catch (Exception e)
        {
            throw new Exception($"OnPressUpgradeButton({EGoodType.Gold}) 중 에러가 발생하였습니다\n{e}");
        }


    }

    private void OnPointerUp()
    {
        if (_coolTime != null)
        {
            StopCoroutine(_coolTime);
            _coolTime = null;
        }

        // 한번이라도 업그레이드 했을 때
        if (isUpgraded && (_heroUpgradeType == EHeroUpgradeType.Growth_Atk || _heroUpgradeType == EHeroUpgradeType.Growth_Hp))
        {
            Managers.Event.TriggerEvent(EEventType.HeroTotalPowerUpdated);    
            Managers.UI.ShowBaseUI<UI_TotalPowerBase>().ShowTotalPowerUI();
            isUpgraded = false;
        }

    }

    private void CheckUpgradeInteractive()
    {
        if (Managers.Backend.GameData.UserData.UpgradeStatDic.TryGetValue(_heroUpgradeType.ToString(), out int level))
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
