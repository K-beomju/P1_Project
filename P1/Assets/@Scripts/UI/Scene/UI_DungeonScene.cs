using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DungeonScene : UI_Scene
{
    enum Buttons
    {
        Btn_AutoSkill
    }

    enum Texts
    {
        Text_DungeonInfo,
        Text_AutoSkill,
        ExpValueText
    }
    enum Sliders
    {
        Slider_DungeonInfo,
        Slider_RemainTimer,
        Slider_Exp
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

    public List<UI_EquipSkillSlot> _equipSkillSlotList { get; set; } = new List<UI_EquipSkillSlot>();


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        Bind<UI_EquipSkillSlot>(typeof(EquipSkillSlots));

        GetButton((int)Buttons.Btn_AutoSkill).gameObject.BindEvent(ActiveAutoSkill);

        InitializeUIElements();
        Managers.Event.AddEvent(EEventType.MonsterCountChanged, new Action<int, int>(RefreshShowRemainMonster));
        Managers.Event.AddEvent(EEventType.ExperienceUpdated, new Action<int, float, float>(RefreshShowExp));

        RefreshUI();
        return true;
    }

    private void InitializeUIElements()
    {
        // 6개의 슬롯을 초기화
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            _equipSkillSlotList.Add(Get<UI_EquipSkillSlot>(i));
            _equipSkillSlotList[i].SetInfo(index);
        }

        UpdateAutoSkillUI(Managers.Backend.GameData.SkillInventory.IsAutoSkill);
    }

    
    private void RefreshUI()
    {
        if (_init == false)
            return;

        UpdatedSkillSlotUI();
    }

    public void UpdateStageUI(EDungeonType type, int level)
    {
        GetTMPText((int)Texts.Text_DungeonInfo).text = $"{Util.GetDungeonType(type)} {level}";
    }

    public void RefreshShowRemainMonster(int killMonster, int maxMonster)
    {
        GetSlider((int)Sliders.Slider_DungeonInfo).maxValue = maxMonster;
        GetSlider((int)Sliders.Slider_DungeonInfo).value = killMonster;
    }

    public void RefreshDungeonTimer(float currentTime, float maxTime)
    {
        float timePercentage = currentTime / maxTime;
        GetSlider((int)Sliders.Slider_RemainTimer).value = timePercentage;
    }

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
        Managers.Backend.GameData.SkillInventory.ActiveAutoSkill();
        UpdateAutoSkillUI(Managers.Backend.GameData.SkillInventory.IsAutoSkill);
    }

    private void UpdateAutoSkillUI(bool isAutoSkill)
    {
        if (isAutoSkill)
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
        if (!Managers.Backend.GameData.SkillInventory.IsAutoSkill) return;


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


    #region Exp

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

}
