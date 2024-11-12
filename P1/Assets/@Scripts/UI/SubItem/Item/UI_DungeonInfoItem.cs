using Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DungeonInfoItem : UI_Base
{
    public enum Buttons
    {
        Button_Sweep,
        Button_Entrance
    }

    public enum Texts
    {
        Text_KeyAmount,
        Text_SweepAmount,
        Text_EntranceAmount
    }

    private EDungeonType _dungeonType;
    private const float SWEEP_REWARD_MULTIPLIER = 0.9f;

    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Button_Sweep).onClick.AddListener(OnButtonSweep);
        GetButton((int)Buttons.Button_Entrance).onClick.AddListener(OnButtonEntrance);
        return true;
    }

    private void OnButtonEntrance()
    {
        var dungeonData = Managers.Backend.GameData.DungeonData;
        if (dungeonData.DungeonKeyDic[_dungeonType.ToString()] < 1)
        {
            Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification($"{Util.GetDungeonType(_dungeonType)}키가 부족합니다.");
            return;
        }

        dungeonData.AddKey(_dungeonType, -1);
        var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
        fadeUI.sceneMove = true;
        Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_FADEPOPUP);
        fadeUI.ShowFadeInOut(EFadeType.FadeOut, 1f, 0, () =>
        {
            Managers.Game.SetCurrentDungeon(_dungeonType);
            Managers.Scene.LoadScene(EScene.DungeonScene);
        });
    }

    private void OnButtonSweep()
    {
        var dungeonData = Managers.Backend.GameData.DungeonData;
        if (IsSweepLevel())
        {
            if (dungeonData.DungeonKeyDic[_dungeonType.ToString()] < 1)
            {
                Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification($"{Util.GetDungeonType(_dungeonType)}키가 부족합니다.");
                return;
            }

            DungeonInfoData dungeonInfo = _dungeonType switch
            {
                EDungeonType.Gold => Managers.Data.GoldDungeonChart[dungeonData.DungeonLevelDic[_dungeonType.ToString()] - 1],
                EDungeonType.Dia => Managers.Data.DiaDungeonChart[dungeonData.DungeonLevelDic[_dungeonType.ToString()] - 1],
                _ => throw new ArgumentException($"Unknown DungeonType: {_dungeonType}")
            };

            dungeonData.AddKey(_dungeonType, -1);

            Managers.UI.ShowPopupUI<UI_ItemGainPopup>().
            RefreshUI(dungeonInfo.ItemType, new Dictionary<Enum, int> { { _dungeonType, dungeonInfo.DungeonClearReward } });

            Managers.Backend.GameData.CharacterData.AddAmount(dungeonInfo.ItemType, dungeonInfo.DungeonClearReward);
            RefreshUI();
        }
        else
        {
            Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification("던전을 클리어 해야합니다.");
        }
    }

    public void SetInfo(EDungeonType dungeonType)
    {
        _dungeonType = dungeonType;
    }

    public void RefreshUI()
    {
        var dungeonData = Managers.Backend.GameData.DungeonData;
        GetTMPText((int)Texts.Text_KeyAmount).text
            = $"{dungeonData.DungeonKeyDic[_dungeonType.ToString()]} / {Util.DungenEntranceMaxValue(_dungeonType)}";

        if(_dungeonType == EDungeonType.WorldBoss)
        {
            GetTMPText((int)Texts.Text_SweepAmount).gameObject.SetActive(false);
            GetTMPText((int)Texts.Text_EntranceAmount).gameObject.SetActive(false);
            return;
        }

        float reward = _dungeonType switch
        {
            EDungeonType.Gold => Managers.Data.GoldDungeonChart[dungeonData.DungeonLevelDic[_dungeonType.ToString()]].DungeonClearReward,
            EDungeonType.Dia => Managers.Data.DiaDungeonChart[dungeonData.DungeonLevelDic[_dungeonType.ToString()]].DungeonClearReward,
            _ => 0
        };

        GetTMPText((int)Texts.Text_SweepAmount).text = Util.ConvertToTotalCurrency((long)Mathf.Round(reward * SWEEP_REWARD_MULTIPLIER));
        GetTMPText((int)Texts.Text_EntranceAmount).text = Util.ConvertToTotalCurrency((long)reward);
    }

    private bool IsSweepLevel()
    {
        return Managers.Backend.GameData.DungeonData.DungeonLevelDic[_dungeonType.ToString()] > 1;
    }
}
