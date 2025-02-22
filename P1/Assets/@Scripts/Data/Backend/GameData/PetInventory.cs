using BackEnd;
using Data;
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
        public EOwningState OwningState { get; set; }
        public bool IsEquipped { get; set; }

        public PetInfoData(int level, int count, EOwningState owningState, bool isEquipped)
        {
            Level = level;
            Count = count;
            OwningState = owningState;
            IsEquipped = isEquipped;
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
                _petInventoryDic.Add(petType.ToString(), new PetInfoData(1, 0, EOwningState.Unowned, false));
            }
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["PetInventory"].Keys)
            {
                int level = int.Parse(Data["PetInventory"][column]["Level"].ToString());
                int count = int.Parse(Data["PetInventory"][column]["Count"].ToString());
                EOwningState owningState = (EOwningState)int.Parse(Data["PetInventory"][column]["OwningState"].ToString());
                bool isEquipped = Boolean.Parse(Data["PetInventory"][column]["IsEquipped"].ToString());

                _petInventoryDic.Add(column, new PetInfoData(level, count, owningState, isEquipped));

            }

            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Silver, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Blue, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Flame, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Scale, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Book, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Rune, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Emerald, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Wood, 100);
            // Managers.Backend.GameData.PetInventory.AddPetCraft(EPetType.Gold, 100);

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
                _petInventoryDic.Add(key, new PetInfoData(0, 1, EOwningState.Unowned, false));
            }
            else
            {
                _petInventoryDic[key].Count += amount;
            }

            Managers.Event.TriggerEvent(EEventType.PetItemUpdated);
        }

        public void EquipPet(EPetType petType)
        {
            IsChangedData = true;
            string key = petType.ToString();

            // 펫이 인벤토리에 존재하는지 확인
            if (_petInventoryDic.TryGetValue(key, out PetInfoData petInfoData))
            {
                // 펫이 소유되어 있고, 이미 장착된 펫이 있다면 먼저 해제
                if (petInfoData.OwningState == EOwningState.Owned && !petInfoData.IsEquipped)
                {
                    // 기존에 장착된 펫을 찾아서 해제
                    foreach (var existingPet in _petInventoryDic.Values)
                    {
                        if (existingPet.IsEquipped)
                        {
                            Managers.Object.DespawnPet();
                            existingPet.IsEquipped = false;
                            break;  // 하나만 찾으면 되므로 루프 종료
                        }
                    }

                    // 새 펫을 장착
                    petInfoData.IsEquipped = true;
                }
            }
            else
            {
                Debug.LogWarning($"펫 인벤토리에 {petType} 펫이 없습니다.");
            }
        }

        public void UnEquipPet(EPetType petType)
        {
            IsChangedData = true;
            string key = petType.ToString();

            if (_petInventoryDic.TryGetValue(key, out PetInfoData petInfoData))
            {
                if (petInfoData.OwningState == EOwningState.Owned && petInfoData.IsEquipped)
                {
                    petInfoData.IsEquipped = false;
                }
            }
        }

        public void PetLevelUp(EPetType petType, int maxCount)
        {
            IsChangedData = true;
            string key = petType.ToString();

            _petInventoryDic[key].Level += 1;
            _petInventoryDic[key].Count -= maxCount;
        }

        public void MakePet(EPetType petType)
        {
            IsChangedData = true;
            string key = petType.ToString();

            if (_petInventoryDic.ContainsKey(key))
            {
                _petInventoryDic[key].OwningState = EOwningState.Owned;
            }
        }

        public bool IsEquipPet(EPetType petType)
        {
            return _petInventoryDic[petType.ToString()].IsEquipped;
        }

        public PetData EquipCheckPet()
        {
            foreach (var pet in _petInventoryDic)
            {
                if(pet.Value.IsEquipped)
                {
                    return Managers.Data.PetChart[Util.ParseEnum<EPetType>(pet.Key)];
                }
            }
            return null;
        }

    }

}
