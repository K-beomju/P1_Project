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
    public Dictionary<int, StageInfoData> StageChart { get; private set; } = new Dictionary<int, StageInfoData>(); // 스테이지 정보
    public Dictionary<int, CreatureUpgradeStatInfoData> CreatureUpgradeStatChart { get; private set; } = new Dictionary<int, CreatureUpgradeStatInfoData>(); // 생명체 업그레이드 스탯 정보
    public Dictionary<int, HeroInfoData> HeroChart { get; private set; } = new Dictionary<int, HeroInfoData>(); // 영웅 정보
    public Dictionary<EHeroUpgradeType, HeroUpgradeInfoData> HeroUpgradeChart { get; private set; } = new Dictionary<EHeroUpgradeType, HeroUpgradeInfoData>();  // 영웅 업그레이드 스탯 정보
    public Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData> HeroUpgradeCostChart { get; private set; } = new Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData>(); // 영웅 업그레이드 가격 정보
    public Dictionary<int, MonsterInfoData> MonsterChart { get; private set; } = new Dictionary<int, MonsterInfoData>();  // 일반 몬스터 정보
    public Dictionary<int, BossMonsterInfoData> BossMonsterChart { get; private set; } = new Dictionary<int, BossMonsterInfoData>();// 보스 몬스터 정보 
    public Dictionary<int, DrawEquipmentGachaData> DrawEquipmentChart { get; private set; } = new Dictionary<int, DrawEquipmentGachaData>();// 장비 뽑기 확률 정보
    public Dictionary<int, DrawSkillGachaData> DrawSkillChart { get; private set; } = new Dictionary<int, DrawSkillGachaData>(); // 스킬 뽑기 확률 정보
    public Dictionary<int, EquipmentData> EquipmentChart { get; private set; } = new Dictionary<int, EquipmentData>(); // 장비 정보
    public Dictionary<int, SkillData> SkillChart { get; private set; } = new Dictionary<int, SkillData>(); // 스킬 정보
    public Dictionary<int, EffectData> EffectChart { get; private set; } = new Dictionary<int, EffectData>(); // 이펙트 정보

    public void Init()
    {
        StageChart = LoadJson<StageInfoDataLoader, int, StageInfoData>("StageInfoData").MakeDict();
        MonsterChart = LoadJson<MonsterInfoDataLoader, int, MonsterInfoData>("MonsterInfoData").MakeDict();
        BossMonsterChart = LoadJson<BossMonsterInfoDataLoader, int, BossMonsterInfoData>("BossMonsterInfoData").MakeDict();
        HeroChart = LoadJson<HeroInfoDataLoader, int, HeroInfoData>("HeroInfoData").MakeDict();
        CreatureUpgradeStatChart = LoadJson<CreatureUpgradeStatInfoDataLoader, int, CreatureUpgradeStatInfoData>("CreatureUpgradeStatInfoData").MakeDict();
        HeroUpgradeChart = LoadJson<HeroUpgradeInfoDataLoader, EHeroUpgradeType, HeroUpgradeInfoData>("HeroUpgradeInfoData").MakeDict();
        HeroUpgradeCostChart = LoadJson<HeroUpgradeCostInfoDataLoader, EHeroUpgradeType, HeroUpgradeCostInfoData>("HeroUpgradeCostInfoData").MakeDict();
        DrawEquipmentChart = LoadJson<DrawEquipmentGachaDataLoader, int, DrawEquipmentGachaData>("DrawEquipmentGachaInfoData").MakeDict();
        DrawSkillChart = LoadJson<DrawSkillGachaDataLoader, int, DrawSkillGachaData>("DrawSkillGachaInfoData").MakeDict();
        EquipmentChart = LoadJson<EquipmentDataLoader, int, EquipmentData>("EquipmentData").MakeDict();
        SkillChart = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
        EffectChart = LoadJson<EffectDataLoader, int, EffectData>("EffectData").MakeDict();

    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>("Data/JsonData/" + path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
