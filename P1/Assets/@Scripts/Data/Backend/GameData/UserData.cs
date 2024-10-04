using System.Collections.Generic;
using LitJson;
using BackEnd;
using Unity.VisualScripting;
using static Define;

namespace BackendData.GameData
{

    public partial class UserData
    {
        public int Level { get; private set; }
        public float Exp { get; private set; }
        public float MaxExp { get; private set; }

        // 현재 진행한 스테이지 정보 
        public int StageLevel { get; private set; }

        // Purse 각 재화를 담는 Dictionary
        private Dictionary<string, float> _purseDic = new();
        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, float> PurseDic => (IReadOnlyDictionary<string, float>)_purseDic.AsReadOnlyCollection();

    }

    public partial class UserData : Base.GameData
    {

        protected override void InitializeData()
        {
            Level = 1;
            Exp = 0;
            MaxExp = Util.CalculateRequiredExp(Level);
            StageLevel = 1;
            _purseDic.Clear();
            _purseDic.Add("Gold", 0);
            _purseDic.Add("Dia", 0);
        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            Level = int.Parse(Data["Level"].ToString());
            Exp = float.Parse(Data["Exp"].ToString());
            MaxExp = float.Parse(Data["MaxExp"].ToString());
            StageLevel = int.Parse(Data["StageLevel"].ToString());
            foreach (var column in Data["Purse"].Keys)
            {
                _purseDic.Add(column, float.Parse(Data["Purse"][column].ToString()));
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

            param.Add("Level", Level);
            param.Add("Exp", Exp);
            param.Add("MaxExp", MaxExp);
            param.Add("StageLevel", StageLevel);
            param.Add("Purse", PurseDic);

            return param;
        }


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

        public void AddExp(int exp)
        {
            IsChangedData = true;
            Exp += exp;

            // 레벨업 처리
            while (Exp >= MaxExp)
            {
                LevelUp();
            }

            Managers.Event.TriggerEvent(EEventType.ExperienceUpdated, Level, Exp, MaxExp); // 경험치 갱신 이벤트
        }

        public void LevelUp()
        {
            Exp -= MaxExp;

            MaxExp = Util.CalculateRequiredExp(Level);

            Level++;

            Managers.Event.TriggerEvent(EEventType.PlayerLevelUp, Level); // 레벨업 이벤트 발생
        }

        public void UpdateStageLevel(int stageLevel)
        {
            IsChangedData = true;
            StageLevel += stageLevel;
            if (StageLevel == 0)
                StageLevel = 1;
        }

    }

}


