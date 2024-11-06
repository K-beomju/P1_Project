using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using static Define;
using System;
using System.Globalization;

namespace BackendData.GameData
{



    public class DungeonData : Base.GameData
    {
        // 현재 진행한 던전 정보
        private Dictionary<string, int> _dungeonLevelDic = new();

        // Dungeon 각 필요한 재화 담는 Dic
        private Dictionary<string, int> _dungeonkeyDic = new();

        public string LastLoginTime { get; private set; }


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

                _dungeonkeyDic.Add(dungeonType.ToString(), Util.DungenEntranceMaxValue(dungeonType));
            }

            BackendReturnObject servertime = Backend.Utils.GetServerTime();

            string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
            DateTime parsedDate = DateTime.Parse(time);
            LastLoginTime = parsedDate.ToString();
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

            LastLoginTime = Data["LastLoginTime"].ToString();
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
            param.Add("LastLoginTime", LastLoginTime);

            return param;
        }

        public void CheckDungeonKeyRecharge()
        {
            BackendReturnObject serverTime = Backend.Utils.GetServerTime();
            if (!serverTime.IsSuccess())
            {
                Debug.LogError("Failed to get server time.");
                return;
            }

            if (string.IsNullOrEmpty(LastLoginTime))
                return;


            // 서버 시간 문자열을 DateTime 형식으로 변환
            string time = serverTime.GetReturnValuetoJSON()["utcTime"].ToString();
            DateTime servertime = DateTime.Parse(time);

            DateTime lastLoginDate = DateTime.Parse(LastLoginTime);
            TimeSpan timeDifference = servertime - lastLoginDate;
            Debug.Log(timeDifference);

            if (timeDifference.TotalSeconds >= 12)
            {
                //하루가 지난 경우 LastLoginTime을 서버 시간으로 업데이트
                Debug.Log("3초 이상 차이난 마지막 로그인 시간 " + LastLoginTime);

                IsChangedData = true;
                // 열쇠 주고 마지막 로그인 시간 업데이트. 
                LastLoginTime = servertime.ToString();
                foreach (EDungeonType dungeonType in Enum.GetValues(typeof(EDungeonType)))
                {
                    if (dungeonType == EDungeonType.Unknown) // Unknown 제외
                        continue;

                    // 현재 키 개수가 1개 이하일 때 최대갯수까지 채움
                    if (_dungeonkeyDic[dungeonType.ToString()] <= 1)
                    {
                        _dungeonkeyDic[dungeonType.ToString()] = Util.DungenEntranceMaxValue(dungeonType);
                        Debug.Log(dungeonType + "키 최대로 채워졌습니다.");
                    }
                }

                Managers.Backend.UpdateAllGameData(callback =>
                {
                    if (callback == null)
                    {
                        Debug.LogWarning("저장 데이터 미존재, 저장할 데이터가 존재하지 않습니다.");
                        return;
                    }

                    if (callback.IsSuccess())
                    {
                        Debug.Log("저장 성공, 저장에 성공했습니다.");
                    }
                    else
                    {
                        Debug.LogWarning($"수동 저장 실패, 수동 저장에 실패했습니다. {callback.ToString()}");
                    }

                });
            }
            else
            {
                Debug.Log("3초 미만임 업데이트 안함.");
            }
        }

        public void AddKey(EDungeonType dungeonType, int amount)
        {
            IsChangedData = true;
            string key = dungeonType.ToString();

            _dungeonkeyDic[key] += amount;
        }

        public void IncreaseDungeonLevel(EDungeonType dungeonType)
        {
            IsChangedData = true;
            string key = dungeonType.ToString();

            _dungeonLevelDic[key] += 1;
        }
    }
}
