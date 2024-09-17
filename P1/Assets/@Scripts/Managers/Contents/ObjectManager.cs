using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class ObjectManager
{
    public BossMonster BossMonster { get; private set; }
    public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();
    public Hero Hero { get; private set; }

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

    public T Spawn<T>(Vector3 position, int dataTemplateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;
        if(prefabName == "Monster")
        prefabName += "/" + Managers.Data.MonsterDic[dataTemplateID].PrefabKey;
        else if(prefabName == "BossMonster")
                prefabName += "/" + Managers.Data.BossMonsterDic[dataTemplateID].PrefabKey;

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
        if (obj is Hero hero)
        {
            hero.transform.parent = HeroRoot;
            Hero = hero;
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
            Managers.Game.OnMonsterDestroyed();
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
        BossMonster = null;
        Monsters.Clear();
        Hero = null;
    }
}
