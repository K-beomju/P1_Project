using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager
{

	public T Load<T>(string path) where T : Object
	{
		return Resources.Load<T>(path);
	}

	public GameObject Instantiate(string path, Transform parent = null, bool pooling = false)
	{
		GameObject prefab = Load<GameObject>($"Prefabs/{path}");
		if (prefab == null)
		{
			Debug.Log($"Failed to load prefab : {path}");
			return null;
		}

			// if (pooling)
			// return Managers.Pool.Pop(prefab);

		return Instantiate(prefab, parent);
	}

	public GameObject Instantiate(GameObject prefab, Transform parent = null)
	{
		GameObject go = Object.Instantiate(prefab, parent);
		go.name = prefab.name;
		return go;
	}

	public void Destroy(GameObject go)
	{
		if (go == null)
			return;

		Object.Destroy(go);
	}
}