using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public BossMonster BossMonster { get; private set; }
    public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();
    public HashSet<Hero> Heroes { get; set; } = new HashSet<Hero>();

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

        GameObject go = Managers.Resource.Instantiate("Object/" + prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();
        if (typeof(T) == typeof(Monster))
        {
            Monster monster = go.GetComponent<Monster>();
            monster.transform.parent = MonsterRoot;
            Monsters.Add(monster); 
        }
        if (typeof(T) == typeof(BossMonster))
        {
            BossMonster bossMonster = go.GetComponent<BossMonster>();
            bossMonster.transform.parent = BossMonsterRoot;
            BossMonster = bossMonster;
        }
        if (typeof(T) == typeof(Hero))
        {
            Hero hero = go.GetComponent<Hero>();
            hero.transform.parent = HeroRoot;
            Heroes.Add(hero);
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
            Hero hero = obj.GetComponent<Hero>();
            Heroes.Remove(hero);
        }
        Managers.Resource.Destroy(obj.gameObject);
    }

    public void Clear()
    {
        BossMonster = null;
        Monsters.Clear();
        Heroes.Clear();
    }
}
