using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum Buttons
    {
        CharacterButton,
        SpiritButton,
        SkillButton,
        DungeonButton,
        ShopButton
    }

    enum Sliders
    {
        ExpSlider
    }

    enum TMPTexts
    {
        ExpValueText
    }

    enum Texts
    {
        RemainMonsterValueText
    }

    public enum PlayTab
    {
        None = -1,
        Character,
        Spirit,
        Skill,
        Dungeon,
        Shop
    }

    private PlayTab _tab = PlayTab.None;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(TMPTexts));
        BindTexts(typeof(Texts));

        BindSliders(typeof(Sliders));

        GetButton((int)Buttons.CharacterButton).gameObject.BindEvent(() => ShowTab(PlayTab.Character));
        GetButton((int)Buttons.SpiritButton).gameObject.BindEvent(() => ShowTab(PlayTab.Spirit));
        GetButton((int)Buttons.SkillButton).gameObject.BindEvent(() => ShowTab(PlayTab.Skill));
        GetButton((int)Buttons.DungeonButton).gameObject.BindEvent(() => ShowTab(PlayTab.Dungeon));
        GetButton((int)Buttons.ShopButton).gameObject.BindEvent(() => ShowTab(PlayTab.Shop));


        Managers.Event.AddEvent(EEventType.MonsterCountChanged, new Action<int, int>(RefreshShowRemainMonster));

        RefreshUI();
        return true;
    }

    private void RefreshUI()
    {
        if (_init == false)
            return;

        ShowTab(_tab);
    }



    #region Stage UI

    public void RefreshShowRemainMonster(int currentMonster, int maxMonster)
    {
        GetText((int)Texts.RemainMonsterValueText).text = $"{currentMonster} / {maxMonster}";
    }



    #endregion


    #region Tab

    public void ShowTab(PlayTab tab)
    {
        // 탭 변경 없이 현재 탭을 다시 선택한 경우
        if (_tab == tab)
        {
            if (_tab == PlayTab.None)
                return;
            ToggleTab(tab, false); // 현재 탭을 비활성화
            _tab = PlayTab.None;
        }
        else
        {
            if (_tab != PlayTab.None)
            {
                ToggleTab(_tab, false); // 이전 탭 비활성화
            }

            _tab = tab;
            ToggleTab(_tab, true); // 새 탭 활성화
        }
    }

    private void ToggleTab(PlayTab tab, bool isActive)
    {
        if (isActive)
        {
            switch (tab)
            {
                case PlayTab.Character:
                    Managers.UI.ShowPopupUI<UI_CharacterPopup>();
                    break;
                case PlayTab.Spirit:
                    Managers.UI.ShowPopupUI<UI_SpiritPopup>();
                    break;
                case PlayTab.Skill:
                    Managers.UI.ShowPopupUI<UI_SkillPopup>();
                    break;
                case PlayTab.Dungeon:
                    Managers.UI.ShowPopupUI<UI_DungeonPopup>();
                    break;
                case PlayTab.Shop:
                    Managers.UI.ShowPopupUI<UI_ShopPopup>();
                    break;
            }
        }
        else
            Managers.UI.ClosePopupUI();
    }

    #endregion

}
