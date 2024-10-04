using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using BackEnd;
using System;
using static Define;
using Unity.VisualScripting;
using DG.Tweening;

namespace BackendData.GameData
{

    public partial class UserData
    {
        //public int Level { get; private set; }
        //public float Gold { get; private set; }

        // Purse 각 재화를 담는 Dictionary
        private Dictionary<string, int> _purseDic = new();
        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, int> PurseDic => (IReadOnlyDictionary<string, int>)_purseDic.AsReadOnlyCollection();

    }

    public partial class UserData : Base.GameData
    {

        protected override void InitializeData()
        {
            //Level = 1;
            //Gold = 200;
            _purseDic.Clear();
            _purseDic.Add("Gold", 0);
            _purseDic.Add("Dia", 0);

        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            //Level = int.Parse(gameDataJson["Level"].ToString());
            //Gold = float.Parse(gameDataJson["Gold"].ToString());

            foreach (var column in gameDataJson["Purse"].Keys)
            {
                _purseDic.Add(column, int.Parse(gameDataJson["Purse"][column].ToString()));
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

            //param.Add("Level", Level);
            //param.Add("Gold", Gold);
            param.Add("Purse", PurseDic);

            return param;
        }

        //public void UpdateUserData(float gold)
        //{
        //    IsChangedData = true;
        //    Gold += gold;
        //}


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
    }

}


