using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

namespace BackendData.GameData
{
    // 일퀘 : 수행횃수, 도전중인지, 보상을 받을 상태인지, 보상을 받았는지 (잠금)
    // 반복퀘 : 수행횃수, 도전중인지, 보상을 받을 상태인지, 보상을 받았는지 (리셋) 
    // 업적 : 수행횃수, 도전 레벨, 보상을 받을 상태인지, 보상을 받았는지 (레벨*비율 -> 리셋)
    public class QuestInfoData
    {
        public int CurrentCount;      // 현재 진행도
        public int TargetCount;       // 목표 진행도
        public int CurrentLevel;      // 현재 업적 레벨 (업적 퀘스트에만 사용)
        public EQuestState QuestState;

        public QuestInfoData(int currentCount, int targetCount, int currentLevel, EQuestState questState)
        {
            CurrentCount = currentCount;
            TargetCount = targetCount;
            CurrentLevel = currentLevel;
            QuestState = questState;
        }

        public void CheckAndUpdateState()
        {
            CurrentCount += 1; // 진행도 증가
            if (CurrentCount >= TargetCount && QuestState == EQuestState.InProgress)
            {
                QuestState = EQuestState.ReadyToClaim; // 상태를 "보상 받을 상태"로 변경
            }
        }

    }

    public partial class QuestData
    {
        private Dictionary<string, QuestInfoData> _dailyQuestDic = new();       // 일일 퀘스트
        private Dictionary<string, QuestInfoData> _repeatableQuestDic = new();  // 반복 퀘스트
        private Dictionary<string, QuestInfoData> _achievementQuestDic = new(); // 업적 퀘스트 

        public IReadOnlyDictionary<string, QuestInfoData> DailyQuestDic => (IReadOnlyDictionary<string, QuestInfoData>)_dailyQuestDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, QuestInfoData> RepeatableQuestDic => (IReadOnlyDictionary<string, QuestInfoData>)_repeatableQuestDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, QuestInfoData> AchievementQuestDic => (IReadOnlyDictionary<string, QuestInfoData>)_achievementQuestDic.AsReadOnlyCollection();
    }

    public partial class QuestData : Base.GameData
    {
        protected override void InitializeData()
        {
            _dailyQuestDic.Clear();
            _repeatableQuestDic.Clear();
            _achievementQuestDic.Clear();
            foreach (var quest in Managers.Data.QuestChart)
            {
                var questCategory = quest.Value.QuestCategory;
                switch (questCategory)
                {
                    case EQuestCategory.Daily:
                        _dailyQuestDic.Add(quest.Value.QuestType.ToString(), new QuestInfoData(0, quest.Value.RequestCount, 0, EQuestState.InProgress));
                        break;
                    case EQuestCategory.Repeatable:
                        _repeatableQuestDic.Add(quest.Value.QuestType.ToString(), new QuestInfoData(0, quest.Value.RequestCount, 0, EQuestState.InProgress));
                        break;
                    case EQuestCategory.Achievement:
                        _achievementQuestDic.Add(quest.Value.QuestType.ToString(), new QuestInfoData(0, quest.Value.RequestCount, 1, EQuestState.InProgress));
                        break;
                }
            }
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data["AchievementQuest"].Keys)
            {
                int currentCount = int.Parse(Data["AchievementQuest"][column]["CurrentCount"].ToString());
                int targetCount = int.Parse(Data["AchievementQuest"][column]["TargetCount"].ToString());
                int currentLevel = int.Parse(Data["AchievementQuest"][column]["CurrentLevel"].ToString());
                EQuestState questState = Util.ParseEnum<EQuestState>(Data["AchievementQuest"][column]["QuestState"].ToString());

                _achievementQuestDic.Add(column, new QuestInfoData(currentCount, targetCount, currentLevel, questState));
            }

            foreach (var column in Data["RepeatableQuest"].Keys)
            {
                int currentCount = int.Parse(Data["RepeatableQuest"][column]["CurrentCount"].ToString());
                int targetCount = int.Parse(Data["RepeatableQuest"][column]["TargetCount"].ToString());
                int currentLevel = int.Parse(Data["RepeatableQuest"][column]["CurrentLevel"].ToString());
                EQuestState questState = Util.ParseEnum<EQuestState>(Data["RepeatableQuest"][column]["QuestState"].ToString());

                _repeatableQuestDic.Add(column, new QuestInfoData(currentCount, targetCount, currentLevel, questState));
            }

