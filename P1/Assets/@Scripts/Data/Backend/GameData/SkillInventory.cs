using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using static Define;
using Unity.VisualScripting;
using BackEnd;
using System;
using Data;

namespace BackendData.GameData
{
    public class SkillSlot
    {
        public int Index { get; private set; }
        public ESkillSlotType SlotType { get; private set; }
        public SkillInfoData SkillInfoData { get; private set; }

        public bool IsCoolTimeActive { get; set; } // 쿨타임을 돌려야 하는지 여부를 확인하는 플래그
        public Action<int> OnSkillUnEquipped; // 스킬 해제 시 슬롯 인덱스를 전달하는 이벤트

        public SkillSlot(int index, ESkillSlotType slotType, SkillInfoData skillInfoData)
        {
            Index = index;
            SlotType = slotType;
            SkillInfoData = skillInfoData;
            IsCoolTimeActive = false;
        }

        // 슬롯에 스킬 장착
        public void EquipSkill(SkillInfoData skillInfoData)
        {
            SlotType = ESkillSlotType.Equipped;
            SkillInfoData = skillInfoData;
            IsCoolTimeActive = true;
            Managers.Event.TriggerEvent(EEventType.UpdatedSkillSlot);

        }

        // 슬롯에서 스킬 해제
        public void UnEquipSkill()
        {
            SlotType = ESkillSlotType.None;
            SkillInfoData = null;
            IsCoolTimeActive = false;
            Managers.Event.TriggerEvent(EEventType.UpdatedSkillSlot);

            OnSkillUnEquipped?.Invoke(Index);
        }

        // 슬롯 잠금 상태 설정 
        public void LockSlot()
        {
            SlotType = ESkillSlotType.Lock;
            SkillInfoData = null;
        }

        // 슬롯 비어있는 상태 설정 
        public void EmptySlot()
        {
            SlotType = ESkillSlotType.None;
            SkillInfoData = null;
        }
    }
    //===============================================================
    // EquipmentInventory 테이블의 장비 EquipmentInfoData 클래스
    //===============================================================
    public class SkillInfoData : Item
    {
        public SkillData Data { get; private set; }
        public override string Name => Data.Name;
        public override string SpriteKey => Data.SpriteKey;

        public SkillInfoData(int dataTemplateID, EOwningState owningState, int level, int count, bool isEquipped)
            : base(dataTemplateID, owningState, level, count, isEquipped)
        {
            Data = Managers.Data.SkillChart[dataTemplateID];
        }
    }

    //===============================================================
    // SkillInventory 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class SkillInventory
    {
        // Skill 각 스킬 데이터를 담는 Dic 
        private Dictionary<int, SkillInfoData> _skillInventoryDic = new();
        // Skill 각 스킬 슬롯을 담는 List
        private List<SkillSlot> _skillSlotList = new();

        public IReadOnlyDictionary<int, SkillInfoData> SkillInventoryDic => (IReadOnlyDictionary<int, SkillInfoData>)_skillInventoryDic.AsReadOnlyCollection();
        public IReadOnlyList<SkillSlot> SkillSlotList => (IReadOnlyList<SkillSlot>)_skillSlotList.AsReadOnlyList();
    }


    public partial class SkillInventory : Base.GameData
    {
        protected override void InitializeData()
        {
            _skillInventoryDic.Clear();
            _skillSlotList.Clear();
            // 6개의 슬롯 생성 
            for (int i = 0; i < 6; i++)
            {
                _skillSlotList.Add(new SkillSlot(i, slotType: ESkillSlotType.Lock, null));
            }
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["SkillInventory"].Keys)
            {
                int dataId = int.Parse(Data["SkillInventory"][column]["DataTemplateID"].ToString());
                int level = int.Parse(Data["SkillInventory"][column]["Level"].ToString());
                int count = int.Parse(Data["SkillInventory"][column]["Count"].ToString());
                bool isEquipped = Boolean.Parse(Data["SkillInventory"][column]["IsEquipped"].ToString());

                _skillInventoryDic.Add(dataId, new SkillInfoData(dataId, EOwningState.Owned, level, count, isEquipped));
            }

