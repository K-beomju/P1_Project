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
        public float Atk;
        public float MaxHp;
    }

    [Serializable]
    public class CreatureUpgradeStatInfoData
    {
        public int DataID;
        public string Name;
        // public float IncreaseDef;
        public float IncreaseAtk;
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
        public float Recovery;
        public float CriRate;
        public float CriDmg;
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
        public int KillMonsterCount;

        public int BossDataId;
        public int BossBattleTimeLimit;

        public List<int> MonsterDataIdList = new List<int>();
        public int MonsterLevel;
        public int MonsterGoldReward;
        public int MonsterExpReward;
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
        public float IncreaseValue;
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
        public List<int> StartCostList = new List<int>();
        public List<int> IncreaseCostList = new List<int>();
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

    #region DrawData
    [Serializable]
    public class DrawEquipmentGachaData
    {
        public int Level;
        public int MaxExp;

        public List<float> DrawProbability = new List<float>();
        public List<float> NormalDrawList = new List<float>();
        public List<float> AdvancedDrawList = new List<float>();
        public List<float> RareDrawList = new List<float>();
        public List<float> LegendaryDrawList = new List<float>();
        public List<float> MythicalDrawList = new List<float>();
        public List<float> CelestialDrawList = new List<float>();

        public List<int> NormalEqIdList = new List<int>();
        public List<int> AdvancedEqIdList = new List<int>();
        public List<int> RareEqIdList = new List<int>();
        public List<int> LegendaryEqIdList = new List<int>();
        public List<int> MythicalEqIdList = new List<int>();
        public List<int> CelestialEqIdList = new List<int>();

    }

    [Serializable]
    public class DrawEquipmentGachaDataLoader : ILoader<int, DrawEquipmentGachaData>
    {
        public List<DrawEquipmentGachaData> draws = new List<DrawEquipmentGachaData>();
        public Dictionary<int, DrawEquipmentGachaData> MakeDict()
        {
            Dictionary<int, DrawEquipmentGachaData> dict = new Dictionary<int, DrawEquipmentGachaData>();
            foreach (DrawEquipmentGachaData draw in draws)
                dict.Add(draw.Level, draw);
            return dict;
        }
    }

    [Serializable]
    public class DrawSkillGachaData
    {
        public int Level;
        public int MaxExp;

        public List<float> DrawProbabilityList;
        public List<float> NormalDrawList;
        public List<float> AdvancedDrawList;
        public List<float> RareDrawList;
        public List<float> LegendaryDrawList;
        public List<float> MythicalDrawList;
        public List<float> CelestialDrawList;

        public List<int> NormalEqIdList;
        public List<int> AdvancedEqIdList;
        public List<int> RareEqIdList;
        public List<int> LegendaryEqIdList;
        public List<int> MythicalEqIdList;
        public List<int> CelestialEqIdList;

    }

    [Serializable]
    public class DrawSkillGachaDataLoader : ILoader<int, DrawSkillGachaData>
    {
        public List<DrawSkillGachaData> draws = new List<DrawSkillGachaData>();
        public Dictionary<int, DrawSkillGachaData> MakeDict()
        {
            Dictionary<int, DrawSkillGachaData> dict = new Dictionary<int, DrawSkillGachaData>();
            foreach (DrawSkillGachaData draw in draws)
                dict.Add(draw.Level, draw);
            return dict;
        }
    }
    #endregion

    #region ItemData (Equipment, Skill) 

    [Serializable]
    public class EquipmentData
    {
        public int DataId;
        public ERareType RareType;
        public EEquipmentType EquipmentType;
        public string SpriteKey;
        public string Name;
        public float OwnedValue;
        public float EquippedValue;
    }

    [Serializable]
    public class EquipmentDataLoader : ILoader<int, EquipmentData>
    {
        public List<EquipmentData> equipmentDatas = new List<EquipmentData>();
        public Dictionary<int, EquipmentData> MakeDict()
        {
            Dictionary<int, EquipmentData> dict = new Dictionary<int, EquipmentData>();
            foreach (EquipmentData equipment in equipmentDatas)
                dict.Add(equipment.DataId, equipment);
            return dict;
        }
    }

    [Serializable]
    public class SkillData
    {
        public int DataId;
        public ERareType RareType;

        public string Name;
        public string Description;
        public string ClassName;
        public string PrefabKey;
        public string SpriteKey;
        public string SoundKey;

        public float OwnedValue;
        public float DamageMultiplier;
        public float CoolTime;
        public int EffectId;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skillDatas = new List<SkillData>();
        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skillDatas)
                dict.Add(skill.DataId, skill);
            return dict;
        }
    }
    
    [Serializable]
    public class EffectData
    {
        public int DataId;
        public string Name;
        public string Description;
        public int Duration;
        public float TickTime;
        public EEffectType EffectType;
    }

    [Serializable]
    public class EffectDataLoader : ILoader<int, EffectData>
    {
        public List<EffectData> effectDatas = new List<EffectData>();
        public Dictionary<int, EffectData> MakeDict()
        {
            Dictionary<int, EffectData> dict = new Dictionary<int, EffectData>();
            foreach (EffectData effect in effectDatas)
                dict.Add(effect.DataId, effect);
            return dict;
        }
    }
    #endregion

    

    


}
