using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AttendanceItem : UI_Base
{
    public enum GameObjects
    {
        GameObject_ReceiveBG
    }

    public enum Texts
    {
        Text_Day,
        Text_Amount
    }

    public enum AttendanceStatus
    {
        NotReceived, // 안받음
        Received     // 받음
    }

    private int _index;
    private AttendanceStatus _status;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));
        GetObject((int)GameObjects.GameObject_ReceiveBG).SetActive(false);
        RefreshUI();
        return true;
    }

    public void SetInfo(int index)
    {
        _index = index;


        if (Init() == false)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        IsCheckReceiveReward();
        GetTMPText((int)Texts.Text_Day).text = $"DAY {_index}";
        GetTMPText((int)Texts.Text_Amount).text = (_index * 100).ToString();


        GetObject((int)GameObjects.GameObject_ReceiveBG).SetActive(_status == AttendanceStatus.Received);
    }

    public void IsCheckReceiveReward()
    {
        // 보상을 이미 받았다는 뜻  
        if(Managers.Backend.GameData.CharacterData.AttendanceIndex > _index)
        {
            _status = AttendanceStatus.Received;
        }
        // 보상 아직 안받음 
        else
        {
            _status = AttendanceStatus.NotReceived;
        }
    }
}