            for (int i = 0; i < 6; i++)
            {
                int index = int.Parse(Data["SkillSlot"][i]["Index"].ToString());
                ESkillSlotType skillSlotType = (ESkillSlotType)int.Parse(Data["SkillSlot"][i]["SlotType"].ToString());

                SkillInfoData skillInfoData = null;
                // SkillInfoData가 존재하고 빈 객체가 아닌지 확인 해야함 
                if (Data["SkillSlot"][i]["SkillInfoData"].IsObject)
                {
                    int dataTemplateID = int.Parse(Data["SkillSlot"][i]["SkillInfoData"]["DataTemplateID"].ToString());
                    int level = int.Parse(Data["SkillSlot"][i]["SkillInfoData"]["Level"].ToString());
                    int count = int.Parse(Data["SkillSlot"][i]["SkillInfoData"]["Count"].ToString());
                    bool isEquipped = bool.Parse(Data["SkillSlot"][i]["SkillInfoData"]["IsEquipped"].ToString());

                    skillInfoData = new SkillInfoData(dataTemplateID, EOwningState.Owned, level, count, isEquipped);
                }

                _skillSlotList.Add(new SkillSlot(index, skillSlotType, skillInfoData));
            }
            if (_skillSlotList[0].SlotType == ESkillSlotType.Lock)
            {
                _skillSlotList[0].EmptySlot();
                _skillSlotList[1].EmptySlot();
            }

        }

        public override string GetTableName()
        {
            return "SkillInventory";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("SkillInventory", _skillInventoryDic);
            param.Add("SkillSlot", _skillSlotList);
            return param;
        }

        public void AddSkill(int dataTemplateID)
        {
            IsChangedData = true;
            if (_skillInventoryDic.TryGetValue(dataTemplateID, out SkillInfoData skillInfoData))
            {
                skillInfoData.Count += 1;
                // 보유효과 처리 
            }
            else
            {
                skillInfoData = new SkillInfoData(dataTemplateID, EOwningState.Owned, 1, 1, false);
                _skillInventoryDic.Add(dataTemplateID, skillInfoData);
            }
        }

        public void EquipSkill(int dataTemplateID, Action<int> onEquipResult = null)
        {
            IsChangedData = true;

            if (!_skillInventoryDic.TryGetValue(dataTemplateID, out SkillInfoData skillInfoData))
            {
                Debug.LogWarning($"스킬 인벤토리에 존재하지 않은 {dataTemplateID}를 장착할려고 합니다..");
                onEquipResult?.Invoke(-1); // 실패 시 false 반환
                return;
            }

            //장착할 스킬이 있는지 확인 
            for (int i = 0; i < _skillSlotList.Count; i++)
            {
                if (_skillSlotList[i] == null)
                    return;

                SkillSlot skillSlot = _skillSlotList[i];
                if (skillSlot.SlotType == ESkillSlotType.None)
                {
                    Debug.LogWarning($"{i + 1}번째 슬롯의 <color=#FF0000>{skillInfoData.Name}</color> 스킬이 장착되었습니다.");
                    skillInfoData.IsEquipped = true;
                    skillSlot.EquipSkill(skillInfoData);
                    onEquipResult?.Invoke(i); // 성공 시 슬롯 인덱스를 반환
                    return;

                }
            }

            Debug.LogWarning("스킬을 장착할 수 있는 빈 슬롯이 없습니다.");
            onEquipResult?.Invoke(-1); // 실패 시 -1 반환
        }


        public void UnEquipSkill(int dataTemplateID, Action<int> onEquipResult = null)
        {
            IsChangedData = true;

            if (!_skillInventoryDic.TryGetValue(dataTemplateID, out SkillInfoData skillInfoData))
            {
                Debug.LogWarning($"스킬 인벤토리에 존재하지 않은 {dataTemplateID}를 해제할려고 합니다..");
                onEquipResult?.Invoke(-1); // 실패 시 false 반환
                return;
            }


            //해제할 스킬이 있는지 확인 
            for (int i = _skillSlotList.Count - 1; i >= 0; i--)
            {
                // 슬롯이 null인 경우 건너뜀
                if (_skillSlotList[i] == null || _skillSlotList[i].SkillInfoData == null)
                    continue;

                // 해제할려는 스킬 ID와 선택된 ID중 같은 게 있는지 확인 
                if (_skillSlotList[i].SkillInfoData.DataTemplateID == dataTemplateID)
                {
                    SkillSlot skillSlot = _skillSlotList[i];

                    if (skillSlot.SlotType == ESkillSlotType.Equipped)
                    {
                        Debug.LogWarning($"{i + 1}번째 슬롯의 <color=#FF0000>{skillInfoData.Name}</color> 스킬이 해제되었습니다.");
                        skillInfoData.IsEquipped = false;
                        skillSlot.UnEquipSkill();
                        onEquipResult?.Invoke(i); // 성공 시 슬롯 인덱스를 반환
                        return;

                    }
                }
            }
        }

    }
}