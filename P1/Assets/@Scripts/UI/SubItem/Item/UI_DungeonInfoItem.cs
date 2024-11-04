using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DungeonInfoItem : UI_Base
{
    public enum Buttons
    {
        Button_Entrance
    }

    public enum Texts
    {
        Text_Amount
    }

    private EDungeonType _dungeonType;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Button_Entrance).onClick.AddListener(OnButtonClick);
        return true;
    }

    private void OnButtonClick()
    {
        // 던전 키 하나라도 가지고 있을 경우
        var dungeonData = Managers.Backend.GameData.DungeonData;
        if (dungeonData.DungeonKeyDic[_dungeonType.ToString()] < 1)
        {
            Debug.Log($"{_dungeonType}키가 없습니다.");
            return;
        }

        dungeonData.AddKey(_dungeonType, -1);

        var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
        fadeUI.sceneMove = true;
        Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_TOTALPOWER + 1);
        fadeUI.ShowFadeInOut(EFadeType.FadeOut, 1f, 0,
        fadeOutCallBack: () =>
        {
            Managers.Game.SetCurrentDungeon(_dungeonType);
            Managers.Scene.LoadScene(EScene.DungeonScene);
        });
    }


    public void SetInfo(EDungeonType dungeonType)
    {
        _dungeonType = dungeonType;
    }

    public void RefreshUI()
    {
        GetTMPText((int)Texts.Text_Amount).text
        = $"{Managers.Backend.GameData.DungeonData.DungeonKeyDic[_dungeonType.ToString()]} / {Util.DungenEntranceMaxValue(_dungeonType)}";
    }
}
