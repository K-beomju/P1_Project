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
    //public Dictionary<int, StageInfoData> StageDataDic { get; private set; } = new Dictionary<int, StageInfoData>();
    //public Dictionary<int, MonsterInfoData> MonsterDataDic { get; private set; } = new Dictionary<int, MonsterInfoData>();
    //public Dictionary<int, BossMonsterInfoData> BossMonsterDataDic { get; private set; } = new Dictionary<int, BossMonsterInfoData>();
    //public Dictionary<int, CreatureUpgradeStatInfoData> CreatureUpgradeStatInfoDataDic { get; private set; } = new Dictionary<int, CreatureUpgradeStatInfoData>();
    //public Dictionary<EHeroUpgradeType, HeroUpgradeInfoData> HeroUpgradeInfoDataDic { get; private set; } = new Dictionary<EHeroUpgradeType, HeroUpgradeInfoData>();
    //public Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData> HeroUpgradeCostInfoDataDic { get; private set; } = new Dictionary<EHeroUpgradeType, HeroUpgradeCostInfoData>();
    //public Dictionary<int, HeroInfoData> HeroInfoDataDic { get; private set; } = new Dictionary<int, HeroInfoData>();
    //public Dictionary<int, DrawEquipmentGachaData> GachaDataDic { get; private set; } = new Dictionary<int, DrawEquipmentGachaData>();
    //public Dictionary<int, EquipmentData> EquipmentDic { get; private set; } = new Dictionary<int, EquipmentData>();

    public void Init()
    {
        //StageDataDic = LoadJson<StageInfoDataLoader, int, StageInfoData>("StageInfoData").MakeDict();
        //MonsterDataDic = LoadJson<MonsterInfoDataLoader, int, MonsterInfoData>("MonsterInfoData").MakeDict();
        //BossMonsterDataDic = LoadJson<BossMonsterInfoDataLoader, int, BossMonsterInfoData>("BossMonsterInfoData").MakeDict();
        //HeroInfoDataDic = LoadJson<HeroInfoDataLoader, int, HeroInfoData>("HeroInfoData").MakeDict();
        //CreatureUpgradeStatInfoDataDic = LoadJson<CreatureUpgradeStatInfoDataLoader, int, CreatureUpgradeStatInfoData>("CreatureUpgradeStatInfoData").MakeDict();
        //HeroUpgradeInfoDataDic = LoadJson<HeroUpgradeInfoDataLoader, EHeroUpgradeType, HeroUpgradeInfoData>("HeroUpgradeInfoData").MakeDict();
        //HeroUpgradeCostInfoDataDic = LoadJson<HeroUpgradeCostInfoDataLoader, EHeroUpgradeType, HeroUpgradeCostInfoData>("HeroUpgradeCostInfoData").MakeDict();
        //GachaDataDic = LoadJson<DrawEquipmentGachaDataLoader, int, DrawEquipmentGachaData>("DrawEquipmentGachaData").MakeDict();
        //EquipmentDic = LoadJson<EquipmentDataLoader, int, EquipmentData>("EquipmentData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>("Data/JsonData/" + path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
