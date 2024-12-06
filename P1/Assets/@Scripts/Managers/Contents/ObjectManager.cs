using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public WorldBoss WorldBoss { get; private set; }
    public RankMonster RankMonster { get; private set; }
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

    public void SpawnMonster(Vector3 position, int dataTemplateID) 
    {
        string prefabName = "Monster/" + Managers.Data.MonsterChart[dataTemplateID].PrefabKey;
        GameObject go = Managers.Resource.Instantiate("Object/" + prefabName,pooling: true);
        //go.name = prefabName;
        go.transform.position = position;
        
        BaseObject obj = go.GetComponent<BaseObject>();

        Monster monster = obj.GetComponent<Monster>();
        Monsters.Add(monster);

        obj.SetInfo(dataTemplateID);
    }


    public T Spawn<T>(Vector3 position, int dataTemplateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;
        if (prefabName == "Monster")
            prefabName += "/" + Managers.Data.MonsterChart[dataTemplateID].PrefabKey;
        else if (prefabName == "BossMonster")
            prefabName += "/" + Managers.Data.BossMonsterChart[dataTemplateID].PrefabKey;
        else if (prefabName == "WorldBoss")
            prefabName += "/" + Managers.Data.WorldBossDungeonChart[1].PrefabKey;
        else if (prefabName == "RankMonster")
            prefabName += "/" + Managers.Data.RankUpMonsterChart[dataTemplateID].PrefabKey;


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
        if(obj is RankMonster rankUpMonster)
        {
            RankMonster = rankUpMonster;
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
