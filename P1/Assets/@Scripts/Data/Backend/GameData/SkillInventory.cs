using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using static Define;
using Unity.VisualScripting;
using BackEnd;
using System;

namespace BackendData.GameData
{
    //===============================================================
    // EquipmentInventory 테이블의 장비 EquipmentInfoData 클래스
    //===============================================================
    public class SkillInfoData
    {
        public int DataTemplateID { get; private set; }
        public BackendData.Chart.Skill.Item Data { get; private set; } // 수정 필여
        public EOwningState OwningState { get; set; }

        public int Level { get; set; }
        public int Count { get; set; }
        public bool IsEquipped { get; set; }

        public SkillInfoData(int dataTemplateID, EOwningState owningState, int level, int count, bool isEquipped)
        {
            DataTemplateID = dataTemplateID;
            OwningState = owningState;
            Level = level;
            Count = count;
            IsEquipped = isEquipped;
            Data = Managers.Backend.Chart.Skill.Dic[dataTemplateID]; // 수정 필여
        }
    }

    //===============================================================
    // SkillInventory 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class SkillInventory
    {
        // Skill 각 스킬 데이터를 담는 Dic 
        private Dictionary<int, SkillInfoData> _skillInventoryDic = new();
        // Skill 각 장착한 스킬 데이터를 담는 Dic 
        private Dictionary<int, SkillInfoData> _skillEquipDic = new();

        public IReadOnlyDictionary<int, SkillInfoData> SkillInventoryDic => (IReadOnlyDictionary<int, SkillInfoData>)_skillInventoryDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<int, SkillInfoData> SkillEquipDic => (IReadOnlyDictionary<int, SkillInfoData>)_skillEquipDic.AsReadOnlyCollection();
    }


    public partial class SkillInventory : Base.GameData
    {
        protected override void InitializeData()
        {
            _skillEquipDic.Clear();
            _skillInventoryDic.Clear();
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

            foreach (var column in Data["SkillEquip"].Keys)
            {
                int dataId = int.Parse(Data["SkillEquip"][column]["DataTemplateID"].ToString());
                int level = int.Parse(Data["SkillEquip"][column]["Level"].ToString());
                int count = int.Parse(Data["SkillEquip"][column]["Count"].ToString());
                bool isEquipped = Boolean.Parse(Data["SkillEquip"][column]["IsEquipped"].ToString());

                _skillEquipDic.Add(dataId, new SkillInfoData(dataId, EOwningState.Owned, level, count, isEquipped));
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
            param.Add("SkillEquip", _skillEquipDic);
            param.Add("SkillInventory", _skillInventoryDic);
            return param;
        }

    }
}
