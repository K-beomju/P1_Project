using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Util.GetOrAddComponent<T>(go);
	}

	public static void BindEvent(this GameObject go, Action action = null, Define.EUIEvent type = Define.EUIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type);
	}

	public static bool IsValid(this GameObject go)
	{
		return go != null && go.activeSelf;
	}

	public static bool IsValid(this BaseObject bo)
	{
		if (bo == null || bo.isActiveAndEnabled == false)
			return false;

		return true;
	}

	public static void DestroyChilds(this GameObject go)
	{
		foreach (Transform child in go.transform)
			Managers.Resource.Destroy(child.gameObject);
	}


	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;

		while (n > 1)
		{
			n--;
			int k = UnityEngine.Random.Range(0, n + 1);
			(list[k], list[n]) = (list[n], list[k]); //swap
		}
	}

	public static bool IsEquipmentType(this EDrawType type)
    {
        return type == EDrawType.Weapon || type == EDrawType.Armor || type == EDrawType.Ring;
    }
}
