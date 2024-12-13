using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_SettingPopup : UI_Popup
{
    public enum GameObjects
    {
        BG
    }

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

    public enum Images
    {
        Image_SaveCoolTime
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindSliders(typeof(Sliders));
        BindObjects(typeof(GameObjects));
        BindImages(typeof(Images));

        GetObject((int)GameObjects.BG).BindEvent(ClosePopupUI);
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        GetButton((int)Buttons.Btn_Attendance).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_AttendancePopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SETTING_CONTENT_POPUP);
            popupUI.RefreshUI();
        });


        // BottomBar
        GetButton((int)Buttons.Btn_SleepMode).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_SleepModePopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SLEEPMODEPOPUP);
            popupUI.RefreshUI();
        });
        GetButton((int)Buttons.Btn_SaveData).onClick.AddListener(OnClickButtonSaveData);
        GetButton((int)Buttons.Btn_FeedBack).onClick.AddListener(() =>
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.LukeDogCompany.com.IdleGame");
        });
        GetButton((int)Buttons.Btn_NaverCafe).onClick.AddListener(() =>
        {
            Debug.Log("네이버 카페로 이동");
        });
        GetImage((int)Images.Image_SaveCoolTime).fillAmount = 0;
        return true;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (!Managers.Backend.IsSaveCoolTimeActive)
        {
            GetImage((int)Images.Image_SaveCoolTime).fillAmount = 0;
            GetButton((int)Buttons.Btn_SaveData).interactable = true;
        }

    }

    private void OnClickButtonSaveData()
    {
        if (Managers.Backend.IsSaveCoolTimeActive)
        {
            ShowAlertUI($"쿨타임이 남아 있습니다. 남은 시간: {Managers.Backend.saveDataCoolTime:F1}초");
            return;
        }
        GetButton((int)Buttons.Btn_SaveData).interactable = false;

        // 쿨타임 시작
        Managers.Backend.StartSaveCoolTime(() =>
        {
            GetButton((int)Buttons.Btn_SaveData).interactable = true;
        });

        // 데이터 저장
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
    }


    void FixedUpdate()
    {
        if (Managers.Backend.IsSaveCoolTimeActive)
        {
            GetImage((int)Images.Image_SaveCoolTime).fillAmount = Managers.Backend.saveDataCoolTime / 5;
        }
    }


}
