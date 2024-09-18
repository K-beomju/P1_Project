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

    #region StatData
    [Serializable]
    public class StatData
    {
        public int Level;
        public int IncreaseStat;
        public int Price;
    }

    [Serializable]
    public class AttackLevelData : StatData
    {
    }

    [Serializable]
    public class AttackLevelDataLoader : ILoader<int, AttackLevelData>
    {
        public List<AttackLevelData> attackLevelDatas = new List<AttackLevelData>();

        public Dictionary<int, AttackLevelData> MakeDict()
        {
            Dictionary<int, AttackLevelData> dict = new Dictionary<int, AttackLevelData>();
            foreach (AttackLevelData infoData in attackLevelDatas)
            {
                dict.Add(infoData.Level, infoData);
            }

            return dict;
        }
    }

    [Serializable]
    public class HpLevelData : StatData
    {
    }

    [Serializable]
    public class HpLevelDataLoader : ILoader<int, HpLevelData>
    {
        public List<HpLevelData> hpLevelDatas = new List<HpLevelData>();

        public Dictionary<int, HpLevelData> MakeDict()
        {
            Dictionary<int, HpLevelData> dict = new Dictionary<int, HpLevelData>();
            foreach (HpLevelData infoData in hpLevelDatas)
            {
                dict.Add(infoData.Level, infoData);
            }

            return dict;
        }
    }
    #endregion


}
