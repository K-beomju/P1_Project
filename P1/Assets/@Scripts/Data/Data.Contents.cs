using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
    #region CreatureData
    [Serializable]
    public class CreatureInfoData
    {
        public int DataId;
        public string Name;
        public EObjectType ObjectType;
        public string PrefabKey;
        //public float Atk;
        //public float Def;
        public float MaxHp;
        // public float AttackRange;
        // public float AttackDelay;
        // public float AttackSpeedRate;
    }
    [Serializable]
    public class CreatureUpgradeStatInfoData
    {
        public int DataID;
        public string Name;
        // public float IncreaseAtk;
        // public float IncreaseDef;
        public float IncreaseMaxHp;
    }
    [Serializable]
    public class CreatureUpgradeStatInfoDataLoader : ILoader<int, CreatureUpgradeStatInfoData>
    {
        public List<CreatureUpgradeStatInfoData> CreatureUpgradeStatInfoDataList = new List<CreatureUpgradeStatInfoData>();

        public Dictionary<int, CreatureUpgradeStatInfoData> MakeDict()
        {
            Dictionary<int, CreatureUpgradeStatInfoData> dict = new Dictionary<int, CreatureUpgradeStatInfoData>();
            foreach (CreatureUpgradeStatInfoData infoData in CreatureUpgradeStatInfoDataList)
            {
                dict.Add(infoData.DataID, infoData);
            }

            return dict;
        }
    }

    
    #region HeroData
    [Serializable]
    public class HeroInfoData : CreatureInfoData
    {
        public float Atk;
        public float AttackRange;
        public float AttackDelay;
        public float AttackSpeedRate;
    }

    [Serializable]
    public class HeroInfoDataLoader : ILoader<int, HeroInfoData>
    {
        public List<HeroInfoData> HeroInfoDatas = new List<HeroInfoData>();

        public Dictionary<int, HeroInfoData> MakeDict()
        {
            Dictionary<int, HeroInfoData> dict = new Dictionary<int, HeroInfoData>();
            foreach (HeroInfoData infoData in HeroInfoDatas)
            {
                dict.Add(infoData.DataId, infoData);
            }

            return dict;
        }
    }
    #endregion

    #region MonsterData
    [Serializable]
    public class MonsterInfoData : CreatureInfoData
    {
        public float MoveSpeed;
    }

    [Serializable]
    public class MonsterInfoDataLoader : ILoader<int, MonsterInfoData>
    {
        public List<MonsterInfoData> monsters = new List<MonsterInfoData>();
        public Dictionary<int, MonsterInfoData> MakeDict()
        {
            Dictionary<int, MonsterInfoData> dict = new Dictionary<int, MonsterInfoData>();
            foreach (MonsterInfoData monster in monsters)
                dict.Add(monster.DataId, monster);
            return dict;
        }
    }

    #region BossMonsterData
    [Serializable]
    public class BossMonsterInfoData : MonsterInfoData
    {
    }

    [Serializable]
    public class BossMonsterInfoDataLoader : ILoader<int, BossMonsterInfoData>
    {
        public List<BossMonsterInfoData> BossMonsterInfoDataList = new List<BossMonsterInfoData>();

        public Dictionary<int, BossMonsterInfoData> MakeDict()
        {
            Dictionary<int, BossMonsterInfoData> dict = new Dictionary<int, BossMonsterInfoData>();
            foreach (BossMonsterInfoData infoData in BossMonsterInfoDataList)
            {
                dict.Add(infoData.DataId, infoData);
            }

            return dict;
        }
    }
    #endregion

    #endregion
    #endregion


    #region StageData
    [Serializable]
    public class StageInfoData
    {
        public int StageNumber;
        public EStageType StageType;
        public int MonsterCount;

        public bool BossStage;
        public int BossDataId;
        public int BossBattleTimeLimit;

        public List<int> MonsterDataIdList = new List<int>();
        public int MonsterLevel;
        public int MonsterGoldReward;
    }

    [Serializable]
    public class StageInfoDataLoader : ILoader<int, StageInfoData>
    {
        public List<StageInfoData> stages = new List<StageInfoData>();
        public Dictionary<int, StageInfoData> MakeDict()
        {
            Dictionary<int, StageInfoData> dict = new Dictionary<int, StageInfoData>();
            foreach (StageInfoData stage in stages)
                dict.Add(stage.StageNumber, stage);
            return dict;
        }
    }
    #endregion

    #region HeroUpgradeData
    [Serializable]
    public class HeroUpgradeInfoData
    { 
        public EHeroUpgradeType HeroUpgradeType;
        public string Remark;
        public float Value;
    }

    [Serializable]
    public class HeroUpgradeInfoDataLoader : ILoader<EHeroUpgradeType, HeroUpgradeInfoData>
    {
        public List<HeroUpgradeInfoData> HeroUpgradeInfoDataList = new List<HeroUpgradeInfoData>();

        public Dictionary<EHeroUpgradeType, HeroUpgradeInfoData> MakeDict()
        {
            Dictionary<EHeroUpgradeType, HeroUpgradeInfoData> dict = new Dictionary<EHeroUpgradeType, HeroUpgradeInfoData>();
            foreach (HeroUpgradeInfoData infoData in HeroUpgradeInfoDataList)
            {
                dict.Add(infoData.HeroUpgradeType, infoData);
            }

            return dict;
        }
    }

    [Serializable]
    public class HeroUpgradeCostInfoData
    {
        public EHeroUpgradeType HeroUpgradeType;
        public string Remark;
        public List<int> ReferenceLevelList = new List<int>();
        public List<EGoodType> GoodList = new List<EGoodType>();
        public List<int>StartCostList = new List<int>();
        public List<int>IncreaseCostList = new List<int>();
        public List<float> SuccessPercentageList = new List<float>();
    }

    [Serializable]
    public class HeroUpgradeCostInfoDataLoader : ILoader<EHeroUpgradeType, HeroUpgradeCostInfoData>
    {
        public List<HeroUpgradeCostInfoData> HeroUpgradeCostInfoDataList = new List<HeroUpgradeCostInfoData>();

        public Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData> MakeDict()
        {
            Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData> dict = new Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData>();
            foreach (HeroUpgradeCostInfoData infoData in HeroUpgradeCostInfoDataList)
            {
                dict.Add(infoData.HeroUpgradeType, infoData);
            }

            return dict;
        }
    }
    #endregion


}
