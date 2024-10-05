using System.Collections.Generic;
using LitJson;
using BackEnd;
using Unity.VisualScripting;
using static Define;
using UnityEngine;
using DG.Tweening;
using Data;
using System;
using System.Reflection;
using static Cinemachine.DocumentationSortingAttribute;

namespace BackendData.GameData
{
    //===============================================================
    // UserData 테이블의 뽑기 관련 데이터를 담당하는 클래스
    //===============================================================
    public class DrawData
    {
        public int DrawLevel { get; set; }
        public int DrawCount { get; set; }

        public DrawData(int drawlevel, int drawCount)
        {
            DrawLevel = drawlevel;
            DrawCount = drawCount;
        }
    }

    //===============================================================
    // UserData 테이블의 장비 관련 데이터를 담당하는 클래스
    //===============================================================
    public class EquipmentInfoData
    {
        public int DataTemplateID { get; private set; }
        public EquipmentData Data { get; private set; }
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
            Data = Managers.Data.EquipmentDic[dataTemplateID];
        }
    }

    //===============================================================
    // UserData 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class UserData
    {
        public int Level { get; private set; }
        public float Exp { get; private set; }
        public float MaxExp { get; private set; }

        // 현재 진행한 스테이지 정보 
        public int StageLevel { get; private set; }

        // Purse 각 재화를 담는 Dictionary
        private Dictionary<string, float> _purseDic = new();
        // Draw 각 뽑기 데이터를 담는 Dic
        private Dictionary<string, DrawData> _drawDic = new();
        // Equipment 각 장비 데이터를 담는 Dic 
        private Dictionary<int, EquipmentInfoData> _equipmentInventoryDic = new();
        // Equipment 각 장착한 장비 데이터를 담는 Dic 
        private Dictionary<string, EquipmentInfoData> _equipmentEquipDic = new();

        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, float> PurseDic => (IReadOnlyDictionary<string, float>)_purseDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, DrawData> DrawDic => (IReadOnlyDictionary<string, DrawData>)_drawDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<int, EquipmentInfoData> EquipmentInventoryDic => (IReadOnlyDictionary<int, EquipmentInfoData>)_equipmentInventoryDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, EquipmentInfoData> EquipmentEquipDic => (IReadOnlyDictionary<string, EquipmentInfoData>)_equipmentEquipDic.AsReadOnlyCollection();

    }


    //===============================================================
    // UserData 테이블의 데이터를 담당하는 클래스(함수)
    //===============================================================
    public partial class UserData : Base.GameData
    {

        protected override void InitializeData()
        {
            Level = 1;
            Exp = 0;
            MaxExp = Util.CalculateRequiredExp(Level);
            StageLevel = 1;
            _purseDic.Clear();
            _purseDic.Add("Gold", 0);
            _purseDic.Add("Dia", 0);
            _drawDic.Clear();
            _drawDic.Add("Weapon", new DrawData(1, 0));
            _drawDic.Add("Armor", new DrawData(1, 0));
            _drawDic.Add("Ring", new DrawData(1, 0));

            // 장비 인벤은 처음부터 존재하지 않으니까
            _equipmentInventoryDic.Clear();
            _equipmentEquipDic.Clear();
        }

        // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        // 서버에서 데이터를 불러오는 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고
        protected override void SetServerDataToLocal(JsonData Data)
        {
            Level = int.Parse(Data["Level"].ToString());
            Exp = float.Parse(Data["Exp"].ToString());
            MaxExp = float.Parse(Data["MaxExp"].ToString());

            StageLevel = int.Parse(Data["StageLevel"].ToString());

            foreach (var column in Data["Purse"].Keys)
            {
                _purseDic.Add(column, float.Parse(Data["Purse"][column].ToString()));
            }

            foreach (var column in Data["Draw"].Keys)
            {
                int drawLevel = int.Parse(Data["Draw"][column]["DrawLevel"].ToString());
                int drawCount = int.Parse(Data["Draw"][column]["DrawCount"].ToString());
                _drawDic.Add(column, new DrawData(drawLevel, drawCount));
            }

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
            return "UserData";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();

            param.Add("Level", Level);
            param.Add("Exp", Exp);
            param.Add("MaxExp", MaxExp);
            param.Add("StageLevel", StageLevel);
            param.Add("Purse", _purseDic);
            param.Add("Draw", _drawDic);
            param.Add("EquipmentEquip", _equipmentEquipDic);
            param.Add("EquipmentInventory", _equipmentInventoryDic);
            return param;
        }

        #region User Methods 

        // 유저의 재화를 변경하는 함수
        public void AddAmount(EGoodType goodType, int amount)
        {
            IsChangedData = true;
            string key = goodType.ToString();

            if (!_purseDic.ContainsKey(key))
            {
                _purseDic.Add(key, amount);
            }
            else
            {
                _purseDic[key] += amount;
            }
        }

        // 유저의 경험치를 변경하는 함수
        public void AddExp(int exp)
        {
            IsChangedData = true;
            Exp += exp;

            // 레벨업 처리
            while (Exp >= MaxExp)
            {
                LevelUp();
            }

            Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, Level, Exp, MaxExp); // 경험치 갱신 이벤트
        }

        // 유저의 레벨을 변경하는 함수
        public void LevelUp()
        {
            Exp -= MaxExp;

            MaxExp = Util.CalculateRequiredExp(Level);

            Level++;

            Managers.Event.TriggerEvent(EEventType.PlayerLevelUp, Level); // 레벨업 이벤트 발생
        }

        #endregion

        #region Stage Methods 

        // 유저의 스테이지 정보를 변경하는 함수
        public void UpdateStageLevel(int stageLevel)
        {
            IsChangedData = true;
            StageLevel += stageLevel;
            if (StageLevel == 0)
                StageLevel = 1;
        }

        #endregion


        #region Draw Methods 

        // 유저의 뽑기 횟수를 변경하는 함수
        public void AddDrawCount(EEquipmentType equipmentType)
        {
            IsChangedData = true;
            string key = equipmentType.ToString();

            _drawDic[key].DrawCount++;

            while (_drawDic[key].DrawCount >= Managers.Data.GachaDataDic[_drawDic[key].DrawLevel].MaxExp)
            {
                DrawLevelUp(key);
            }

        }

        // 유저의 뽑기 레벨을 변경하는 함수
        public void DrawLevelUp(string key)
        {
            _drawDic[key].DrawCount -= Managers.Data.GachaDataDic[Level].MaxExp;
            _drawDic[key].DrawLevel++;
            Managers.Event.TriggerEvent(EEventType.DrawLevelUpUIUpdated, _drawDic[key].DrawLevel);
        }

        #endregion


        #region Equipment Methods

        // 유저의 장비 아이템을 추가하는 함수
        public void AddEquipment(int dataTemplateID)
        {
            IsChangedData = true;
            if (_equipmentInventoryDic.TryGetValue(dataTemplateID, out EquipmentInfoData equipEquipmentInfo)) {
                equipEquipmentInfo.Count += 1;
                //Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
            }
            else {
                equipEquipmentInfo = new EquipmentInfoData(dataTemplateID, EOwningState.Owned, 1, 1, false);
                _equipmentInventoryDic.Add(dataTemplateID, equipEquipmentInfo);
            }
        }

        public void EquipEquipment(int dataTemplateID)
        {
            IsChangedData = true;

            // 장착할 장비가 인벤토리에 있는지 확인
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

                // 새롭게 장착한 장비도 인벤토리 Dictionary에 업데이트 (동기화)
                _equipmentInventoryDic[dataTemplateID] = equipmentInventoryData;
            }
            else
            {
                Debug.LogWarning($"장착하려는 장비({dataTemplateID})가 인벤토리에 없습니다.");
            }

            //Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
        }

        public void EquipmentLevelUp(EquipmentInfoData equipmentInfoData)
        {
            IsChangedData = true;

            int maxCount = Util.GetUpgradeEquipmentMaxCount(equipmentInfoData.Level);

            _equipmentInventoryDic[equipmentInfoData.DataTemplateID].Level += 1;
            _equipmentInventoryDic[equipmentInfoData.DataTemplateID].Count -= maxCount;

        }

        #endregion

    }

}


