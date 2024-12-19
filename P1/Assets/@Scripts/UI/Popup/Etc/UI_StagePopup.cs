using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_StagePopup : UI_Popup
{
    public enum Buttons
    {
        Btn_Exit,
        Btn_MoveStage,
        Btn_MinStage,
        Btn_MaxStage,
        Btn_MinusStage,
        Btn_PlusStage
    }

    public enum Sliders
    {
        Slider_MoveStage
    }

    public enum Texts
    {
        Text_ChageStage,
    }

    private Slider _moveStageSlider;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButtons(typeof(Buttons));
        BindSliders(typeof(Sliders));
        BindTMPTexts(typeof(Texts));

        _moveStageSlider = GetSlider((int)Sliders.Slider_MoveStage);

        _moveStageSlider.onValueChanged.AddListener(value =>
        {
            GetTMPText((int)Texts.Text_ChageStage).text = Mathf.RoundToInt(value).ToString();
        });

        GetButton((int)Buttons.Btn_MinStage).onClick.AddListener(OnClickMinStageButton);
        GetButton((int)Buttons.Btn_MaxStage).onClick.AddListener(OnClickMaxStageButton);
        GetButton((int)Buttons.Btn_PlusStage).onClick.AddListener(OnClickPlusStageButton);
        GetButton((int)Buttons.Btn_MinusStage).onClick.AddListener(OnClickMinusStageButton);


        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        GetButton((int)Buttons.Btn_MoveStage).onClick.AddListener(OnClickMoveStageButton);
        return true;
    }

    public void RefreshUI()
    {
        int currentStage = Managers.Backend.GameData.CharacterData.StageLevel;

        _moveStageSlider.minValue = 1;
        _moveStageSlider.maxValue = currentStage;
        _moveStageSlider.value = currentStage;
    }

    private void OnClickMinStageButton()
    {
        _moveStageSlider.value = GetSlider((int)Sliders.Slider_MoveStage).minValue;
    }

    private void OnClickMaxStageButton()
    {
        _moveStageSlider.value = GetSlider((int)Sliders.Slider_MoveStage).maxValue;
    }

    private void OnClickMinusStageButton()
    {
        _moveStageSlider.value -= 1;
    }

    private void OnClickPlusStageButton()
    {
        _moveStageSlider.value += 1;
    }


    private void OnClickMoveStageButton()
    {
        if (_moveStageSlider.value == _moveStageSlider.maxValue)
        {
            ShowAlertUI("현재 스테이지입니다");
            return;
        }

        var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
        Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_FADEPOPUP);
        fadeUI.ShowFadeInOut(EFadeType.FadeInOut, 1f, 1f, 1f,
           fadeOutCallBack: () =>
           {
               // UI 내려주고 스테이지 로드 
               ClosePopupUI();
               Managers.Scene.GetCurrentScene<GameScene>().StayStage( Mathf.RoundToInt(_moveStageSlider.value));
           });
    }

}
