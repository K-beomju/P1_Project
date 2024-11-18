using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public WorldBoss WorldBoss { get; private set; }
    public BossMonster BossMonster { get; private set; }
    public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();
    public Hero Hero { get; private set; }
    public Bot Bot { get; private set; }

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    public Transform BossMonsterRoot { get { return GetRootTransform("@BossMonster"); } }
    #endregion

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;
        return go;
    }

    public void SpawnRankUpMonster(Vector3 position)
    {
        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Pending);

        string prefabName = Managers.Data.RankUpMonsterChart[rankType].PrefabKey;
        GameObject go = Managers.Resource.Instantiate("Object/RankUpMonster/" + prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        obj.SetInfo(0);
    }

    public T Spawn<T>(Vector3 position, int dataTemplateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;
        if (prefabName == "Monster")
            prefabName += "/" + Managers.Data.MonsterChart[dataTemplateID].PrefabKey;
        else if (prefabName == "BossMonster")
            prefabName += "/" + Managers.Data.BossMonsterChart[dataTemplateID].PrefabKey;
        else if(prefabName == "WorldBoss")
            prefabName += "/" + Managers.Data.WorldBossDungeonChart[1].PrefabKey;

        GameObject go = Managers.Resource.Instantiate("Object/" + prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();
        if (obj is Monster monster)
        {
            monster.transform.parent = MonsterRoot;
            Monsters.Add(monster);
        }
        if (obj is BossMonster bossMonster)
        {
            bossMonster.transform.parent = BossMonsterRoot;
            BossMonster = bossMonster;
        }
        if (obj is WorldBoss worldBoss)
        {
            WorldBoss = worldBoss;
        }
        if (obj is Hero hero)
        {
            hero.transform.parent = HeroRoot;
            Hero = hero;
        }
        if (obj is Bot bot)
        {
            Bot = bot;
        }
        obj.SetInfo(dataTemplateID);

        return obj as T;
    }

    private string GetPrefabName<T>(int dataTemplateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        if (typeof(T) == typeof(Monster))
            return $"{prefabName}/{Managers.Data.MonsterChart[dataTemplateID].PrefabKey}";
        if (typeof(T) == typeof(BossMonster) || typeof(T) == typeof(WorldBoss))
            return $"{prefabName}/{Managers.Data.BossMonsterChart[dataTemplateID].PrefabKey}";

        return prefabName; // 기본 Prefab 이름 반환
    }

    public void Despawn<T>(T obj) where T : MonoBehaviour
    {
        if (typeof(T) == typeof(Monster))
        {
            Monster monster = obj.GetComponent<Monster>();
            Monsters.Remove(monster);
        }
        if (typeof(T) == typeof(BossMonster))
        {
            BossMonster = null;
        }
        if (typeof(T) == typeof(Hero))
        {
            Hero = null;
        }
        Managers.Resource.Destroy(obj.gameObject);
    }

    public void Clear()
    {
        WorldBoss = null;
        BossMonster = null;
        Monsters.Clear();
        Hero = null;
    }
}
