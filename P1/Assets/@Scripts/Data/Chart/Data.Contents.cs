using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using System.Numerics; // BigInteger를 사용하기 위해 추가

namespace Data
{
    #region CreatureData
    [Serializable]
    public class CreatureInfoData
    {
        public int DataId;
        public string Name;
        public string PrefabKey;
    }

    #region HeroData
    [Serializable]
    public class HeroInfoData : CreatureInfoData
    {
        public float Atk;
        public float MaxHp;
        public float Recovery;
        public float CriRate;
        public float CriDmg;
        public float AttackRange;
        public float AttackDelay;
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

    #region RankUpMonsterData
    [Serializable]
    public class RankUpMonsterInfoData
    {
        public int DataId;
        public ERankType rankType;
        public string Name;
        public string PrefabKey;
        public float Atk;
        public float MaxHp;
        public float AttackRange;
    }

    [Serializable]
    public class RankUpMonsterInfoDataLoader : ILoader<int, RankUpMonsterInfoData>
    {
        public List<RankUpMonsterInfoData> rankUpMonsterDataInfoDatas = new List<RankUpMonsterInfoData>();

        public Dictionary<int, RankUpMonsterInfoData> MakeDict()
        {
            Dictionary<int, RankUpMonsterInfoData> dict = new Dictionary<int, RankUpMonsterInfoData>();
            foreach (RankUpMonsterInfoData infoData in rankUpMonsterDataInfoDatas)
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
        public double MonsterAtk;
        public double MonsterMaxHp;
        public double MonsterGoldReward;
        public double MonsterExpReward;
        public double BossMonsterAtk;
        public double BossMonsterMaxHp;
        public Dictionary<EItemType, int> RewardItem = new Dictionary<EItemType, int>();

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
        public List<EItemType> GoodList = new List<EItemType>();
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


    [Serializable]
    public class HeroAttributeInfoData
    {
        public EHeroAttrType HeroAttrType;
        public string Name;
        public float Value;
        public float IncreaseValue;
        public string SpriteKey;
    }

    [Serializable]
    public class HeroAttributeInfoDataLoader : ILoader<EHeroAttrType, HeroAttributeInfoData>
    {
        public List<HeroAttributeInfoData> HeroAttributeInfoDataList = new List<HeroAttributeInfoData>();

        public Dictionary<EHeroAttrType, HeroAttributeInfoData> MakeDict()
        {
            Dictionary<EHeroAttrType, HeroAttributeInfoData> dict = new Dictionary<EHeroAttrType, HeroAttributeInfoData>();
            foreach (HeroAttributeInfoData infoData in HeroAttributeInfoDataList)
            {
                dict.Add(infoData.HeroAttrType, infoData);
            }

            return dict;
        }
    }

    [Serializable]
    public class HeroAttributeCostInfoData
    {
        public EHeroAttrType HeroAttrType;
        public string Remark;
        public List<int> ReferenceLevelList = new List<int>();
        public List<EItemType> GoodList = new List<EItemType>();
        public List<int> StartCostList = new List<int>();
        public List<int> IncreaseCostList = new List<int>();
    }

    [Serializable]
    public class HeroAttributeCostInfoDataLoader : ILoader<EHeroAttrType, HeroAttributeCostInfoData>
    {
        public List<HeroAttributeCostInfoData> HeroAttributeCostInfoDataList = new List<HeroAttributeCostInfoData>();

        public Dictionary<EHeroAttrType, HeroAttributeCostInfoData> MakeDict()
        {
            Dictionary<EHeroAttrType, HeroAttributeCostInfoData> dict = new Dictionary<EHeroAttrType, HeroAttributeCostInfoData>();
            foreach (HeroAttributeCostInfoData infoData in HeroAttributeCostInfoDataList)
            {
                dict.Add(infoData.HeroAttrType, infoData);
            }

            return dict;
        }
    }

    [Serializable]
    public class RelicInfoData
    {
        public EHeroRelicType HeroRelicType;
        public string Name;
        public string Remark;
        public string SpriteKey;
        public float IncreaseValue;
        public int MaxCount;
    }

    [Serializable]
    public class RelicInfoDataLoader : ILoader<EHeroRelicType, RelicInfoData>
    {
        public List<RelicInfoData> HeroRelicInfoDatas = new List<RelicInfoData>();

        public Dictionary<EHeroRelicType, RelicInfoData> MakeDict()
        {
            Dictionary<EHeroRelicType, RelicInfoData> dict = new Dictionary<EHeroRelicType, RelicInfoData>();
            foreach (RelicInfoData infoData in HeroRelicInfoDatas)
            {
                dict.Add(infoData.HeroRelicType, infoData);
            }

            return dict;
        }
    }



    #endregion

    #region DrawData
    [Serializable]
    public class DrawGachaData
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
    public class DrawEquipmentGachaData : DrawGachaData
    {

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
    public class DrawSkillGachaData : DrawGachaData
    {

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
    public class ItemData
    {
        public EItemType ItemType;
        public string Remark;
        public string SpriteKey;
    }

    [Serializable]
    public class ItemDataLoader : ILoader<EItemType, ItemData>
    {
        public List<ItemData> itemDatas = new List<ItemData>();
        public Dictionary<EItemType, ItemData> MakeDict()
        {
            Dictionary<EItemType, ItemData> dict = new Dictionary<EItemType, ItemData>();
            foreach (ItemData item in itemDatas)
                dict.Add(item.ItemType, item);
            return dict;
        }
    }

    [Serializable]
    public class EquipmentData
    {
        public int DataId;
        public ERareType RareType;
        public EEquipmentType EquipmentType;
        public string SpriteKey;
        public string Name;
        public float OwnedValue;            // 보유효과 기본 수치
        public float OwnedIncreaseRate;     // 보유효과 증가율
        public float EquippedValue;         // 장착효과 기본 수치  
        public float EquippedIncreaseRate;  // 장착효과 증가율 
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
        public string PrefabKey;
        public string SpriteKey;
        public string SoundKey;

        public float OwnedValue;
        public float DamageMultiplier;
        public float CoolTime;
        public int SkillCount;
        public int EffectId;
        public bool AttachToOwner;
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
        public EEffectSpawnType EffectSpawnType;
        public string ProjectileKey;
        public string ExplosionKey;
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

    #region DungeonData
    [Serializable]
    public class DungeonInfoData
    {
        public int DungeonLevel;
        public int DungeonTimeLimit;
        public int KillMonsterCount;
        public List<int> MonsterDataIdList = new List<int>();
        public float MonsterAtk;
        public float MonsterMaxHp;
        public EItemType ItemType;
        public int DungeonClearReward;
    }

    [Serializable]
    public class GoldDungeonInfoData : DungeonInfoData
    {

    }

    [Serializable]
    public class GoldDungeonInfoDataLoader : ILoader<int, GoldDungeonInfoData>
    {
        public List<GoldDungeonInfoData> goldDungeonInfoDatas = new List<GoldDungeonInfoData>();
        public Dictionary<int, GoldDungeonInfoData> MakeDict()
        {
            Dictionary<int, GoldDungeonInfoData> dict = new Dictionary<int, GoldDungeonInfoData>();
            foreach (GoldDungeonInfoData dungeon in goldDungeonInfoDatas)
                dict.Add(dungeon.DungeonLevel, dungeon);
            return dict;
        }
    }

    [Serializable]
    public class DiaDungeonInfoData : DungeonInfoData
    {
    }

    [Serializable]
    public class DiaDungeonInfoDataLoader : ILoader<int, DiaDungeonInfoData>
    {
        public List<DiaDungeonInfoData> diaDungeonInfoDatas = new List<DiaDungeonInfoData>();
        public Dictionary<int, DiaDungeonInfoData> MakeDict()
        {
            Dictionary<int, DiaDungeonInfoData> dict = new Dictionary<int, DiaDungeonInfoData>();
            foreach (DiaDungeonInfoData dungeon in diaDungeonInfoDatas)
                dict.Add(dungeon.DungeonLevel, dungeon);
            return dict;
        }
    }

    [Serializable]
    public class WorldBossDungeonInfoData
    {
        public int DungeonLevel;
        public int DungeonTimeLimit;
        public string PrefabKey;
        public float BossAtk;
        public float BossMaxHp;
    }

    [Serializable]
    public class WorldBossDungeonInfoDataLoader : ILoader<int, WorldBossDungeonInfoData>
    {
        public List<WorldBossDungeonInfoData> worldBossDungeonInfoDatas = new List<WorldBossDungeonInfoData>();
        public Dictionary<int, WorldBossDungeonInfoData> MakeDict()
        {
            Dictionary<int, WorldBossDungeonInfoData> dict = new Dictionary<int, WorldBossDungeonInfoData>();
            foreach (WorldBossDungeonInfoData dungeon in worldBossDungeonInfoDatas)
                dict.Add(dungeon.DungeonLevel, dungeon);
            return dict;
        }
    }


    #endregion

    #region RankUp

    [Serializable]
    public class RankUpInfoData
    {
        public ERankType RankType;
        public string Name;
        public string PassiveName;
        public int RdLevel;
        public int MonsterDataId;
        public int BossBattleTimeLimit;
    }

    [Serializable]
    public class RankUpInfoDataLoader : ILoader<ERankType, RankUpInfoData>
    {
        public List<RankUpInfoData> rankUpInfoDatas = new List<RankUpInfoData>();
        public Dictionary<ERankType, RankUpInfoData> MakeDict()
        {
            Dictionary<ERankType, RankUpInfoData> dict = new Dictionary<ERankType, RankUpInfoData>();
            foreach (RankUpInfoData rank in rankUpInfoDatas)
                dict.Add(rank.RankType, rank);
            return dict;
        }
    }

    [Serializable]
    public class DrawRankUpGachaInfoData
    {
        public EHeroRankUpStatType StatType;
        public string Name;

        public List<int> ProbabilityList = new List<int>();
        public List<int> NormalValueList = new List<int>();
        public List<int> AdvanceValueList = new List<int>();
        public List<int> RareValueList = new List<int>();
        public List<int> LegendaryValueList = new List<int>();
        public List<int> MythicalValueList = new List<int>();
    }

    [Serializable]
    public class DrawRankUpGachaInfoDataLoader : ILoader<EHeroRankUpStatType, DrawRankUpGachaInfoData>
    {
        public List<DrawRankUpGachaInfoData> rankUpInfoDatas = new List<DrawRankUpGachaInfoData>();
        public Dictionary<EHeroRankUpStatType, DrawRankUpGachaInfoData> MakeDict()
        {
            Dictionary<EHeroRankUpStatType, DrawRankUpGachaInfoData> dict = new Dictionary<EHeroRankUpStatType, DrawRankUpGachaInfoData>();
            foreach (DrawRankUpGachaInfoData rank in rankUpInfoDatas)
                dict.Add(rank.StatType, rank);
            return dict;
        }
    }

    #endregion


    #region Shop

    [Serializable]
    public class ShopData
    {
        public string ShopItemName;
        public string Remark;
        public EShopItemType ShopItemType;
        public EItemType ItemType;
        public int Amount;
        public int Price;
    }

    [Serializable]
    public class ShopDataLoader : ILoader<string, ShopData>
    {
        public List<ShopData> shopDatas = new List<ShopData>();
        public Dictionary<string, ShopData> MakeDict()
        {
            Dictionary<string, ShopData> dict = new Dictionary<string, ShopData>();
            foreach (ShopData rank in shopDatas)
                dict.Add(rank.ShopItemName, rank);
            return dict;
        }
    }
    #endregion


    #region Quest

    [Serializable]
    public class QuestData
    {
        public int DataId;
        public string QuestName;
        public EQuestCategory QuestCategory;
        public EQuestType QuestType;
        public int RequestCount;
        public Dictionary<EItemType, int> RewardItem = new Dictionary<EItemType, int>();
    }

    [Serializable]
    public class QuestDataLoader : ILoader<int, QuestData>
    {
        public List<QuestData> questDatas = new List<QuestData>();
        public Dictionary<int, QuestData> MakeDict()
        {
            Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();
            foreach (QuestData quest in questDatas)
                dict.Add(quest.DataId, quest);
            return dict;
        }
    }

    #endregion


    #region Pet

    [Serializable]
    public class PetData
    {
        public EPetType PetType;
        public string PetName;
        public string Remark;
        public string PetDesc;

        public int ChapterLevel;
        public float DropCraftItemRate;
        public EPetCraftType PetCraftType;

        public string PetSpriteKey;
        public string PetCraftSpriteKey;
        public string PetObjectSpriteKey;
        public string PetObjectPrefabKey;

        public int MaxCount;

        // 보유 효과는 % 
        public float OwnedAtkPercent;
        public float OwnedAtkIncreasePercent;

        // 장착 효과는 합연산
        public float EquippedAtkValue;
        public float EquippedAtkIncreaseRate;
        public float EquippedHpValue;
        public float EquippedHpIncreaseRate;
    }

    [Serializable]
    public class PetDataLoader : ILoader<EPetType, PetData>
    {
        public List<PetData> petDatas = new List<PetData>();
        public Dictionary<EPetType, PetData> MakeDict()
        {
            Dictionary<EPetType, PetData> dict = new Dictionary<EPetType, PetData>();
            foreach (PetData pet in petDatas)
                dict.Add(pet.PetType, pet);
            return dict;
        }
    }

    #endregion

    #region Mission

    [Serializable]
    public class MissionData
    {
        public int DataId;
        public string Remark;
        public EMissionType MissionType;
        public int CompleteValue;
        public Dictionary<EItemType, int> RewardItem = new Dictionary<EItemType, int>();
    }

    [Serializable]
    public class MissionDataLoader : ILoader<int, MissionData>
    {
        public List<MissionData> missionDatas = new List<MissionData>();
        public Dictionary<int, MissionData> MakeDict()
        {
            Dictionary<int, MissionData> dict = new Dictionary<int, MissionData>();
            foreach (MissionData mission in missionDatas)
                dict.Add(mission.DataId, mission);
            return dict;
        }
    }

    #endregion



    [Serializable]
    public class TestInfoData
    {
        public int ID;
        public BigInteger  TestInt;
        public BigInteger  TestFloat;
        public BigInteger  TestLong;
        public BigInteger  TestDouble;
        public BigInteger TestDemical;
    }

    [Serializable]
    public class TestInfoDataLoader : ILoader<int, TestInfoData>
    {
        public List<TestInfoData> tests = new List<TestInfoData>();
        public Dictionary<int, TestInfoData> MakeDict()
        {
            Dictionary<int, TestInfoData> dict = new Dictionary<int, TestInfoData>();
            foreach (TestInfoData test in tests)
                dict.Add(test.ID, test);
            return dict;
        }
    }

}
