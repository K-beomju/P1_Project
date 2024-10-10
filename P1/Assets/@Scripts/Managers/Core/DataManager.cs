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
    public Dictionary<int, StageInfoData> StageChart { get; private set; } = new Dictionary<int, StageInfoData>();
    public Dictionary<int, MonsterInfoData> MonsterChart { get; private set; } = new Dictionary<int, MonsterInfoData>();
    public Dictionary<int, BossMonsterInfoData> BossMonsterChart { get; private set; } = new Dictionary<int, BossMonsterInfoData>();
    public Dictionary<int, CreatureUpgradeStatInfoData> CreatureUpgradeStatChart { get; private set; } = new Dictionary<int, CreatureUpgradeStatInfoData>();
    public Dictionary<EHeroUpgradeType, HeroUpgradeInfoData> HeroUpgradeChart { get; private set; } = new Dictionary<EHeroUpgradeType, HeroUpgradeInfoData>();
    public Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData> HeroUpgradeCostChart { get; private set; } = new Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData>();
    public Dictionary<int, HeroInfoData> HeroChart { get; private set; } = new Dictionary<int, HeroInfoData>();
    public Dictionary<int, DrawEquipmentGachaData> DrawEquipmentChart { get; private set; } = new Dictionary<int, DrawEquipmentGachaData>();
    public Dictionary<int, DrawSkillGachaData> DrawSkillChart { get; private set; } = new Dictionary<int, DrawSkillGachaData>();
    public Dictionary<int, EquipmentData> EquipmentChart { get; private set; } = new Dictionary<int, EquipmentData>();
    public Dictionary<int, SkillData> SkillChart { get; private set; } = new Dictionary<int, SkillData>();

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
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>("Data/JsonData/" + path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
