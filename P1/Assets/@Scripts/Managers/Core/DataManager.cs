using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static Define;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.StageInfoData> StageDic { get; private set; } = new Dictionary<int, Data.StageInfoData>();
    public Dictionary<int, Data.MonsterInfoData> MonsterDic { get; private set; } = new Dictionary<int, Data.MonsterInfoData>();
    public Dictionary<int, Data.BossMonsterInfoData> BossMonsterDic { get; private set; } = new Dictionary<int, Data.BossMonsterInfoData>();
    public Dictionary<int, Data.CreatureUpgradeStatInfoData> CreatureUpgradeDic { get; private set; } = new Dictionary<int, Data.CreatureUpgradeStatInfoData>();
    public Dictionary<EHeroUpgradeType, Data.HeroUpgradeInfoData> HeroUpgradeInfoDataDic { get; private set; } = new Dictionary<EHeroUpgradeType, Data.HeroUpgradeInfoData>();
    public Dictionary<EHeroUpgradeType, Data.HeroUpgradeCostInfoData> HeroUpgradeCostInfoDataDic { get; private set; } = new Dictionary<EHeroUpgradeType, Data.HeroUpgradeCostInfoData>();


    public void Init()
    {
        StageDic = LoadJson<Data.StageInfoDataLoader, int, Data.StageInfoData>("StageInfoData").MakeDict();
        MonsterDic = LoadJson<Data.MonsterInfoDataLoader, int, Data.MonsterInfoData>("MonsterInfoData").MakeDict();
        BossMonsterDic = LoadJson<Data.BossMonsterInfoDataLoader, int, Data.BossMonsterInfoData>("BossMonsterInfoData").MakeDict();
        CreatureUpgradeDic = LoadJson<Data.CreatureUpgradeStatInfoDataLoader, int, Data.CreatureUpgradeStatInfoData>("CreatureUpgradeStatInfoData").MakeDict();
        HeroUpgradeInfoDataDic = LoadJson<Data.HeroUpgradeInfoDataLoader, EHeroUpgradeType, Data.HeroUpgradeInfoData>("HeroUpgradeInfoData").MakeDict();
        HeroUpgradeCostInfoDataDic = LoadJson<Data.HeroUpgradeCostInfoDataLoader, EHeroUpgradeType, Data.HeroUpgradeCostInfoData>("HeroUpgradeCostInfoData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>("Data/JsonData/" + path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
