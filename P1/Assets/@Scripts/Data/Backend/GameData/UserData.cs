using System.Collections.Generic;
using LitJson;
using BackEnd;
using Unity.VisualScripting;
using UnityEngine;
using Data;
using System;
using static Define;

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

        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, float> PurseDic => (IReadOnlyDictionary<string, float>)_purseDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, DrawData> DrawDic => (IReadOnlyDictionary<string, DrawData>)_drawDic.AsReadOnlyCollection();

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

    }

}


