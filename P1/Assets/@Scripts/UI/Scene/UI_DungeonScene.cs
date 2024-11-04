using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonScene : UI_Scene
{
    enum Sliders
    {
        Slider_DungeonInfo,
        Slider_RemainTimer
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
    private bool _isAutoSkillActive = false; // AutoSkill 활성화 상태를 저장하는 변수


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindSliders(typeof(Sliders));
        Bind<UI_EquipSkillSlot>(typeof(EquipSkillSlots));

        // 6개의 슬롯을 초기화
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            _equipSkillSlotList.Add(Get<UI_EquipSkillSlot>(i));
            _equipSkillSlotList[i].SetInfo(index);
        }
        return true;
    }
}
