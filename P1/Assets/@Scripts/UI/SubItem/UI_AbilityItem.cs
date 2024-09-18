using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AbilityItem : UI_Base
{
    public enum Texts
    {
        UpgradeLevelText,
        AbilityTitleText,
        AbilityValueText,
        UpgradeDescText,
        UpgradeCostText
    }

    public enum Buttons
    {
        UpgradeButton
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnPressUpgradeButton, Define.EUIEvent.Pressed);
        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnPointerUp, Define.EUIEvent.PointerUp);

        return true;
    }

    void OnPressUpgradeButton()
    {

    }

    void OnPointerUp()
    {

    }

}
