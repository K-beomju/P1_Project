using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using System;
using Unity.VisualScripting;

namespace BackendData.GameData
{
    public class AbilityData
    {
        public ERankState RankState { get; set; }                  // 랭크 상태 (잠김, 진행 중, 완료 등)
        public ERankAbilityState RankAbilityState { get; set; }    // 어빌리티 슬롯 상태 (잠김, 해제, 임의 잠금 등)

        public AbilityData(ERankState rankState, ERankAbilityState abilityState)
        {
            RankState = rankState;
            RankAbilityState = abilityState;
        }
    }

    public class RankUpData : Base.GameData
    {
        // 랭크와 랭크 상태 타입
        private Dictionary<string, AbilityData> _rankUpDic = new();

        public IReadOnlyDictionary<string, AbilityData> RankUpDic => (IReadOnlyDictionary<string, AbilityData>)_rankUpDic.AsReadOnlyCollection();

        protected override void InitializeData()
        {
            _rankUpDic.Clear();
            foreach (ERankType rankType in Enum.GetValues(typeof(ERankType)))
            {
                if (rankType == ERankType.Iron)
                    _rankUpDic.Add(rankType.ToString(), new AbilityData(ERankState.Pending, ERankAbilityState.Locked));
                else
                    _rankUpDic.Add(rankType.ToString(), new AbilityData(ERankState.Locked, ERankAbilityState.Locked));
            }
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["Rank"].Keys)
            {
                ERankState rankState = (ERankState)int.Parse(Data["Rank"][column]["RankState"].ToString());
                ERankAbilityState rankAbilityState = (ERankAbilityState)int.Parse(Data["Rank"][column]["RankAbilityState"].ToString());

                _rankUpDic.Add(column, new AbilityData(rankState, rankAbilityState));
            }
        }

        public override string GetTableName()
        {
            return "RankUpData";
        }

        public override string GetColumnName()
        {
            return null;

        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("Rank", _rankUpDic);

            return param;
        }
        public void UpdateRankUp(ERankType rankType)
        {
            IsChangedData = true;

            // 현재 랭크를 'Current' 상태로 설정, 
            // 랭크 능력 슬롯을 'Unlocked' 상태로 설정
            _rankUpDic[rankType.ToString()].RankState = ERankState.Current;
            _rankUpDic[rankType.ToString()].RankAbilityState = ERankAbilityState.Unlocked;

            // 이전 랭크를 'Completed' 상태로 설정
            ERankType[] rankTypes = (ERankType[])Enum.GetValues(typeof(ERankType));
            int currentIndex = Array.IndexOf(rankTypes, rankType);

            if (currentIndex > 0)
            {
                ERankType previousRank = rankTypes[currentIndex - 1];
                _rankUpDic[previousRank.ToString()].RankState = ERankState.Completed;
                Debug.Log($"{previousRank} 랭크의 상태를 'Completed'으로 설정");
            }

            // 다음 랭크를 'Pending' 상태로 설정
            if (currentIndex >= 0 && currentIndex < rankTypes.Length - 1)
            {
                ERankType nextRank = rankTypes[currentIndex + 1];
                _rankUpDic[nextRank.ToString()].RankState = ERankState.Pending;
                Debug.Log($"{nextRank} 랭크의 상태를 'Pending'으로 설정");
            }
            else if (currentIndex == rankTypes.Length - 1)
            {
                // 마지막 랭크일 경우 완료 처리
                Debug.Log($"{rankType}는 마지막 랭크입니다. 모든 랭크가 완료되었습니다.");
            }

            Managers.Event.TriggerEvent(EEventType.HeroRankUpdated);
        }

        // 현재 'Current' 상태인 랭크 타입 반환
        public ERankType GetCurrentRankType()
        {
            foreach (var kvp in _rankUpDic)
            {
                if (kvp.Value.RankState == ERankState.Current)
                {
                    if (Enum.TryParse(kvp.Key, out ERankType rankType))
                        return rankType;
                }
            }
            return ERankType.Unknown;
        }
    }
}
