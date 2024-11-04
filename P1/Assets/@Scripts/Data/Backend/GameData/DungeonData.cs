using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using static Define;
using System;

namespace BackendData.GameData
{



    public class DungeonData : Base.GameData
    {
        // 현재 진행한 던전 정보
        private Dictionary<string, int> _dungeonLevelDic = new();

        // Dungeon 각 필요한 재화 담는 Dic
        private Dictionary<string, int> _dungeonkeyDic = new();


        public IReadOnlyDictionary<string, int> DungeonLevelDic => (IReadOnlyDictionary<string, int>)_dungeonLevelDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> DungeonKeyDic => (IReadOnlyDictionary<string, int>)_dungeonkeyDic.AsReadOnlyCollection();

        protected override void InitializeData()
        {
            // 던전 정보 초기화 
            _dungeonLevelDic.Clear();
            foreach (EDungeonType dungeonType in Enum.GetValues(typeof(EDungeonType)))
            {
                if (dungeonType == EDungeonType.Unknown) // Unknown 제외
                    continue;

                _dungeonLevelDic.Add(dungeonType.ToString(), 1);
            }

            _dungeonkeyDic.Clear();
            foreach (EDungeonType dungeonType in Enum.GetValues(typeof(EDungeonType)))
            {
                if (dungeonType == EDungeonType.Unknown) // Unknown 제외
                    continue;

                _dungeonLevelDic.Add(dungeonType.ToString(), 1);
            }
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["DungeonKey"].Keys)
            {
                _dungeonkeyDic.Add(column, int.Parse(Data["DungeonKey"][column].ToString()));
            }

            foreach (var column in Data["DungeonLevel"].Keys)
            {
                _dungeonLevelDic.Add(column, int.Parse(Data["DungeonLevel"][column].ToString()));
            }

        }

        public override string GetTableName()
        {
            return "DungeonData";
        }

        public override string GetColumnName()
        {
            return null;

        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("DungeonKey", _dungeonkeyDic);
            param.Add("DungeonLevel", _dungeonLevelDic);

            return param;
        }

        public void AddKey(EDungeonType dungeonType, int amount)
        {
            IsChangedData = true;
            string key = dungeonType.ToString();

            _dungeonkeyDic[key] += amount;
        }
    }
}
