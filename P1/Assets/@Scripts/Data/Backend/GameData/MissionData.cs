using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;


namespace BackendData.GameData
{
    public class MissionInfoData
    {
        public int Id;
        public bool IsComplete { get; set; }
        public EMissionType MissionType { get; private set; }
        public int CompleteValue { get; private set; }

        public MissionInfoData(int id, bool isComplete, EMissionType missionType, int completeValue)
        {
            Id = id;
            IsComplete = isComplete;
            MissionType = missionType;
            CompleteValue = completeValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is not MissionInfoData other)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

    public partial class MissionData
    {
        private List<MissionInfoData> _missionList = new();
        private Dictionary<EMissionType, int> _missionDic = new();


        public IReadOnlyList<MissionInfoData> MissionList => (IReadOnlyList<MissionInfoData>)_missionList.AsReadOnlyList();
        public IReadOnlyDictionary<EMissionType, int> MissionDic => (IReadOnlyDictionary<EMissionType, int>)_missionDic.AsReadOnlyCollection();
    }

    public partial class MissionData : Base.GameData
    {
        protected override void InitializeData()
        {
            _missionList.Clear();
            _missionDic.Clear();

            for (int i = 1; i <= Managers.Data.MissionChart.Count; i++)
            {
                int id = Managers.Data.MissionChart[i].DataId;
                EMissionType missionType = Managers.Data.MissionChart[i].MissionType;
                int completeValue = Managers.Data.MissionChart[i].CompleteValue;
                MissionInfoData infoData = new MissionInfoData(id, false, missionType, completeValue);
                _missionList.Add(infoData);
            }

            foreach (string type in Enum.GetNames(typeof(EMissionType)))
            {
                EMissionType missionType = Util.ParseEnum<EMissionType>(type);
                _missionDic.Add(missionType, 0);
            }
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            _missionList.Clear();
            for (int i = 0; i < Managers.Data.MissionChart.Count; i++)
            {
                int id = int.Parse(Data["MissionList"][i]["Id"].ToString());
                bool isComplete = bool.Parse(Data["MissionList"][i]["IsComplete"].ToString());
                EMissionType missionType = (EMissionType)int.Parse(Data["MissionList"][i]["MissionType"].ToString());
                int completeValue = int.Parse(Data["MissionList"][i]["CompleteValue"].ToString());
                MissionInfoData infoData = new MissionInfoData(id, isComplete, missionType, completeValue);
                _missionList.Add(infoData);
            }

            foreach (var column in Data["MissionDic"].Keys)
            {
                _missionDic.Add(Util.ParseEnum<EMissionType>(column), int.Parse(Data["MissionDic"][column].ToString()));
            }
        }

        public override string GetTableName()
        {
            return "MissionData";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("MissionList", _missionList);
            param.Add("MissionDic", _missionDic);
            return param;
        }

        public void UpdateMission(EMissionType missionType)
        {
            IsChangedData = true;
            if (_missionDic.ContainsKey(missionType))
            {
                _missionDic[missionType] += 1;
                Managers.Event.TriggerEvent(EEventType.MissionItemUpdated);
            }
        }

        public void CompleteMission(MissionInfoData infoData)
        {
            var missionInfoData = FindMission(infoData);
            if (missionInfoData != null)
            {
                missionInfoData.IsComplete = true;
                IsChangedData = true;
                Debug.LogWarning($"{missionInfoData.MissionType} 미션 완료");

                if(missionInfoData.MissionType == EMissionType.StageChallenge)
                _missionDic[missionInfoData.MissionType] = 0;

                Managers.Event.TriggerEvent(EEventType.MissionCompleted);
            }
            else
            {
                Debug.LogWarning("해당 미션을 찾을 수 없습니다.");
            }

            
        }

        // 완료하지 않은 미션 찾기 
        public MissionInfoData GetCurrentMission()
        {
            MissionInfoData missionData = _missionList.FirstOrDefault(mission => !mission.IsComplete);

            if (missionData == null)
                return null;

            return missionData;
        }

        // 특정 미션 찾기
        public MissionInfoData FindMission(MissionInfoData infoData)
        {
            return _missionList.FirstOrDefault(m => m.Equals(infoData));
        }
    }

}
