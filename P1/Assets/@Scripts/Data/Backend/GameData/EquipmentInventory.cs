using System.Collections.Generic;
using LitJson;
using BackEnd;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static Define;


namespace BackendData.GameData
{

    //===============================================================
    // EquipmentInventory 테이블의 장비 EquipmentInfoData 클래스
    //===============================================================
    public class EquipmentInfoData
    {
        public int DataTemplateID { get; private set; }
        public BackendData.Chart.Equipment.Item Data { get; private set; }
        public EOwningState OwningState { get; set; }

        public int Level { get; set; }
        public int Count { get; set; }
        public bool IsEquipped { get; set; }

        public EquipmentInfoData(int dataTemplateID, EOwningState owningState, int level, int count, bool isEquipped)
        {
            DataTemplateID = dataTemplateID;
            OwningState = owningState;
            Level = level;
            Count = count;
            IsEquipped = isEquipped;
            Data = Managers.Backend.Chart.Equipment.Dic[dataTemplateID]; //Managers.Data.EquipmentDic[dataTemplateID];
        }
    }

    //===============================================================
    // EquipmentInventory 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class EquipmentInventory
    {
        // Equipment 각 장비 데이터를 담는 Dic 
        private Dictionary<int, EquipmentInfoData> _equipmentInventoryDic = new();
        // Equipment 각 장착한 장비 데이터를 담는 Dic 
        private Dictionary<string, EquipmentInfoData> _equipmentEquipDic = new();

        public IReadOnlyDictionary<int, EquipmentInfoData> EquipmentInventoryDic => (IReadOnlyDictionary<int, EquipmentInfoData>)_equipmentInventoryDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, EquipmentInfoData> EquipmentEquipDic => (IReadOnlyDictionary<string, EquipmentInfoData>)_equipmentEquipDic.AsReadOnlyCollection();
    }


    //===============================================================
    // EquipmentInventory 테이블의 데이터를 담당하는 클래스(함수)
    //===============================================================
    public partial class EquipmentInventory : Base.GameData
    {
        protected override void InitializeData()
        {
            // 장비 인벤은 처음부터 존재하지 않으니까
            _equipmentInventoryDic.Clear();
            _equipmentEquipDic.Clear();
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["EquipmentInventory"].Keys)
            {
                int dataId = int.Parse(Data["EquipmentInventory"][column]["DataTemplateID"].ToString());
                int level = int.Parse(Data["EquipmentInventory"][column]["Level"].ToString());
                int count = int.Parse(Data["EquipmentInventory"][column]["Count"].ToString());
                bool isEquipped = Boolean.Parse(Data["EquipmentInventory"][column]["IsEquipped"].ToString());

                _equipmentInventoryDic.Add(dataId, new EquipmentInfoData(dataId, EOwningState.Owned, level, count, isEquipped));
            }

            foreach (var column in Data["EquipmentEquip"].Keys)
            {
                string equipmentType = ((EEquipmentType)int.Parse(Data["EquipmentEquip"][column]["Data"]["EquipmentType"].ToString())).ToString();
                int dataId = int.Parse(Data["EquipmentEquip"][column]["DataTemplateID"].ToString());
                int level = int.Parse(Data["EquipmentEquip"][column]["Level"].ToString());
                int count = int.Parse(Data["EquipmentEquip"][column]["Count"].ToString());
                bool isEquipped = Boolean.Parse(Data["EquipmentEquip"][column]["IsEquipped"].ToString());

                _equipmentEquipDic.Add(equipmentType, new EquipmentInfoData(dataId, EOwningState.Owned, level, count, isEquipped));
            }
        }

        public override string GetTableName()
        {
            return "EquipmentInventory";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("EquipmentEquip", _equipmentEquipDic);
            param.Add("EquipmentInventory", _equipmentInventoryDic);
            return param;
        }



        #region Equipment Methods

        // 유저의 장비 아이템을 추가하는 함수
        public void AddEquipment(int dataTemplateID)
        {
            IsChangedData = true;
            if (_equipmentInventoryDic.TryGetValue(dataTemplateID, out EquipmentInfoData equipEquipmentInfo))
            {
                equipEquipmentInfo.Count += 1;
                Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
            }
            else
            {
                equipEquipmentInfo = new EquipmentInfoData(dataTemplateID, EOwningState.Owned, 1, 1, false);
                _equipmentInventoryDic.Add(dataTemplateID, equipEquipmentInfo);
            }
        }

        public void EquipEquipment(int dataTemplateID)
        {
            IsChangedData = true;

            //장착할 장비가 인벤토리에 있는지 확인
            if (_equipmentInventoryDic.TryGetValue(dataTemplateID, out EquipmentInfoData equipmentInventoryData))
            {
                // 현재 장착 중인 장비가 있는지 확인 (해당 EquipmentType을 기준으로)
                if (_equipmentEquipDic.TryGetValue(equipmentInventoryData.Data.EquipmentType.ToString(), out EquipmentInfoData currentlyEquippedData))
                {
                    // 기존에 장착된 장비가 있다면 IsEquipped를 false로 설정
                    currentlyEquippedData.IsEquipped = false;

                    _equipmentInventoryDic[currentlyEquippedData.DataTemplateID] = currentlyEquippedData;

                }

                // 새롭게 장착할 장비의 IsEquipped를 true로 설정하고 장착된 장비 Dictionary에 업데이트
                equipmentInventoryData.IsEquipped = true;
                _equipmentEquipDic[equipmentInventoryData.Data.EquipmentType.ToString()] = equipmentInventoryData;
                Managers.Object.Hero.ChangeAnimController(true);

                // 새롭게 장착한 장비도 인벤토리 Dictionary에 업데이트 (동기화)
                _equipmentInventoryDic[dataTemplateID] = equipmentInventoryData;
                Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
            }
            else
            {
                Debug.LogWarning($"장착하려는 장비({dataTemplateID})가 인벤토리에 없습니다.");
            }
        }

        public void EquipmentLevelUp(EquipmentInfoData equipmentInfoData, int maxCount)
        {
            IsChangedData = true;

            _equipmentInventoryDic[equipmentInfoData.DataTemplateID].Level += 1;
            _equipmentInventoryDic[equipmentInfoData.DataTemplateID].Count -= maxCount;

            // 만약 현재 착용한 장비를 업그레이드 했다면, 착용 장비 딕셔너리 업데이트
            if (_equipmentEquipDic.ContainsKey(equipmentInfoData.Data.EquipmentType.ToString()))
            {
                _equipmentEquipDic[equipmentInfoData.Data.EquipmentType.ToString()] = _equipmentInventoryDic[equipmentInfoData.DataTemplateID];
            }

        }

        #endregion
    }


}
