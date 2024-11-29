using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_SettingPopup : UI_Popup
{
    public enum Buttons
    {
        Btn_Attendance,
        Btn_SleepMode,
        Btn_SaveData,
        Btn_FeedBack,
        Btn_NaverCafe,
        Btn_Exit
    }

    public enum Sliders
    {
        Slider_Bgm,
        Slider_Effect
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindSliders(typeof(Sliders));
        
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        GetButton((int)Buttons.Btn_Attendance).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_AttendancePopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
            popupUI.RefreshUI();
        });


        // BottomBar
        GetButton((int)Buttons.Btn_SleepMode).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_SleepModePopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SLEEPMODEPOPUP);
            popupUI.RefreshUI();
        });
        GetButton((int)Buttons.Btn_SaveData).onClick.AddListener(() =>
        {
            Managers.Backend.UpdateAllGameData(callback =>
            {
                if (callback == null)
                {
                    ShowAlertUI("저장할 데이터가 존재하지 않습니다.");
                    return;
                }

                if (callback.IsSuccess())
                {
                    ShowAlertUI("저장 성공, 저장에 성공했습니다.");
                }
                else
                {
                    ShowAlertUI($"수동 저장 실패, 수동 저장에 실패했습니다. {callback.ToString()}");
                }

            });
        });
        GetButton((int)Buttons.Btn_FeedBack).onClick.AddListener(() => 
        {
            Debug.Log("구글 스토어 링크로 이동");
        });
        GetButton((int)Buttons.Btn_NaverCafe).onClick.AddListener(() => 
        {
            Debug.Log("네이버 카페로 이동");
        });
        return true;
    }
    
}
