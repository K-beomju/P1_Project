using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<UI_AttendanceItem> attendanceItems = new List<UI_AttendanceItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

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

        // 버튼 상태 갱신
        RefreshUI();
    }



    public void RefreshUI()
    {
        bool receive = Managers.Backend.GameData.CharacterData.AttendanceCheck();
        GetButton((int)Buttons.Button_Receive).interactable = receive;
    }
}
