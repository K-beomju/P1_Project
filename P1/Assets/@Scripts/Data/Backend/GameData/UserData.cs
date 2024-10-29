using System.Collections.Generic;
using LitJson;
using BackEnd;
using Unity.VisualScripting;
using static Define;
using System;

namespace BackendData.GameData
{

    //===============================================================
    // UserData 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class UserData
    {
        public int Level { get; private set; }
        public float Exp { get; private set; }
        public float MaxExp { get; private set; }

        // 현재 진행한 스테이지 정보 
        public int StageLevel { get; private set; }

        // 각 재화 담는 Dic
        private Dictionary<string, float> _purseDic = new();

        // 각 스탯레벨 담는 Dic
        private Dictionary<string, int> _upgradeStatDic = new();

        // 각 특성레벨 담는 Dic
        private Dictionary<string, int> _upgradeAttrDic = new();

        // 각 유물갯수 담는 Dic
        private Dictionary<string, int> _ownedRelicDic = new();


        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, float> PurseDic => (IReadOnlyDictionary<string, float>)_purseDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> UpgradeStatDic => (IReadOnlyDictionary<string, int>)_upgradeStatDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> UpgradeAttrDic => (IReadOnlyDictionary<string, int>)_upgradeAttrDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> OwnedRelicDic => (IReadOnlyDictionary<string, int>)_ownedRelicDic.AsReadOnlyCollection();

    }

    //===============================================================
    // UserData 테이블의 데이터를 담당하는 클래스(함수)
    //===============================================================
    public partial class UserData : Base.GameData
    {

        protected override void InitializeData()
        {
            StageLevel = 1;
            Level = 1;
            Exp = 0;
            MaxExp = Util.CalculateRequiredExp(Level);

            // 재화 정보 초기화 
            _purseDic.Clear();
            foreach (EGoodType goodType in Enum.GetValues(typeof(EGoodType)))
            {
                _purseDic.Add(goodType.ToString(), 0);
            }

            // 스탯 레벨 정보 초기화 
            _upgradeStatDic.Clear();
            foreach (EHeroUpgradeType upgradeType in Enum.GetValues(typeof(EHeroUpgradeType)))
            {
                _upgradeStatDic.Add(upgradeType.ToString(), 0);
            }

            // 특성 레벨 정보 초기화 
            _upgradeAttrDic.Clear();
            foreach (EHeroAttrType attrType in Enum.GetValues(typeof(EHeroAttrType)))
            {
                _upgradeAttrDic.Add(attrType.ToString(), 0);
            }

            // 유물 보유 정보 초기화 
            _ownedRelicDic.Clear();
            foreach (EHeroRelicType relicType in Enum.GetValues(typeof(EHeroRelicType)))
            {
                _ownedRelicDic.Add(relicType.ToString(), 0);
            }


        }

        // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        // 서버에서 데이터를 불러오는 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고
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

            foreach (var column in Data["UpgradeStat"].Keys)
            {
                _upgradeStatDic.Add(column, int.Parse(Data["UpgradeStat"][column].ToString()));
            }

            foreach (var column in Data["UpgradeAttr"].Keys)
            {
                _upgradeAttrDic.Add(column, int.Parse(Data["UpgradeAttr"][column].ToString()));
            }

            foreach (var column in Data["OwnedRelic"].Keys)
            {
                _ownedRelicDic.Add(column, int.Parse(Data["OwnedRelic"][column].ToString()));
            }

            AddAmount(EGoodType.ExpPoint, 1100);
            AddAmount(EGoodType.Dia, 1000000000);
            AddAmount(EGoodType.Gold, 185000);

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
            param.Add("Purse", _purseDic);
            param.Add("UpgradeStat", _upgradeStatDic);
            param.Add("UpgradeAttr", _upgradeAttrDic);
            param.Add("OwnedRelic", _ownedRelicDic);

            return param;
        }

        #region Good,Exp 

        // 유저의 재화를 변경하는 함수
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

            Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);
        }

        // 유저의 경험치를 변경하는 함수
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

        // 유저의 레벨을 변경하는 함수
        public void LevelUp()
        {
            // 레벨업 포인트 1씩 늘려줌 
            AddAmount(EGoodType.ExpPoint, 1);

            Exp -= MaxExp;

            MaxExp = Util.CalculateRequiredExp(Level);

            Level++;

            Managers.Event.TriggerEvent(EEventType.PlayerLevelUp, Level); // 레벨업 이벤트 발생
        }

        #endregion

        #region Stage 

        // 유저의 스테이지 정보를 변경하는 함수
        public void UpdateStageLevel(int stageLevel)
        {
            IsChangedData = true;
            StageLevel += stageLevel;
            if (StageLevel == 0)
                StageLevel = 1;
        }

        #endregion

        #region Upgrade
        public void LevelUpHeroUpgrade(EHeroUpgradeType upgradeType)
        {
            string key = upgradeType.ToString();
            if (_upgradeStatDic.ContainsKey(key))
            {
                _upgradeStatDic[key]++;
            }
            else
            {
                _upgradeStatDic.Add(key, 1);
            }

            Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
            Managers.Event.TriggerEvent(EEventType.HeroUpgradeUpdated);
        }

        public void LevelUpHeroAttribute(EHeroAttrType attrType)
        {
            string key = attrType.ToString();
            if (_upgradeAttrDic.ContainsKey(key))
            {
                _upgradeAttrDic[key]++;
            }
            else
            {
                _upgradeAttrDic.Add(key, 1);
            }
            Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
            Managers.Event.TriggerEvent(EEventType.HeroAttributeUpdated);
        }

        #endregion


    }

}


