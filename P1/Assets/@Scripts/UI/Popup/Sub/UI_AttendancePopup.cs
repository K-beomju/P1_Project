using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_AttendancePopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        AttendanceContent
    }

    public enum Buttons
    {
        Btn_Exit,
        Button_Receive
    }

    public enum Texts
    {
        Text_Timer
    }

    private List<UI_AttendanceItem> attendanceItems = new List<UI_AttendanceItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        GetButton((int)Buttons.Button_Receive).onClick.AddListener(OnClickButton);
        GetObject((int)GameObjects.BG).BindEvent(ClosePopupUI);

        var contentTransform = GetObject((int)GameObjects.AttendanceContent).transform;

        for (int i = 0; i < contentTransform.childCount; i++)
        {
            GameObject go = contentTransform.GetChild(i).gameObject;
            if (attendanceItems.Count <= i)
            {
                UI_AttendanceItem item = go.GetOrAddComponent<UI_AttendanceItem>();
                item.SetInfo(i + 1);
                item.name = $"UI_AttendanceItem_{i + 1}";
                attendanceItems.Add(item);
            }
        }
        return true;
    }

    private void OnClickButton()
    {
        Managers.Backend.GameData.CharacterData.AttendanceReceive();

        // 1일차로 초기화된 경우 모든 아이템 갱신
        if (Managers.Backend.GameData.CharacterData.AttendanceIndex == 2)
        {
            attendanceItems.ForEach(item => item.RefreshUI());
        }
        else
        {
            // 변경된 항목만 갱신
            int updatedIndex = Managers.Backend.GameData.CharacterData.AttendanceIndex - 1;
            if (updatedIndex > 0 && updatedIndex <= attendanceItems.Count)
            {
                attendanceItems[updatedIndex - 1].RefreshUI();
            }
        }

        for (int i = 0; i < 10; i++)
        {
            UI_ItemIconBase itemIcon = Managers.UI.ShowPooledUI<UI_ItemIconBase>();
            itemIcon.SetItemIconAtCanvasPosition(EItemType.Dia, Vector2.zero);
        }
        // 버튼 상태 갱신
        RefreshUI();
    }



    public void RefreshUI()
    {
        var characterData = Managers.Backend.GameData.CharacterData;
        bool receive = characterData.AttendanceCheck();

        GetButton((int)Buttons.Button_Receive).interactable = receive;
        GetTMPText((int)Texts.Text_Timer).text = GetAttendanceResetTime(characterData.AttendanceLastLoginTime);
    }

    // 출석체크 초기화 남은 시간 체크 
    private string GetAttendanceResetTime(string lastLoginTime)
    {
        DateTime lastLogin = DateTime.Parse(lastLoginTime);
        DateTime nextReset = lastLogin.AddDays(1);
        TimeSpan timeUntilReset = nextReset - DateTime.UtcNow;

        if (timeUntilReset.TotalSeconds <= 0)
        {
            return "출석 체크 가능";
        }

        return $"초기화까지 남은 시간\n: {timeUntilReset.Hours}시간 {timeUntilReset.Minutes}분";
    }
}
