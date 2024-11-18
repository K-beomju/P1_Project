using System.Collections;
using System.Collections.Generic;
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
    public Dictionary<int, StageInfoData> StageChart { get; private set; } = new(); // 스테이지 정보
    public Dictionary<int, CreatureUpgradeStatInfoData> CreatureUpgradeStatChart { get; private set; } = new(); // 생명체 업그레이드 스탯 정보
    public Dictionary<int, HeroInfoData> HeroChart { get; private set; } = new(); // 영웅 정보
    public Dictionary<EHeroUpgradeType, HeroUpgradeInfoData> HeroUpgradeChart { get; private set; } = new();  // 영웅 업그레이드 스탯 정보
    public Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData> HeroUpgradeCostChart { get; private set; } = new(); // 영웅 업그레이드 스탯 가격 정보
    public Dictionary<EHeroAttrType, HeroAttributeInfoData> HeroAttributeChart { get; private set; } = new();  // 영웅 업그레이드 특성 정보
    public Dictionary<EHeroRelicType, HeroRelicInfoData> HeroRelicChart { get; private set; } = new();  // 영웅 업그레이드 유물 정보
    public Dictionary<EHeroAttrType, HeroAttributeCostInfoData> HeroAttributeCostChart { get; private set; } = new(); // 영웅 업그레이드 특성 가격 정보

    public Dictionary<int, MonsterInfoData> MonsterChart { get; private set; } = new();  // 일반 몬스터 정보
    public Dictionary<int, BossMonsterInfoData> BossMonsterChart { get; private set; } = new(); // 보스 몬스터 정보 
    public Dictionary<int, RankUpMonsterDataInfoData> RankUpMonsterChart { get; private set; } = new(); // 승급전 몬스터 정보 

    public Dictionary<int, DrawEquipmentGachaData> DrawEquipmentChart { get; private set; } = new();// 장비 뽑기 확률 정보
    public Dictionary<int, DrawSkillGachaData> DrawSkillChart { get; private set; } = new(); // 스킬 뽑기 확률 정보
    public Dictionary<int, EquipmentData> EquipmentChart { get; private set; } = new(); // 장비 정보
    public Dictionary<int, SkillData> SkillChart { get; private set; } = new(); // 스킬 정보
    public Dictionary<int, EffectData> EffectChart { get; private set; } = new(); // 이펙트 정보

    public Dictionary<int, GoldDungeonInfoData> GoldDungeonChart { get; private set; } = new(); // 골드 던전 정보
    public Dictionary<int, DiaDungeonInfoData> DiaDungeonChart { get; private set; } = new(); // 다이아 던전 정보
    public Dictionary<int, WorldBossDungeonInfoData> WorldBossDungeonChart { get; private set; } = new(); // 월드보스전 정보

    public Dictionary<EItemType, ItemData> ItemChart { get; private set; } = new(); // 아이템 정보
    public Dictionary<ERankType, RankUpInfoData> RankUpChart { get; private set; } = new(); // 승급 차트
    public Dictionary<EHeroRankUpStatType, DrawRankUpGachaInfoData> DrawRankUpChart { get; private set; } = new(); // 승급 스탯 뽑기 차트


    public void Init()
    {
        StageChart = LoadJson<StageInfoDataLoader, int, StageInfoData>("StageInfoData").MakeDict();
        MonsterChart = LoadJson<MonsterInfoDataLoader, int, MonsterInfoData>("MonsterInfoData").MakeDict();
        BossMonsterChart = LoadJson<BossMonsterInfoDataLoader, int, BossMonsterInfoData>("BossMonsterInfoData").MakeDict();
        RankUpMonsterChart =  LoadJson<RankUpMonsterDataInfoDataLoader, int, RankUpMonsterDataInfoData>("RankUpMonsterDataInfoData").MakeDict();
        HeroChart = LoadJson<HeroInfoDataLoader, int, HeroInfoData>("HeroInfoData").MakeDict();
        CreatureUpgradeStatChart = LoadJson<CreatureUpgradeStatInfoDataLoader, int, CreatureUpgradeStatInfoData>("CreatureUpgradeStatInfoData").MakeDict();
        HeroUpgradeChart = LoadJson<HeroUpgradeInfoDataLoader, EHeroUpgradeType, HeroUpgradeInfoData>("HeroUpgradeInfoData").MakeDict();
        HeroUpgradeCostChart = LoadJson<HeroUpgradeCostInfoDataLoader, EHeroUpgradeType, HeroUpgradeCostInfoData>("HeroUpgradeCostInfoData").MakeDict();
        HeroAttributeChart = LoadJson<HeroAttributeInfoDataLoader, EHeroAttrType, HeroAttributeInfoData>("HeroAttributeInfoData").MakeDict();
        HeroRelicChart = LoadJson<HeroRelicInfoDataLoader, EHeroRelicType, HeroRelicInfoData>("HeroRelicInfoData").MakeDict();
        HeroAttributeCostChart = LoadJson<HeroAttributeCostInfoDataLoader, EHeroAttrType, HeroAttributeCostInfoData>("HeroAttributeCostInfoData").MakeDict();
        DrawEquipmentChart = LoadJson<DrawEquipmentGachaDataLoader, int, DrawEquipmentGachaData>("DrawEquipmentGachaInfoData").MakeDict();
        DrawSkillChart = LoadJson<DrawSkillGachaDataLoader, int, DrawSkillGachaData>("DrawSkillGachaInfoData").MakeDict();
        EquipmentChart = LoadJson<EquipmentDataLoader, int, EquipmentData>("EquipmentData").MakeDict();
        SkillChart = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
        EffectChart = LoadJson<EffectDataLoader, int, EffectData>("EffectData").MakeDict();
        GoldDungeonChart = LoadJson<GoldDungeonInfoDataLoader, int, GoldDungeonInfoData>("GoldDungeonInfoData").MakeDict();
        DiaDungeonChart = LoadJson<DiaDungeonInfoDataLoader, int, DiaDungeonInfoData>("DiaDungeonInfoData").MakeDict();
        WorldBossDungeonChart = LoadJson<WorldBossDungeonInfoDataLoader, int, WorldBossDungeonInfoData>("WorldBossDungeonInfoData").MakeDict();

        ItemChart = LoadJson<ItemDataLoader, EItemType, ItemData>("ItemData").MakeDict();
        RankUpChart = LoadJson<RankUpInfoDataLoader, ERankType, RankUpInfoData>("RankUpInfoData").MakeDict();
        DrawRankUpChart = LoadJson<DrawRankUpGachaInfoDataLoader, EHeroRankUpStatType, DrawRankUpGachaInfoData>("DrawRankUpGachaInfoData").MakeDict();

    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>("Data/JsonData/" + path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
