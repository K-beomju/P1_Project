using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

namespace BackendData.GameData
{
    public class PetInfoData 
    {
        public int Level { get; set; }
        public int Count { get; set; }

        public PetInfoData(int level, int count)
        {
            Level = level;
            Count = count;
        }
    }
    //===============================================================
    // EquipmentInventory 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class PetInventory
    {
        private Dictionary<string, PetInfoData> _petInventoryDic = new();

        public IReadOnlyDictionary<string, PetInfoData> PetInventoryDic => (IReadOnlyDictionary<string, PetInfoData>)_petInventoryDic.AsReadOnlyCollection();
    }

    //===============================================================
    // EquipmentInventory 테이블의 데이터를 담당하는 클래스(함수)
    //===============================================================
    public partial class PetInventory : Base.GameData
    {

        protected override void InitializeData()
        {
            // 펫 알 조각 보유 정보 초기화 
            _petInventoryDic.Clear();
            foreach (EPetType petType in Enum.GetValues(typeof(EPetType)))
            {
                _petInventoryDic.Add(petType.ToString(), new PetInfoData(1, 0));
            }
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["PetInventory"].Keys)
            {
                int level = int.Parse(Data["PetInventory"][column]["Level"].ToString());
                int count = int.Parse(Data["PetInventory"][column]["Count"].ToString());
                _petInventoryDic.Add(column, new PetInfoData(level, count));

            }
        }

        public override string GetTableName()
        {
            return "PetInventory";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("PetInventory", _petInventoryDic);
            return param;
        }


        public void AddPetCraft(EPetType petType, int amount)
        {
            IsChangedData = true;
            string key = petType.ToString();

            if (!_petInventoryDic.ContainsKey(key))
            {
                _petInventoryDic.Add(key, new PetInfoData(0, 1));
            }
            else
            {
                _petInventoryDic[key].Count += amount;
            }

            Managers.Event.TriggerEvent(EEventType.PetItemUpdated);
        }


    }

}
