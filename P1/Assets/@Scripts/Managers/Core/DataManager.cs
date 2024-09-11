using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.StageInfoData> StageDic { get; private set; } = new Dictionary<int, Data.StageInfoData>();
    public Dictionary<int, Data.MonsterInfoData> MonsterDic { get; private set; } = new Dictionary<int, Data.MonsterInfoData>();
    public Dictionary<int, Data.CreatureUpgradeStatInfoData> CreatureUpgradeDic { get; private set; } = new Dictionary<int, Data.CreatureUpgradeStatInfoData>();


    public void Init()
    {
        StageDic = LoadJson<Data.StageInfoDataLoader, int, Data.StageInfoData>("Data/JsonData/StageInfoData").MakeDict();
        MonsterDic = LoadJson<Data.MonsterInfoDataLoader, int, Data.MonsterInfoData>("Data/JsonData/MonsterInfoData").MakeDict();
        CreatureUpgradeDic= LoadJson<Data.CreatureUpgradeStatInfoDataLoader, int, Data.CreatureUpgradeStatInfoData>("Data/JsonData/CreatureUpgradeStatInfoData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
