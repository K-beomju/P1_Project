using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class UI_SleepModePopup : UI_Popup
{
    public enum Sliders
    {
        Slider_CurrentBattery
    }

    public enum Texts
    {
        Text_CurrentBattery,
        Text_CurrentTime,
        Text_StageLevel,
        Text_StageChapter,
        Text_IsFighting
    }

    public enum Buttons
    {
        Btn_ClearSleepMode
    }

    public enum Images
    {
        Image_ClearPressAmount
    }

    private int _fightindex = 0;
    private int _fightMaxindex = 3;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindSliders(typeof(Sliders));
        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));

        GetButton((int)Buttons.Btn_ClearSleepMode).onClick.AddListener(Clear);

        return true;
    }

    public void RefreshUI()
    {
        AudioListener.pause = true; // 오디오 비활성화
        OnDemandRendering.renderFrameInterval = 3; // 렌더링 프레임 간격을 3으로 설정 (20FPS)
        StartCoroutine(ShowFightTextCo());
        StartCoroutine(ShowFightTextCo());
    }

    private IEnumerator ShowFightTextCo()
    {
        var delay = new WaitForSeconds(1);

        while (true)
        {
            UpdateFightText();
            // Battery 잔량 로직 // 컴퓨터에선 -100으로뜸
            float batteryLevel = SystemInfo.batteryLevel;
            GetTMPText((int)Texts.Text_CurrentBattery).text = $"{batteryLevel * 100}%";
            GetSlider((int)Sliders.Slider_CurrentBattery).value = Mathf.Clamp01(batteryLevel); // 0~1 사이 값만 허용

            // 시간 로직
            GetTMPText((int)Texts.Text_CurrentTime).text = DateTime.Now.ToString("HH : mm");

            // Stage 로직
            GetTMPText((int)Texts.Text_StageLevel).text = $"STAGE {Managers.Backend.GameData.CharacterData.StageLevel}";
            yield return delay; // 1초 간격으로 실행

        }
    }

    private void UpdateFightText()
    {
        _fightindex = (_fightindex + 1) % _fightMaxindex;

        switch (_fightindex)
        {
            case 0:
                GetTMPText((int)Texts.Text_IsFighting).text = "전투 중 . . .";
                break;
            case 1:
                GetTMPText((int)Texts.Text_IsFighting).text = "전투 중 . .";
                break;
            case 2:
                GetTMPText((int)Texts.Text_IsFighting).text = "전투 중 .";
                break;
        }
    }

    public void Clear()
    {

        OnDemandRendering.renderFrameInterval = 1; // 렌더링 프레임 간격 복원
        AudioListener.pause = false; // 오디오 활성화
        Managers.UI.CloseAllPopupUI();
    }

}
