using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
                Managers.Event.TriggerEvent(EEventType.QuestCheckNotification);
            }
        }

    }

    public partial class QuestData
    {
        // 일일 퀘스트 리셋 타임
        public string DailyQuestResetTime { get; private set; }

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
            DailyQuestResetTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

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
            DailyQuestResetTime = Data["DailyQuestResetTime"].ToString();

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

            DailyQuestTimeCheck();
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
            param.Add("DailyQuestResetTime", DailyQuestResetTime);
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

        private void DailyQuestTimeCheck()
        {
            TimeSpan timeSinceLastLogin = DateTime.UtcNow - DateTime.Parse(DailyQuestResetTime);
            Debug.Log($"경과 시간: {timeSinceLastLogin.TotalHours:F2}시간");
            Debug.Log($"현재 DailyQuestResetTime: {DailyQuestResetTime}");

            // 하루가 지나면 초기화 해줘야함
            if (timeSinceLastLogin.TotalDays >= 1)
            {
                _dailyQuestDic.Clear();
                foreach (var quest in Managers.Data.QuestChart)
                {
                    if(quest.Value.QuestCategory == EQuestCategory.Daily)
                    _dailyQuestDic.Add(quest.Value.QuestType.ToString(), new QuestInfoData(0, quest.Value.RequestCount, 0, EQuestState.InProgress));
                }
                DailyQuestResetTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            }

            Debug.Log("출석 체크 실패");
        }

        public void UpdateQuest(EQuestType questType)
        {
            IsChangedData = true;

            // 모든 퀘스트 딕셔너리를 순회하며 해당 퀘스트 타입의 상태를 업데이트
            if (_dailyQuestDic.ContainsKey(questType.ToString()))
                _dailyQuestDic[questType.ToString()].CheckAndUpdateState();

            if (_repeatableQuestDic.ContainsKey(questType.ToString()))
                _repeatableQuestDic[questType.ToString()].CheckAndUpdateState();

            if (_achievementQuestDic.ContainsKey(questType.ToString()))
                _achievementQuestDic[questType.ToString()].CheckAndUpdateState();
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

                        // 일퀘 완료 증가
                        _dailyQuestDic[EQuestType.CompleteDailyQuest.ToString()].CheckAndUpdateState();
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

        public bool IsReadyToClaimQuest()
        {
            foreach (var item in _dailyQuestDic.Values)
            {
                if (item.QuestState == EQuestState.ReadyToClaim)
                    return true;
            }
            foreach (var item in _repeatableQuestDic.Values)
            {
                if (item.QuestState == EQuestState.ReadyToClaim)
                    return true;
            }
            foreach (var item in _achievementQuestDic.Values)
            {
                if (item.QuestState == EQuestState.ReadyToClaim)
                    return true;
            }

            return false;
        }
    }

}
