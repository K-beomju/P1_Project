using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum Buttons
    {
        CharacterButton,
        EquipmentButton,
        SkillButton,
        DungeonButton,
        DrawButton
    }

    enum Sliders
    {
        Slider_Exp,
        Slider_StageInfo,
        Slider_BossTimer
    }

    enum Texts
    {
        RemainMonsterValueText,
        ExpValueText,
        Text_StageInfo
    }

    enum GameObjects
    {
        TopStage,
        SkillSlotGroup,
        RemainMonster
    }

    public enum PlayTab
    {
        None = -1,
        Character,
        Equipment,
        Skill,
        Dungeon,
        Draw,
        Shop
    }

    public enum UI_GoodItems
    {
        UI_GoodItem_Gold,
        UI_GoodItem_Dia,
    }

    public PlayTab _tab = PlayTab.None;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindObjects(typeof(GameObjects));
        Bind<UI_GoodItem>(typeof(UI_GoodItems));

        GetButton((int)Buttons.CharacterButton).gameObject.BindEvent(() => ShowTab(PlayTab.Character));
        GetButton((int)Buttons.EquipmentButton).gameObject.BindEvent(() => ShowTab(PlayTab.Equipment));
        GetButton((int)Buttons.SkillButton).gameObject.BindEvent(() => ShowTab(PlayTab.Skill));
        GetButton((int)Buttons.DungeonButton).gameObject.BindEvent(() => ShowTab(PlayTab.Dungeon));
        GetButton((int)Buttons.DrawButton).gameObject.BindEvent(() => ShowTab(PlayTab.Draw));

        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Gold).SetInfo(EGoodType.Gold);
        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Dia).SetInfo(EGoodType.Dia);

        InitializeUIElements();

        Managers.Event.AddEvent(EEventType.MonsterCountChanged, new Action<int, int>(RefreshShowRemainMonster));
        Managers.Event.AddEvent(EEventType.ExperienceUpdated, new Action<int, int, int>(RefreshShowExp));

        RefreshUI();
        return true;
    }

    private void InitializeUIElements()
    {
        GetSlider((int)Sliders.Slider_Exp).value = 0;
        GetSlider((int)Sliders.Slider_BossTimer).value = 0;
        GetSlider((int)Sliders.Slider_StageInfo).value = 0;

        GetTMPText((int)Texts.ExpValueText).text = string.Empty;
        GetTMPText((int)Texts.RemainMonsterValueText).text = string.Empty;
    }

    private void RefreshUI()
    {
        if (_init == false)
            return;

        ShowTab(_tab);
    }



    #region Stage UI

    public void UpdateStageUI(bool isBoss = false)
    {
        if(!isBoss)
                GetTMPText((int)Texts.Text_StageInfo).text = $"푸른 초원 {Managers.Scene.GetCurrentScene<GameScene>().GetCurrentStage()}";

        GetObject((int)GameObjects.RemainMonster).SetActive(!isBoss);
    }

    public void RefreshShowRemainMonster(int killMonster, int maxMonster)
    {
        GetTMPText((int)Texts.RemainMonsterValueText).text = $"{killMonster} / {maxMonster}";

        GetSlider((int)Sliders.Slider_StageInfo).value = killMonster;
        GetSlider((int)Sliders.Slider_StageInfo).maxValue = maxMonster;
    }

    public void RefreshBossMonsterHp(BossMonster boss)
    {
        float hpAmount = boss.Hp / boss.MaxHp;
        GetSlider((int)Sliders.Slider_StageInfo).maxValue = 1;
        GetSlider((int)Sliders.Slider_StageInfo).value = hpAmount;
        GetTMPText((int)Texts.Text_StageInfo).text = $"{boss.Hp} / {boss.MaxHp}";
    }

    public void RefreshBossStageTimer(float currentTime, float maxTime)
    {
        float timePercentage = currentTime / maxTime;
        GetSlider((int)Sliders.Slider_BossTimer).value = timePercentage;
    }

    #endregion

    #region Good, Exp

    public UI_GoodItem GetGoodItem(EGoodType goodType)
    {
        return goodType switch
        {
            EGoodType.Gold => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Gold),
            EGoodType.Dia => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Dia),
            _ => null,
        };
    }

    public void RefreshShowExp(int currentLevel, int currentExp, int expToNextLevel)
    {
        // 예외처리: expToNextLevel이 0이 아닌 경우에만 계산
        if (expToNextLevel > 0)
        {
            // 경험치 슬라이더와 텍스트 갱신
            GetSlider((int)Sliders.Slider_Exp).value = (float)currentExp / expToNextLevel;

            float expPercentage = ((float)currentExp / expToNextLevel) * 100;
            // 텍스트에 반영 (소수점 2자리로 표시)
            GetTMPText((int)Texts.ExpValueText).text = $"Lv.{currentLevel} ({expPercentage:F2})%";
        }
        else
        {
            // 경험치 슬라이더가 0인 경우 처리 (경험치가 없거나 초기화 상황)
            GetSlider((int)Sliders.Slider_Exp).value = 0;
            GetTMPText((int)Texts.ExpValueText).text = $"Lv.{currentLevel} (0%)";
        }
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

    public void ToggleTab(PlayTab tab, bool isActive)
    {
        if (isActive)
        {
            switch (tab)
            {
                case PlayTab.Character:
                    Managers.UI.ShowPopupUI<UI_CharacterPopup>().RefreshUI();
                    break;
                case PlayTab.Equipment:
                    Managers.UI.ShowPopupUI<UI_EquipmentPopup>().SetInfo();
                    break;
                case PlayTab.Skill:
                    Managers.UI.ShowPopupUI<UI_SkillPopup>();
                    break;
                case PlayTab.Dungeon:
                    Managers.UI.ShowPopupUI<UI_DungeonPopup>();
                    break;
                      case PlayTab.Draw:
                    Managers.UI.ShowPopupUI<UI_DrawPopup>().RefreshUI();
                    break;
                case PlayTab.Shop:
                    Managers.UI.ShowPopupUI<UI_ShopPopup>();
                    break;
                    
            }
            ShowPopupActiveGameUI(false);
        }
        else
        {
            Debug.Log("머자ㅣㅇ");
            Managers.UI.ClosePopupUI();
            ShowPopupActiveGameUI(true);
        }
    }

    #endregion

    public void ShowPopupActiveGameUI(bool active)
    {
        GetObject((int)GameObjects.TopStage).SetActive(active);
        GetObject((int)GameObjects.SkillSlotGroup).SetActive(active);

    }

    public void CloseDrawPopup(UI_Popup popup)
    {
        Managers.UI.ClosePopupUI(popup);
        _tab = PlayTab.None; // 팝업이 닫힐 때 _tab을 None으로 초기화
    }

}
