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
        public int RemainChargeHour { get; private set; }
        
        public DateTime UpdateTime { get; private set; }
        public IReadOnlyDictionary<string, int> DungeonLevelDic => (IReadOnlyDictionary<string, int>)_dungeonLevelDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> DungeonKeyDic => (IReadOnlyDictionary<string, int>)_dungeonkeyDic.AsReadOnlyCollection();

        protected override void InitializeData()
        {
            // 던전 정보 초기화 
            _dungeonLevelDic.Clear();
            _dungeonkeyDic.Clear();

            foreach (EDungeonType dungeonType in Enum.GetValues(typeof(EDungeonType)))
            {
                if (dungeonType == EDungeonType.Unknown) // Unknown 제외
                    continue;

                _dungeonLevelDic.Add(dungeonType.ToString(), 1);
                _dungeonkeyDic.Add(dungeonType.ToString(), Util.DungenEntranceMaxValue(dungeonType));

            }

            BackendReturnObject servertime = Backend.Utils.GetServerTime();

            string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
            LastLoginTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
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
            // if ((DateTime.UtcNow - UpdateTime).Hours < 1) {
            //     Debug.Log("아직 1시간이 지나지 않았습니다.");
            //     return;
            // }

            // 갱신 주기 갱신
            UpdateTime = DateTime.UtcNow;
            
            BackendReturnObject serverTime = Backend.Utils.GetServerTime();
            if (!serverTime.IsSuccess())
            {
                Debug.LogError("Failed to get server time.");
                return;
            }

            // 서버 시간과 마지막 로그인 시간을 비교하여 시간 차이를 계산
            TimeSpan timeDifference = DateTime.UtcNow - DateTime.Parse(LastLoginTime);

            // `RemainChargeHour` 설정: 남은 충전 시간을 계산
            RemainChargeHour = Mathf.Clamp(12 - (int)timeDifference.TotalHours, 1, 12);


            Debug.LogWarning($"지난 로그인 후 경과 시간: {timeDifference.Days}일 {timeDifference.Hours}시간"
            + $"{timeDifference.Minutes}분 {timeDifference.Seconds}초");

            // 충전까지 12시간 미만 경과했을 경우, 충전 프로세스 종료
            if (timeDifference.TotalHours < 12)
            {
                Debug.Log("12시간 미만 경과 - 키 충전 미실행");
                return;
            }

            RemainChargeHour = 12;
            // 충전 완료: 키 리필 및 마지막 로그인 시간 갱신
            Debug.Log("12시간 이상 경과: 키 리필 및 마지막 로그인 시간 업데이트.");
            IsChangedData = true;
            LastLoginTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            
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
            
            // 던전 데이터만 저장 
            Managers.Backend.UpdateSingleGameData(this, callback =>
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
