using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace BackendData.GameData
{



    public class DungeonData : Base.GameData
    {
        // Dungeon 각 필요한 재화 담는 Dic
        private Dictionary<string, int> _dungeonFeeDic = new();
        
        public IReadOnlyDictionary<string, int> DungeonFeeDic => (IReadOnlyDictionary<string, int>)_dungeonFeeDic.AsReadOnlyCollection();

        protected override void InitializeData()
        {
            _dungeonFeeDic.Clear();
            _dungeonFeeDic.Add("Gold", 2);
            _dungeonFeeDic.Add("Dia", 2);
            //_dungeonFeeDic.Add("Promotion", 2);
            //_dungeonFeeDic.Add("WorldBoss", 2);
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data.Keys)
            {
                _dungeonFeeDic.Add(column, int.Parse(Data[column].ToString()));
                Debug.Log(column);
            }
            
        }

        public override string GetTableName()
        {
            return "DungeonData";
        }

        public override string GetColumnName()
        {
            return "DungeonData";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), _dungeonFeeDic);
            return param;
        }
    }
}
