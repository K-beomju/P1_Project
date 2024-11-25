using System.Collections.Generic;
using LitJson;
using BackEnd;
using Unity.VisualScripting;
using static Define;
using System;
using UnityEngine;
using System.Globalization;

namespace BackendData.GameData
{

    //===============================================================
    // CharacterData 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class CharacterData
    {
        public int Level { get; private set; }
        public float Exp { get; private set; }
        public float MaxExp { get; private set; }

        // 출석체크 
        public int AttendanceIndex { get; private set; }
        public string AttendanceLastLoginTime { get; private set; }
        public DateTime AttendanceUpdateTime { get; private set; }

        // 현재 진행한 스테이지 정보 
        public int StageLevel { get; private set; }

        // 월드 보스 전투력 
        public float WorldBossCombatPower { get; private set; }


        // 각 재화 담는 Dic
        private Dictionary<string, float> _purseDic = new();

        // 각 스탯레벨 담는 Dic
        private Dictionary<string, int> _upgradeStatDic = new();

        // 각 특성레벨 담는 Dic
        private Dictionary<string, int> _upgradeAttrDic = new();

        // 각 유물갯수 담는 Dic
        private Dictionary<string, int> _ownedRelicDic = new();

        // 버프 아이템 가능 횟수를 담는 Dic
        public Dictionary<string, int> _adBuffDic = new();


        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, float> PurseDic => (IReadOnlyDictionary<string, float>)_purseDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> UpgradeStatDic => (IReadOnlyDictionary<string, int>)_upgradeStatDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> UpgradeAttrDic => (IReadOnlyDictionary<string, int>)_upgradeAttrDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> OwnedRelicDic => (IReadOnlyDictionary<string, int>)_ownedRelicDic.AsReadOnlyCollection();
        public IReadOnlyDictionary<string, int> AdBuffDic => (IReadOnlyDictionary<string, int>)_adBuffDic.AsReadOnlyCollection();

    }

    //===============================================================
    // CharacterData 테이블의 데이터를 담당하는 클래스(함수)
    //===============================================================
    public partial class CharacterData : Base.GameData
    {

