using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_AttributePanel : UI_Base
{
    public enum Images
    {
        Image_SelectAbIcon
    }

    public enum Texts
    {
        Text_AttributeName,
        Text_AttributeLevel,
        Text_AttributeValue,
        Text_Amount
    }

    public enum Buttons
    {
        Btn_Upgrade
    }

    public enum UI_AttributeGrowthSlots
    {
        UI_AttributeGrowthInvenSlot_Atk,
        UI_AttributeGrowthInvenSlot_MaxHp,
        UI_AttributeGrowthInvenSlot_CriRate,
        UI_AttributeGrowthInvenSlot_CriDmg,
        UI_AttributeGrowthInvenSlot_SkillTime,
        UI_AttributeGrowthInvenSlot_SkillDmg
    }

    private EHeroAttrType selectAttrType;
    public EHeroAttrType SelectAttrType
    {
        get { return selectAttrType; }
        set
        {
            if (selectAttrType != value)
            {
                selectAttrType = value;
            }
        }
    }
    private Coroutine _upgradeCoroutine;

    private float _minUpgradeDelay = 0.05f; // 최대 업그레이드 속도값
    private float _initialUpgradeDelay = 0.2f; // 초기 업그레이드 속도값
    private float _speedIncreaseFactor = 0.2f; // 속도 증가 비율 (1보다 작아야 속도가 빨라짐) -> 점점 줄어들게
    private bool isUpgraded = false;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_AttributeGrowhInvenSlot>(typeof(UI_AttributeGrowthSlots));
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_Upgrade).gameObject.BindEvent(OnPressUpgradeButton, EUIEvent.Pressed);
        GetButton((int)Buttons.Btn_Upgrade).gameObject.BindEvent(OnPointerUp, EUIEvent.PointerUp);

        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_Atk).SetInfo(EHeroAttrType.Attribute_Atk);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_MaxHp).SetInfo(EHeroAttrType.Attribute_MaxHp);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriRate).SetInfo(EHeroAttrType.Attribute_CriRate);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriDmg).SetInfo(EHeroAttrType.Attribute_CriDmg);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillTime).SetInfo(EHeroAttrType.Attribute_SkillTime);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillDmg).SetInfo(EHeroAttrType.Attribute_SkillDmg);

        ShowAttributeDetailUI(EHeroAttrType.Attribute_Atk);
        return true;
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.AttributeItemClick, new Action<EHeroAttrType>(ShowAttributeDetailUI));

    }

    void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.AttributeItemClick, new Action<EHeroAttrType>(ShowAttributeDetailUI));
    }


    public void ShowAttributeDetailUI(EHeroAttrType attrType)
    {
        SelectAttrType = attrType;

        if (!Managers.Backend.GameData.CharacterData.UpgradeAttrDic.TryGetValue(SelectAttrType.ToString(), out int level))
        {
            Debug.LogWarning($"UpdateSlotInfoUI도중 {level}이 없습니다");
            return;
        }
        CheckUpgradeInteractive();
        Data.HeroAttributeInfoData attriData = Managers.Data.HeroAttributeChart[SelectAttrType];

        GetTMPText((int)Texts.Text_AttributeLevel).text = $"Lv {level}";
        GetTMPText((int)Texts.Text_AttributeName).text = attriData.Name;
        GetTMPText((int)Texts.Text_AttributeValue).text =
        $"{attriData.IncreaseValue * level}% => {attriData.IncreaseValue * (level + 1)}%";
        GetTMPText((int)Texts.Text_AttributeName).text = attriData.Name;
        GetTMPText((int)Texts.Text_Amount).text = $"{Util.GetAttributeCost(SelectAttrType, level + 1):N0}"; ;

        GetImage((int)Images.Image_SelectAbIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{attriData.SpriteKey}");
    }

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

        // 한번이라도 업그레이드 했을 때
        if (isUpgraded && (SelectAttrType == EHeroAttrType.Attribute_Atk || SelectAttrType == EHeroAttrType.Attribute_MaxHp))
        {
            Managers.Event.TriggerEvent(EEventType.HeroTotalPowerUpdated);
            Managers.UI.ShowBaseUI<UI_TotalPowerBase>().ShowTotalPowerUI();
            isUpgraded = false;
        }
    }

    private IEnumerator CoHoldUpgrade()
    {
        float currentDelay = _initialUpgradeDelay;

        while (true)
        {
            TryUpgrade(); // 업그레이드 시도

            yield return new WaitForSeconds(currentDelay);

            // 업그레이드 속도를 점차적으로 증가
            currentDelay = Mathf.Max(currentDelay * _speedIncreaseFactor, _minUpgradeDelay);
        }
    }

    private void TryUpgrade()
    {
        if (Managers.Backend.GameData.CharacterData.UpgradeAttrDic.TryGetValue(SelectAttrType.ToString(), out int level))
        {
            int price = Util.GetAttributeCost(SelectAttrType, level + 1);
            if (CanUpgrade(price))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.ExpPoint, -price);
                Managers.Backend.GameData.CharacterData.LevelUpHeroAttribute(SelectAttrType);
                ShowAttributeDetailUI(SelectAttrType);

                if (SelectAttrType == EHeroAttrType.Attribute_Atk || SelectAttrType == EHeroAttrType.Attribute_MaxHp)
                    isUpgraded = true;
            }
            else
                ShowAlertUI("경험치포인트가 부족합니다");
        }
    }


    private void CheckUpgradeInteractive()
    {
        if (Managers.Backend.GameData.CharacterData.UpgradeAttrDic.TryGetValue(SelectAttrType.ToString(), out int level))
        {
            int price = Util.GetAttributeCost(SelectAttrType, level + 1);
            GetButton((int)Buttons.Btn_Upgrade).interactable = CanUpgrade(price);
        }
    }

    bool CanUpgrade(float cost)
    {
        if (!Managers.Backend.GameData.CharacterData.PurseDic.TryGetValue(EItemType.ExpPoint.ToString(), out float amount))
            return false;

        return amount >= cost;
    }
}
