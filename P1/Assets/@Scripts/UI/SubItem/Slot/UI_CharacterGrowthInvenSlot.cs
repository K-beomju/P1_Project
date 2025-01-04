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
    private Coroutine _upgradeCoroutine;

    private float _minUpgradeDelay = 0.05f; // 최대 업그레이드 속도값
    private float _initialUpgradeDelay = 0.2f; // 초기 업그레이드 속도값
    private float _speedIncreaseFactor = 0.2f; // 속도 증가 비율 (1보다 작아야 속도가 빨라짐) -> 점점 줄어들게
    private bool isTotalPowerUpdated = false;

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
        UpdateSlotInfoUI();
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.CurrencyUpdated, new Action(CheckUpgradeInteractive));
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
        if (!Managers.Backend.GameData.CharacterData.UpgradeStatDic.TryGetValue(_heroUpgradeType.ToString(), out int level))
        {
            Debug.LogWarning($"UpdateSlotInfoUI도중 {level}이 없습니다");
            return;
        }

        CheckUpgradeInteractive();
        string percentString = "%";
        string titleText = $"{Util.GetHeroUpgradeString(_heroUpgradeType)}";
        string levelText = $"Lv {level}";
        string valueText =
            $"{Util.ConvertToTotalCurrency((long)(Managers.Data.HeroUpgradeChart[_heroUpgradeType].Value + (Managers.Data.HeroUpgradeChart[_heroUpgradeType].IncreaseValue * (level - 1))))}";
        string amountText = $"{Util.GetUpgradeCost(_heroUpgradeType, level + 1):N0}";

        GetTMPText((int)Texts.Text_Title).text = titleText;
        GetTMPText((int)Texts.Text_Level).text = levelText;
        if (_heroUpgradeType == EHeroUpgradeType.Growth_CriRate || _heroUpgradeType == EHeroUpgradeType.Growth_CriDmg)
            valueText += percentString;

        GetTMPText((int)Texts.Text_Value).text = valueText;
        GetTMPText((int)Texts.Text_Amount).text = amountText;
    }

    #region Button Event
    
    private void OnPressUpgradeButton()
    {
        if (_upgradeCoroutine != null)
            return;

        _upgradeCoroutine = StartCoroutine(CoHoldUpgrade());
    }

    private void OnPointerUp()
    {
        if (_upgradeCoroutine != null)
        {
            StopCoroutine(_upgradeCoroutine);
            _upgradeCoroutine = null;
        }

        if (isTotalPowerUpdated && (_heroUpgradeType == EHeroUpgradeType.Growth_Atk || _heroUpgradeType == EHeroUpgradeType.Growth_Hp))
        {
            Managers.Event.TriggerEvent(EEventType.HeroTotalPowerUpdated);
            Managers.UI.ShowBaseUI<UI_TotalPowerBase>().ShowTotalPowerUI();
            isTotalPowerUpdated = false;
        }
    }

    #endregion

    private IEnumerator CoHoldUpgrade()
    {
        TryUpgrade(); // 업그레이드 시도

        float currentDelay = _initialUpgradeDelay;
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            TryUpgrade(); // 업그레이드 시도
            yield return new WaitForSeconds(currentDelay);
            currentDelay = Mathf.Max(currentDelay * _speedIncreaseFactor, _minUpgradeDelay);
        }
    }

    private void TryUpgrade()
    {
        if (Managers.Backend.GameData.CharacterData.UpgradeStatDic.TryGetValue(_heroUpgradeType.ToString(), out int level))
        {
            int price = Util.GetUpgradeCost(_heroUpgradeType, level + 1);
            if (CanUpgrade(price))
            {
                Debug.Log(price);
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Gold, -price);
                Managers.Backend.GameData.CharacterData.LevelUpHeroUpgrade(_heroUpgradeType);

                if (_heroUpgradeType == EHeroUpgradeType.Growth_Atk || _heroUpgradeType == EHeroUpgradeType.Growth_Hp)
                {
                    var questType = _heroUpgradeType == EHeroUpgradeType.Growth_Atk
                        ? EQuestType.UpgradeAtk
                        : EQuestType.UpgradeMaxHp;
                    Managers.Backend.GameData.QuestData.UpdateQuest(questType);
                    isTotalPowerUpdated = true;
                }

                var missionType =  _heroUpgradeType switch
                {
                    EHeroUpgradeType.Growth_Atk => EMissionType.AttackPowerUpgrade,
                    EHeroUpgradeType.Growth_Hp => EMissionType.MaxHealthUpgrade,
                    EHeroUpgradeType.Growth_Recovery => EMissionType.HealthRegenUpgrade,
                    EHeroUpgradeType.Growth_CriRate => EMissionType.CriticalRateUpgrade,
                    EHeroUpgradeType.Growth_CriDmg =>  EMissionType.CriticalDamageUpgrade,
                    _ => throw new ArgumentException($"Unknown rare type String: {_heroUpgradeType}")
                };
                Managers.Backend.GameData.MissionData.UpdateMission(missionType);

                UpdateSlotInfoUI();
            }
            else
            {
                ShowAlertUI("골드가 부족합니다");
            }
        }
        Managers.Sound.Play(ESound.Effect, "Sounds/Button", 0.5f);
    }


    private void CheckUpgradeInteractive()
    {
        if (Managers.Backend.GameData.CharacterData.UpgradeStatDic.TryGetValue(_heroUpgradeType.ToString(), out int level))
        {
            int price = Util.GetUpgradeCost(_heroUpgradeType, level + 1);
            GetButton((int)Buttons.Btn_Upgrade).interactable = CanUpgrade(price);
        }
    }

    bool CanUpgrade(float cost)
    {
        if (!Managers.Backend.GameData.CharacterData.PurseDic.TryGetValue(EItemType.Gold.ToString(), out double amount))
            return false;

        return amount >= cost;
    }

}