        protected override void InitializeData()
        {
            StageLevel = 1;
            Level = 1;
            Exp = 0;
            MaxExp = Util.CalculateRequiredExp(Level);
            AttendanceIndex = 1;
            AttendanceLastLoginTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            AttendanceUpdateTime = DateTime.MinValue;
            WorldBossCombatPower = 0;

            // 재화 정보 초기화 
            _purseDic.Clear();
            foreach (EItemType goodType in Enum.GetValues(typeof(EItemType)))
            {
                _purseDic.Add(goodType.ToString(), 0);
            }

            // 스탯 레벨 정보 초기화 
            _upgradeStatDic.Clear();
            foreach (EHeroUpgradeType upgradeType in Enum.GetValues(typeof(EHeroUpgradeType)))
            {
                _upgradeStatDic.Add(upgradeType.ToString(), 1);
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

            // 광고 버프 아이템 초기화
            _adBuffDic.Clear();
            foreach (EAdBuffType buffType in Enum.GetValues(typeof(EAdBuffType)))
            {
                _adBuffDic.Add(buffType.ToString(), 1);
            }


        }

        // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        // 서버에서 데이터를 불러오는 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고
        protected override void SetServerDataToLocal(JsonData Data)
        {
            Level = int.Parse(Data["Level"].ToString());
            Exp = float.Parse(Data["Exp"].ToString());
            MaxExp = float.Parse(Data["MaxExp"].ToString());
            AttendanceIndex = int.Parse(Data["AttendanceIndex"].ToString());
            AttendanceLastLoginTime = Data["AttendanceLastLoginTime"].ToString();
            StageLevel = int.Parse(Data["StageLevel"].ToString());
            WorldBossCombatPower = float.Parse(Data["WorldBossCombatPower"].ToString());

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

            foreach (var column in Data["AdBuff"].Keys)
            {
                _adBuffDic.Add(column, int.Parse(Data["AdBuff"][column].ToString()));
            }


            // AddAmount(EItemType.ExpPoint, 1100);
            // AddAmount(EItemType.Dia, 1000000000);
            // AddAmount(EItemType.Gold, 185000);
            //AddAmount(EItemType.AbilityPoint, 185000);
        }

        public override string GetTableName()
        {
            return "CharacterData";
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
            param.Add("AttendanceIndex", AttendanceIndex);
            param.Add("AttendanceLastLoginTime", AttendanceLastLoginTime);

            param.Add("WorldBossCombatPower", WorldBossCombatPower);
            param.Add("Purse", _purseDic);
            param.Add("UpgradeStat", _upgradeStatDic);
            param.Add("UpgradeAttr", _upgradeAttrDic);
            param.Add("OwnedRelic", _ownedRelicDic);
            param.Add("AdBuff", _adBuffDic);

            return param;
        }

        #region Good,Exp 

        // 유저의 재화를 변경하는 함수
        public void AddAmount(EItemType goodType, float amount)
        {
            IsChangedData = true;
            string key = goodType.ToString();

            // 골드 증가율 적용 (만약 재화가 골드인 경우)
            if (goodType == EItemType.Gold)
                amount = (int)(amount * Managers.Hero.PlayerHeroInfo.GoldIncreaseRate);

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

            exp = (int)(exp * Managers.Hero.PlayerHeroInfo.ExpIncreaseRate);

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
            AddAmount(EItemType.ExpPoint, 1);

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
            IsChangedData = true;
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
            IsChangedData = true;
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

        #region Relic
        public void AddRelic(EHeroRelicType relicType, int count)
        {
            IsChangedData = true;
            if (_ownedRelicDic.ContainsKey(relicType.ToString()))
            {
                _ownedRelicDic[relicType.ToString()] += count;
            }
            else
            {
                Debug.LogWarning(relicType);
            }
            Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
            Managers.Event.TriggerEvent(EEventType.HeroRelicUpdated);

        }
        #endregion

        // 월드보스전 갱신 
        public void UpdateWorldBossCombatPower(float power)
        {
            IsChangedData = true;
            WorldBossCombatPower = power;
            Managers.Backend.UpdateRankScore();
        }

        // 스크롤 광고 버프
        public void UsedBuff(EAdBuffType buffType)
        {
            IsChangedData = true;
            _adBuffDic[buffType.ToString()] += 1;
        }

        #region Attendance 
        public bool AttendanceCheck()
        {
            TimeSpan timeSinceLastLogin = DateTime.UtcNow - DateTime.Parse(AttendanceLastLoginTime);
            Debug.Log($"경과 시간: {timeSinceLastLogin.TotalHours:F2}시간");
            if (timeSinceLastLogin.TotalDays > 1)
            {
                return true;
            }

            return false;
        }

        // 출석체크 받음
        public void AttendanceReceive()
        {
            IsChangedData = true;

            // 실제 보상일차 계산 (초기화 또는 증가 이전 값 사용)
            int rewardDay = AttendanceIndex > 20 ? 1 : AttendanceIndex;

            // 출석 인덱스를 증가시키고, 20일 초과 시 다시 1로 초기화
            if (AttendanceIndex > 20)
            {
                AttendanceIndex = 2;
                Debug.Log("20일치를 모두 받았으므로 출석 인덱스를 초기화합니다.");
            }
            else
            {
                AttendanceIndex += 1;
            }


            // 출석체크 보상 로직
            Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification($"다이아 {rewardDay * 100}개 획득");
            AddAmount(EItemType.Dia, rewardDay * 100);
            Debug.Log($"{rewardDay}일차 보상을 받았습니다.");

            // 현재 시간을 출석 마지막 로그인 시간으로 기록
            AttendanceLastLoginTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }
        #endregion
    }
}