            foreach (var column in Data["DailyQuest"].Keys)
            {
                int currentCount = int.Parse(Data["DailyQuest"][column]["CurrentCount"].ToString());
                int targetCount = int.Parse(Data["DailyQuest"][column]["TargetCount"].ToString());
                int currentLevel = int.Parse(Data["DailyQuest"][column]["CurrentLevel"].ToString());
                EQuestState questState = Util.ParseEnum<EQuestState>(Data["DailyQuest"][column]["QuestState"].ToString());

                _dailyQuestDic.Add(column, new QuestInfoData(currentCount, targetCount, currentLevel, questState));
            }
        }

        public override string GetTableName()
        {
            return "QuestData";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("DailyQuest", _dailyQuestDic);
            param.Add("RepeatableQuest", _repeatableQuestDic);
            param.Add("AchievementQuest", _achievementQuestDic);
            return param;
        }

        public Dictionary<string, QuestInfoData> GetQuestCategory(EQuestCategory questCategory)
        {
            switch (questCategory)
            {
                case EQuestCategory.Daily:
                    return _dailyQuestDic;
                case EQuestCategory.Repeatable:
                    return _repeatableQuestDic;
                case EQuestCategory.Achievement:
                    return _achievementQuestDic;
            }

            return null;
        }

        // 플레이 시간 기록하는 함수
        public void CountPlayTime()
        {
            IsChangedData = true;
            _dailyQuestDic[EQuestType.PlayTime.ToString()].CheckAndUpdateState();
            _repeatableQuestDic[EQuestType.PlayTime.ToString()].CheckAndUpdateState();
        }

        // 광고 시청 퀘스트 
        public void CountWatchAd()
        {
            IsChangedData = true;
            _dailyQuestDic[EQuestType.WatchAds.ToString()].CheckAndUpdateState();
            _repeatableQuestDic[EQuestType.WatchAds.ToString()].CheckAndUpdateState();
            _achievementQuestDic[EQuestType.WatchAds.ToString()].CheckAndUpdateState();
        }

        public void CompleteQuest(Data.QuestData questData)
        {
            IsChangedData = true;
            switch (questData.QuestCategory)
            {
                case EQuestCategory.Daily:
                    if (_dailyQuestDic.TryGetValue(questData.QuestType.ToString(), out var dailyQuest))
                    {
                        dailyQuest.QuestState = EQuestState.Completed;
                    }
                    break;

                case EQuestCategory.Repeatable:
                    if (_repeatableQuestDic.TryGetValue(questData.QuestType.ToString(), out var repeatableQuest))
                    {
                        // 몇 배 보상을 받을지 계산
                        int multiplier = repeatableQuest.CurrentCount / repeatableQuest.TargetCount;

                        if (multiplier > 0)
                        {
                            // 보상 처리: multiplier에 따라 보상 지급
                            Debug.Log(multiplier);

                            // CurrentCount를 초과된 만큼만 남기기
                            repeatableQuest.CurrentCount %= repeatableQuest.TargetCount;

                            // 퀘스트 상태 갱신
                            repeatableQuest.QuestState = EQuestState.InProgress;
                        }
                    }
                    break;

                case EQuestCategory.Achievement:
                    if (_achievementQuestDic.TryGetValue(questData.QuestType.ToString(), out var achievementQuest))
                    {
                        achievementQuest.CurrentLevel += 1;
                        achievementQuest.TargetCount = achievementQuest.CurrentLevel * Managers.Data.QuestChart[questData.DataId].RequestCount;

                        // 상태 업데이트: 목표 진행도 도달 여부 확인
                        if (achievementQuest.CurrentCount >= achievementQuest.TargetCount)
                            achievementQuest.QuestState = EQuestState.ReadyToClaim;
                        else
                            achievementQuest.QuestState = EQuestState.InProgress;
                    }
                    break;
            }

        }
    }

}