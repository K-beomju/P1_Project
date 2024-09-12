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
        ExpSlider,
        CurrentStageSlider,
        BossHpSlider,
        BossStageTimer
    }

    enum TMPTexts
    {
        ExpValueText
    }

    enum Texts
    {
        RemainMonsterValueText
    }

    enum GameObjects
    {
        TopStage,
        RemainMonster
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

    public enum UI_GoodItems
    {
        UI_GoodItem_Gold,
        //UI_GoodItem_Dia,
        //UI_GoodItem_Money
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
        BindObjects(typeof(GameObjects));
        Bind<UI_GoodItem>(typeof(UI_GoodItems));

        GetButton((int)Buttons.CharacterButton).gameObject.BindEvent(() => ShowTab(PlayTab.Character));
        GetButton((int)Buttons.SpiritButton).gameObject.BindEvent(() => ShowTab(PlayTab.Spirit));
        GetButton((int)Buttons.SkillButton).gameObject.BindEvent(() => ShowTab(PlayTab.Skill));
        GetButton((int)Buttons.DungeonButton).gameObject.BindEvent(() => ShowTab(PlayTab.Dungeon));
        GetButton((int)Buttons.ShopButton).gameObject.BindEvent(() => ShowTab(PlayTab.Shop));

        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Gold).SetInfo(EGoodType.Gold);
        //Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Dia).SetInfo(EGoodType.Dia);
        //Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Money).SetInfo(EGoodType.Money);

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

    public void ShowNormalOrBossStageUI(bool isBoss = false)
    {
        SetActiveSlider(Sliders.BossHpSlider, isBoss, 1f);
        SetActiveSlider(Sliders.BossStageTimer, isBoss, 1f);
        GetObject((int)GameObjects.RemainMonster).SetActive(!isBoss);
    }

    private void SetActiveSlider(Sliders slider, bool active, float value)
    {
        var sliderObj = GetSlider((int)slider);
        sliderObj.gameObject.SetActive(active);
        sliderObj.value = value;
    }

    public void RefreshShowRemainMonster(int currentMonster, int maxMonster)
    {
        GetText((int)Texts.RemainMonsterValueText).text = $"{currentMonster} / {maxMonster}";
    }

    public void RefreshShowCurrentStage(int currentStage, int maxStage)
    {
        GetSlider((int)Sliders.CurrentStageSlider).value = currentStage;
        GetSlider((int)Sliders.CurrentStageSlider).maxValue = maxStage;
    }

    public void RefreshBossMonsterHp(BossMonster boss)
    {
        float hpAmount = boss.Hp / boss.MaxHp.Value;
        GetSlider((int)Sliders.BossHpSlider).value = hpAmount;
    }

    public void RefreshBossStageTimer(float currentTime, float maxTime)
    {
        float timePercentage = currentTime / maxTime;
        GetSlider((int)Sliders.BossStageTimer).value = timePercentage;
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
            GetObject((int)GameObjects.TopStage).SetActive(false);
        }
        else
        {
            Managers.UI.ClosePopupUI();
            GetObject((int)GameObjects.TopStage).SetActive(true);

        }
    }

    #endregion

}
