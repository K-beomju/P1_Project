using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_AdBuffItem : UI_Base
{
    public enum Buttons
    {
        Btn_TakeBuff
    }

    public enum Texts
    {
        Text_Amount
    }

    private EAdBuffType BuffType;
    private int MaxCount = 2;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_TakeBuff).onClick.AddListener(OnClickButton);
        RefreshUI();
        return true;
    }

    private void OnClickButton()
    {
        if (Managers.Backend.GameData.CharacterData.AdBuffDic[BuffType.ToString()] < MaxCount)
        {
            Managers.Backend.GameData.CharacterData.UsedBuff(BuffType);
            RefreshUI();
        }
        else
        {
            Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification($"일일 사용 가능 횟수를 초과하였습니다.");
        }

    }

    public void SetInfo(EAdBuffType buffType)
    {
        BuffType = buffType;

        if (Init() == false)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        GetTMPText((int)Texts.Text_Amount).text
        = $"{Managers.Backend.GameData.CharacterData._adBuffDic[BuffType.ToString()]} / {MaxCount}";
    }
}
