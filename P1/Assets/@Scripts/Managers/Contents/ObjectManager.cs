using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public List<Box> boxes { get; set; } = new List<Box>();

    public T Spawn<T>(Vector3 position) where T : MonoBehaviour
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        if(typeof(T) == typeof(Box))
        {
            Box box = go.GetComponent<Box>();
            boxes.Add(box);
        }

        return go as T;
    }
}
