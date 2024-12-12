using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using System;
using System.Globalization;
using LitJson;
using BackEnd;

namespace BackendData.GameData
{
    public class RewardAdData
    {
        public int WatchedCount;  // 광고 시청 횟수
        public int MaxCount = 3; // 최대 시청 횟수

        public RewardAdData(int watchCount, int maxCount)
        {
            WatchedCount = watchCount;
            MaxCount = maxCount;
        }
    }

    public partial class ShopData
    {
        private Dictionary<string, RewardAdData> _rewardAdDic = new();

        public IReadOnlyDictionary<string, RewardAdData> RewardAdDic => _rewardAdDic;

        // 마지막 리셋 시간 저장 
        public string LastResetTime { get; private set; }
    }

    public partial class ShopData : Base.GameData
    {
        protected override void InitializeData()
        {
            _rewardAdDic.Clear();
            foreach (EAdRewardType rewardType in Enum.GetValues(typeof(EAdRewardType)))
            {
                // 전투 효과는 2번만, 나머진 3번
                if(rewardType == EAdRewardType.AtkBuff || rewardType == EAdRewardType.IncreaseGoldBuff || rewardType == EAdRewardType.IncreaseExpBuff)
                _rewardAdDic[rewardType.ToString()] = new RewardAdData(2, 2);
                else
                _rewardAdDic[rewardType.ToString()] = new RewardAdData(3, 3);

            }

             // 마지막 리셋 시간 초기화
            LastResetTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["RewardAd"].Keys)
            {
                int watchCount = int.Parse(Data["RewardAd"][column]["WatchedCount"].ToString());
                int maxCount = int.Parse(Data["RewardAd"][column]["MaxCount"].ToString());
                _rewardAdDic.Add(column, new RewardAdData(watchCount, maxCount));
            }
            
            LastResetTime = Data["LastResetTime"].ToString();

            CheckReset();
        }

        public override string GetTableName()
        {
            return "ShopData";
        }


        public override string GetColumnName()
        {
            return null;

        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("RewardAd", _rewardAdDic);
            param.Add("LastResetTime", LastResetTime);
            return param;
        }

        private void CheckReset()
        {
            TimeSpan lastReset = DateTime.UtcNow - DateTime.Parse(LastResetTime);
            if(lastReset.TotalDays >= 1)
            {
                ResetAdData();
            }
        }

        private void ResetAdData()
        {
            IsChangedData = true;
            foreach(var rewardAdData in _rewardAdDic.Values)
            {
                rewardAdData.WatchedCount = rewardAdData.MaxCount;
            }
            LastResetTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Debug.Log("하루가 경과하여 광고 데이터를 초기화했습니다.");
        }

        public bool WatchAd(EAdRewardType rewardType)
        {
            IsChangedData = true;
            var rewardAdData = _rewardAdDic[rewardType.ToString()];

            // 광고 시청 처리
            rewardAdData.WatchedCount--;
            Debug.Log($"{rewardType} 광고 시청 완료. 현재 시청 횟수: {rewardAdData.WatchedCount}/{rewardAdData.MaxCount}");
            return true;
        }

        public bool IsCheckWatch(EAdRewardType rewardType)
        {
            if(_rewardAdDic[rewardType.ToString()].WatchedCount != 0)
            {
                return true;
            }

            return false;
        }
    }
}
