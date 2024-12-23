using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Data;
using Newtonsoft.Json;
using UnityEngine;
using static Define;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    // [스테이지]
    public Dictionary<int, StageInfoData> StageChart { get; private set; } = new(); 
    // [던전]
    public Dictionary<int, GoldDungeonInfoData> GoldDungeonChart { get; private set; } = new(); 
    public Dictionary<int, DiaDungeonInfoData> DiaDungeonChart { get; private set; } = new(); 
    public Dictionary<int, WorldBossDungeonInfoData> WorldBossDungeonChart { get; private set; } = new();
    // [영웅]
    public Dictionary<EHeroAttrType, HeroAttributeCostInfoData> HeroAttributeCostChart { get; private set; } = new(); 
    public Dictionary<EHeroAttrType, HeroAttributeInfoData> HeroAttributeChart { get; private set; } = new(); 
    public Dictionary<int, HeroInfoData> HeroChart { get; private set; } = new(); 
    public Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData> HeroUpgradeCostChart { get; private set; } = new(); 
    public Dictionary<EHeroUpgradeType, HeroUpgradeInfoData> HeroUpgradeChart { get; private set; } = new();  
    // [유물]
    public Dictionary<EHeroRelicType, RelicInfoData> RelicChart { get; private set; } = new(); 
    // [승급]
    public Dictionary<ERankType, RankUpInfoData> RankUpChart { get; private set; } = new(); 
    // [가챠]
    public Dictionary<int, DrawEquipmentGachaData> DrawEquipmentChart { get; private set; } = new();
    public Dictionary<int, DrawSkillGachaData> DrawSkillChart { get; private set; } = new();
    public Dictionary<EHeroRankUpStatType, DrawRankUpGachaInfoData> DrawRankUpChart { get; private set; } = new(); 
	// [몬스터]
    public Dictionary<int, MonsterInfoData> MonsterChart { get; private set; } = new();  
    public Dictionary<int, BossMonsterInfoData> BossMonsterChart { get; private set; } = new(); 
    public Dictionary<int, RankUpMonsterInfoData> RankUpMonsterChart { get; private set; } = new(); 
    // [장비]
    public Dictionary<int, EquipmentData> EquipmentChart { get; private set; } = new(); 
    // [스킬]
    public Dictionary<int, SkillData> SkillChart { get; private set; } = new(); 
    public Dictionary<int, EffectData> EffectChart { get; private set; } = new(); 
	// [아이템], [상점], [퀘스트]
    public Dictionary<EItemType, ItemData> ItemChart { get; private set; } = new(); 
    public Dictionary<string, ShopData> ShopChart { get; private set; } = new();
    public Dictionary<int, QuestData> QuestChart { get; private set; } = new(); 
	// [펫]
    public Dictionary<EPetType, PetData> PetChart { get; private set; } = new(); 
	// [미션]
    public Dictionary<int, MissionData> MissionChart { get; private set; } = new(); 


    public void Init()
    {
        // [스테이지]
        StageChart = LoadJson<StageInfoDataLoader, int, StageInfoData>("StageInfoData").MakeDict();
		
        // [던전]
        GoldDungeonChart = LoadJson<GoldDungeonInfoDataLoader, int, GoldDungeonInfoData>("GoldDungeonInfoData").MakeDict();
        DiaDungeonChart = LoadJson<DiaDungeonInfoDataLoader, int, DiaDungeonInfoData>("DiaDungeonInfoData").MakeDict();
        WorldBossDungeonChart = LoadJson<WorldBossDungeonInfoDataLoader, int, WorldBossDungeonInfoData>("WorldBossDungeonInfoData").MakeDict();

        // [영웅]
        HeroAttributeChart = LoadJson<HeroAttributeInfoDataLoader, EHeroAttrType, HeroAttributeInfoData>("HeroAttributeInfoData").MakeDict();
        HeroAttributeCostChart = LoadJson<HeroAttributeCostInfoDataLoader, EHeroAttrType, HeroAttributeCostInfoData>("HeroAttributeCostInfoData").MakeDict();
        HeroChart = LoadJson<HeroInfoDataLoader, int, HeroInfoData>("HeroInfoData").MakeDict();
        HeroUpgradeCostChart = LoadJson<HeroUpgradeCostInfoDataLoader, EHeroUpgradeType, HeroUpgradeCostInfoData>("HeroUpgradeCostInfoData").MakeDict();
        HeroUpgradeChart = LoadJson<HeroUpgradeInfoDataLoader, EHeroUpgradeType, HeroUpgradeInfoData>("HeroUpgradeInfoData").MakeDict();

        // [유물]
        RelicChart = LoadJson<RelicInfoDataLoader, EHeroRelicType, RelicInfoData>("RelicData").MakeDict();

        // [승급]
        RankUpChart = LoadJson<RankUpInfoDataLoader, ERankType, RankUpInfoData>("RankUpData").MakeDict();

     	// [가챠]
        DrawEquipmentChart = LoadJson<DrawEquipmentGachaDataLoader, int, DrawEquipmentGachaData>("DrawEquipmentGachaInfoData").MakeDict();
        DrawSkillChart = LoadJson<DrawSkillGachaDataLoader, int, DrawSkillGachaData>("DrawSkillGachaInfoData").MakeDict();
        DrawRankUpChart = LoadJson<DrawRankUpGachaInfoDataLoader, EHeroRankUpStatType, DrawRankUpGachaInfoData>("DrawRankUpGachaInfoData").MakeDict();

		// [몬스터]
        MonsterChart = LoadJson<MonsterInfoDataLoader, int, MonsterInfoData>("MonsterInfoData").MakeDict();
        BossMonsterChart = LoadJson<BossMonsterInfoDataLoader, int, BossMonsterInfoData>("BossMonsterInfoData").MakeDict();
        RankUpMonsterChart =  LoadJson<RankUpMonsterInfoDataLoader, int, RankUpMonsterInfoData>("RankUpMonsterInfoData").MakeDict();
        
        // [장비]
        EquipmentChart = LoadJson<EquipmentDataLoader, int, EquipmentData>("EquipmentData").MakeDict();
        
        // [스킬]
        SkillChart = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
        EffectChart = LoadJson<EffectDataLoader, int, EffectData>("EffectData").MakeDict();

		// [아이템], [상점], [퀘스트]
        ItemChart = LoadJson<ItemDataLoader, EItemType, ItemData>("ItemData").MakeDict();
        ShopChart = LoadJson<ShopDataLoader, string, ShopData>("ShopData").MakeDict();
        QuestChart = LoadJson<QuestDataLoader, int, QuestData>("QuestData").MakeDict();

        // [펫]
        PetChart = LoadJson<PetDataLoader, EPetType, PetData>("PetData").MakeDict();

        // [미션]
        MissionChart = LoadJson<MissionDataLoader, int, MissionData>("MissionData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>("Data/JsonData/" + path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
