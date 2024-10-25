using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
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
        DrawButton,
        Btn_AutoSkill
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
        Text_StageInfo,
        Text_AutoSkill
    }

    enum GameObjects
    {
        TopStage,
        RemainMonster
    }

    enum CanvasGroups
    {
        SkillSlotGroup
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
        UI_GoodItem_ExpPoint
    }

    public enum EquipSkillSlots
    {
        UI_EquipSkillSlot_1,
        UI_EquipSkillSlot_2,
        UI_EquipSkillSlot_3,
        UI_EquipSkillSlot_4,
        UI_EquipSkillSlot_5,
        UI_EquipSkillSlot_6
    }

    public PlayTab _tab { get; set; } = PlayTab.None;

    public List<UI_EquipSkillSlot> _equipSkillSlotList { get; set; } = new List<UI_EquipSkillSlot>();
    private bool _isAutoSkillActive = false; // AutoSkill 활성화 상태를 저장하는 변수

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindObjects(typeof(GameObjects));
        Bind<CanvasGroup>(typeof(CanvasGroups));
        Bind<UI_GoodItem>(typeof(UI_GoodItems));
        Bind<UI_EquipSkillSlot>(typeof(EquipSkillSlots));


        GetButton((int)Buttons.CharacterButton).gameObject.BindEvent(() => ShowTab(PlayTab.Character));
        GetButton((int)Buttons.EquipmentButton).gameObject.BindEvent(() => ShowTab(PlayTab.Equipment));
        GetButton((int)Buttons.SkillButton).gameObject.BindEvent(() => ShowTab(PlayTab.Skill));
        GetButton((int)Buttons.DungeonButton).gameObject.BindEvent(() => ShowTab(PlayTab.Dungeon));
        GetButton((int)Buttons.DrawButton).gameObject.BindEvent(() => ShowTab(PlayTab.Draw));
        GetButton((int)Buttons.Btn_AutoSkill).gameObject.BindEvent(ActiveAutoSkill);

        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Gold).SetInfo(EGoodType.Gold);
        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Dia).SetInfo(EGoodType.Dia);
        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_ExpPoint).SetInfo(EGoodType.ExpPoint);


        InitializeUIElements();

        Managers.Event.AddEvent(EEventType.MonsterCountChanged, new Action<int, int>(RefreshShowRemainMonster));
        Managers.Event.AddEvent(EEventType.ExperienceUpdated, new Action<int, float, float>(RefreshShowExp));

        RefreshUI();
        return true;
    }

    private void InitializeUIElements()
    {
        // 6개의 슬롯을 초기화하면서 각 슬롯을 Lock 타입으로 설정
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            _equipSkillSlotList.Add(Get<UI_EquipSkillSlot>(i));
            _equipSkillSlotList[i].SetInfo(index);
        }

        GetSlider((int)Sliders.Slider_Exp).value = 0;
        GetSlider((int)Sliders.Slider_BossTimer).value = 0;
        GetSlider((int)Sliders.Slider_StageInfo).value = 0;

        GetTMPText((int)Texts.ExpValueText).text = string.Empty;
        GetTMPText((int)Texts.RemainMonsterValueText).text = string.Empty;

        GetGoodItem(EGoodType.ExpPoint).gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        if (_init == false)
            return;

        UpdatedSkillSlotUI();

        ShowTab(_tab);
    }



    #region Stage UI

    public void UpdateStageUI(bool isBoss = false)
    {
        if (!isBoss)
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

    #region Good, Dia,

    public UI_GoodItem GetGoodItem(EGoodType goodType)
    {
        return goodType switch
        {
            EGoodType.Gold => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Gold),
            EGoodType.Dia => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Dia),
            EGoodType.ExpPoint => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_ExpPoint),
            _ => null,
        };
    }

    public void RefreshShowExp(int currentLevel, float currentExp, float maxExp)
    {
        // 예외처리: expToNextLevel이 0이 아닌 경우에만 계산
        if (maxExp > 0)
        {
            // 경험치 슬라이더와 텍스트 갱신
            GetSlider((int)Sliders.Slider_Exp).value = currentExp / maxExp;

            float expPercentage = (currentExp / maxExp) * 100;
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
                    Managers.UI.ShowPopupUI<UI_EquipmentPopup>().RefreshUI(true);
                    break;
                case PlayTab.Skill:
                    Managers.UI.ShowPopupUI<UI_SkillPopup>().RefreshUI();
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
            Managers.UI.ClosePopupUI();
            ShowPopupActiveGameUI(true);

            switch (tab)
            {
                case PlayTab.Character:
                    GetGoodItem(EGoodType.ExpPoint).gameObject.SetActive(false);
                    break;
                case PlayTab.Equipment:
                    break;
                case PlayTab.Skill:
                    break;
                case PlayTab.Dungeon:
                    break;
                case PlayTab.Draw:
                    break;
                case PlayTab.Shop:
                    break;
            }
        }
    }



    #endregion

    #region Skill Slot
    public void UpdatedSkillSlotUI()
    {
        foreach (var slot in Managers.Backend.GameData.SkillInventory.SkillSlotList)
        {
            _equipSkillSlotList[slot.Index].RefreshUI();
        }
    }

    public void ActiveAutoSkill()
    {
        _isAutoSkillActive = !_isAutoSkillActive;

        if (_isAutoSkillActive)
        {
            Debug.Log("Auto Skill 기능이 활성화되었습니다.");
            GetTMPText((int)Texts.Text_AutoSkill).text = "AUTO\nON";
            CheckUseSkillSlot(-1);
            Managers.Event.AddEvent(EEventType.CompleteSkillCool, new Action<int>(CheckUseSkillSlot));
        }
        else
        {
            Debug.Log("Auto Skill 기능이 비활성화되었습니다.");
            GetTMPText((int)Texts.Text_AutoSkill).text = "AUTO\nOFF";
            Managers.Event.RemoveEvent(EEventType.CompleteSkillCool, new Action<int>(CheckUseSkillSlot));
        }
    }


    public void CheckUseSkillSlot(int slotIndex = -1)
    {
        if (!_isAutoSkillActive) return;


        // slotIndex가 -1인 경우 모든 슬롯을 검사하여 사용할 수 있는 스킬이 있는지 확인
        if (slotIndex == -1)
        {
            foreach (var slot in _equipSkillSlotList)
            {
                if (slot.IsSkillReady())
                    slot.OnUseSkill();
            }
            return;
        } // 특정 슬롯의 스킬만 검사하여 사용할 수 있는지 확인
        else if (slotIndex >= 0 && slotIndex < _equipSkillSlotList.Count)
        {
            if (_equipSkillSlotList[slotIndex].IsSkillReady())
                _equipSkillSlotList[slotIndex].OnUseSkill();
        }
    }
    #endregion



    public void ShowPopupActiveGameUI(bool active)
    {
        GetObject((int)GameObjects.TopStage).SetActive(active);
        Get<CanvasGroup>((int)CanvasGroups.SkillSlotGroup).alpha = active ? 1 : 0;
        Get<CanvasGroup>((int)CanvasGroups.SkillSlotGroup).blocksRaycasts = active;

    }

    public void CloseDrawPopup(UI_Popup popup)
    {
        Managers.UI.ClosePopupUI(popup);
        _tab = PlayTab.None; // 팝업이 닫힐 때 _tab을 None으로 초기화
    }

}
