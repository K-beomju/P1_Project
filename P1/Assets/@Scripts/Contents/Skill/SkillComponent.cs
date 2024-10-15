using BackendData.GameData;
using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    public Dictionary<int, SkillData> SkillDic = new Dictionary<int, SkillData>();
    public Hero Owner;

    public void SetInfo(Hero owner)
    {
        Owner = owner;

        // 스킬 슬롯 초기화 (6개 슬롯 고정)
        for (int i = 0; i < Managers.Backend.GameData.SkillInventory.SkillSlotList.Count; i++)
        {
            SkillDic[i] = null;
            AddSkill(i);
        }
    }

    public void AddSkill(int index)
    {
        SkillSlot slot = Managers.Backend.GameData.SkillInventory.SkillSlotList[index];
        if (slot.SkillInfoData == null)
            return;

        // SkillData만을 사용하여 스킬을 추가
        SkillData skillData = slot.SkillInfoData.Data;
        if (skillData == null)
        {
            Debug.LogWarning("스킬 데이터가 없음");
            return;
        }
        SkillDic[index] = skillData;
    }

    public void RemoveSkill(int index)
    {
        if (SkillDic.ContainsKey(index))
        {
            SkillDic[index] = null;
        }
    }

    public void UseSkill(int index)
    {
        if (SkillDic.ContainsKey(index) && SkillDic[index] != null)
        {
            // SkillData를 사용하여 이펙트를 소환하거나 스킬을 실행
            SpawnEffect(index);
        }
    }

    public void SpawnEffect(int index)
    {
        SkillData skillData = SkillDic[index];
        GameObject effectObj = Managers.Object.SpawnGameObject(Owner.CenterPosition, skillData.PrefabKey);
        
        // 이펙트를 주인(Owner)에게 붙일지 여부를 확인
        if (skillData.AttachToOwner)
        {
            effectObj.transform.SetParent(Owner.transform, false);
            effectObj.transform.localPosition = Vector3.zero;
        }

        Vector3 ownerScale = Owner.transform.localScale;
        effectObj.transform.localScale = new Vector3(ownerScale.x > 0 ? Mathf.Abs(effectObj.transform.localScale.x) : -Mathf.Abs(effectObj.transform.localScale.x),
                                                     effectObj.transform.localScale.y,
                                                     effectObj.transform.localScale.z);

        // 이펙트의 컴포넌트를 받아와 설정
        EffectBase effect = effectObj.GetComponent<EffectBase>();
        if (effect != null)
        {
            effect.SetInfo(SkillDic[index].EffectId, Owner, SkillDic[index]);
        }
    }
}
