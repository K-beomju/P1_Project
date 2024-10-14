using BackendData.GameData;
using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    public Dictionary<int, SkillBase> SkillDic = new Dictionary<int, SkillBase>();
    public Hero Owner;

    public void SetInfo(Hero owner)
    {
        Owner = owner;

        // 스킬 슬롯 초기화 (6개 슬롯 고정)
        for (int i = 0; i < 6; i++)
        {
            SkillDic[i] = null;
        }
    }

    public void UpdateHeroSkillData(SkillSlot slot)
    {
        // 만약 슬롯이 잠금 상태라면 => (이미 스킬이 없다는 뜻이니 리턴)
        if (slot.SlotType == Define.ESkillSlotType.Lock)
            return;

        if (!SkillDic.ContainsKey(slot.Index))
        {
            Debug.LogWarning($"{slot.Index}는 유효하지 않은 슬롯 인덱스입니다.");
            return;
        }

        SkillBase skill = SkillDic[slot.Index];

        if (slot.SkillInfoData == null)  // 스킬 해제
        {
            // 이미 스킬이 해제했다면 추가 할 필요 없음
            if (skill == null)
            {
                return;
            }

            Debug.LogWarning($"{slot.Index + 1}번 슬롯에 스킬 해제 완료");
            Destroy(skill);
            SkillDic[slot.Index] = null;
        }
        else  // 스킬 장착
        {
            // 이미 스킬이 있다면 추가 할 필요 없음
            if (skill != null)
            {
                return;
            }

            skill = gameObject.AddComponent(Type.GetType(slot.SkillInfoData.Data.ClassName)) as SkillBase;
            if (skill == null)
            {
                Debug.LogWarning("스킬 생성 실패");
                return;
            }

            skill.SetInfo(Owner, slot.SkillInfoData.Data);
            SkillDic[slot.Index] = skill;
            Debug.LogWarning($"{slot.Index + 1}번 슬롯에 스킬 장착 완료");
        }

    }

    public void UseSkill(int index)
    {
        if (index >= 0 && index < SkillDic.Count)
        {
            SkillDic[index].DoSkill();
        }
    }

}
