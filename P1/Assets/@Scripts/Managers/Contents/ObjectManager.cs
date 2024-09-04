using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    #endregion
    
    public T Spawn<T>(Vector3 position) where T : MonoBehaviour
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        if (typeof(T) == typeof(Monster))
        {
            Monster monster = go.GetComponent<Monster>();
            Monsters.Add(monster);
        }

        return go as T;
    }
}
