using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public WorldBoss WorldBoss { get; private set; }
    public RankMonster RankMonster { get; private set; }
    public BossMonster BossMonster { get; private set; }
    public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();
    public List<Pet> Pets { get; set; } = new List<Pet>();

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
        GameObject go = Managers.Resource.Instantiate("Object/" + prefabName, pooling: true);
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        Monster monster = obj.GetComponent<Monster>();
        Monsters.Add(monster);

        obj.SetInfo(dataTemplateID);
    }

    public void SpawnBossMonster(Vector3 position, int dataTemplateID)
    {
        string prefabName = "BossMonster/" + Managers.Data.BossMonsterChart[dataTemplateID].PrefabKey;
        GameObject go = Managers.Resource.Instantiate("Object/" + prefabName, pooling: true);
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        BossMonster bossMonster = obj.GetComponent<BossMonster>();
        BossMonster = bossMonster;

        obj.SetInfo(dataTemplateID);
    }

    public RankMonster SpawnRankMonster(Vector3 position, int dataTemplateID)
    {
        string prefabName = "RankMonster/" + Managers.Data.RankUpMonsterChart[dataTemplateID].PrefabKey;
        GameObject go = Managers.Resource.Instantiate("Object/" + prefabName, pooling: true);
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        RankMonster rankMonster = obj.GetComponent<RankMonster>();
        RankMonster = rankMonster;

        obj.SetInfo(dataTemplateID);

        return rankMonster;
    }

    #region  Pet

    public GameObject SpawnPet(Vector3 position, PetData petData)
    {
        GameObject go = Managers.Resource.Instantiate(petData.PetObjectPrefabKey, pooling: false);
        go.transform.position = position;
        go.name = petData.Remark;
        Pet pet = go.GetComponent<Pet>();
        Pets.Add(pet);
        return go;
    }

    public void DespawnPet(string petName)
    {
        Pet petToRemove = Pets.FirstOrDefault(p => p.name == petName);
        if (petToRemove != null)
        {
            Managers.Resource.Destroy(petToRemove.gameObject);
            Pets.Remove(petToRemove);
        }
    }

    public Pet FindPetByName(string petName)
    {
        return Pets.FirstOrDefault(p => p.name == petName);
    }

    #endregion



    public T Spawn<T>(Vector3 position, int dataTemplateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;
        if (prefabName == "WorldBoss")
            prefabName += "/" + Managers.Data.WorldBossDungeonChart[1].PrefabKey;
        else if (prefabName == "RankMonster")
            prefabName += "/" + Managers.Data.RankUpMonsterChart[dataTemplateID].PrefabKey;


        GameObject go = Managers.Resource.Instantiate("Object/" + prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();
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
        if (typeof(T) == typeof(RankMonster))
        {
            RankMonster = null;
        }
        Managers.Pool.Push(obj.gameObject);
    }

    public void DeleteRankMonster()
    {
        if (RankMonster == null)
            return;

        Transform poolTrm = GetRootTransform($"@{RankMonster.name}Pool");
        Managers.Resource.Destroy(poolTrm.gameObject);
    }

    public void KillAllMonsters()
    {
        for (int i = Managers.Object.Monsters.Count - 1; i >= 0; i--)
        {
            Monster monster = Monsters.ElementAt(i);
            Despawn(monster);
        }

        if (BossMonster != null)
            Despawn(BossMonster);

        if (RankMonster != null)
            Despawn(RankMonster);
    }

    public void Clear()
    {
        WorldBoss = null;
        BossMonster = null;
        Monsters.Clear();
        Hero = null;
    }
}
