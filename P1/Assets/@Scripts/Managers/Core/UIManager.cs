using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
	private int _order = 10;

	private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
	private Dictionary<string, UI_Popup> _popups = new Dictionary<string, UI_Popup>();

	private UI_Scene _sceneUI = null;
	public UI_Scene SceneUI
	{
		set { _sceneUI = value; }
		get { return _sceneUI; }
	}

	public GameObject Root
	{
		get
		{
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };
			return root;
		}
	}

	public void CacheAllPopups()
	{
		var list = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.IsSubclassOf(typeof(UI_Popup)));

		foreach (Type type in list)
		{
			Debug.Log(type.Name);
			CachePopupUI(type);
		}
		CloseAllPopupUI();
	}


	public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
	{
		Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
		if (canvas == null)
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.overrideSorting = true;
		}

		CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
		if (cs != null)
		{
			cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			cs.referenceResolution = new Vector2(1080, 1920);
		}

		go.GetOrAddComponent<GraphicRaycaster>();

		if (sort)
		{
			canvas.sortingOrder = _order + Define.SortingLayers.UI_POPUP;
			_order++;
		}
		else
		{
			canvas.sortingOrder = sortOrder;
		}
	}

	public T GetSceneUI<T>() where T : UI_Base
	{
		return _sceneUI as T;
	}

	public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/WorldSpace/{name}");
		GameObject go = Managers.Resource.Instantiate(prefab);
		if (parent != null)
			go.transform.SetParent(parent);

		Canvas canvas = go.GetOrAddComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.worldCamera = Camera.main;

		return Util.GetOrAddComponent<T>(go);
	}

	public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/SubItem/{name}");
		GameObject go = Managers.Resource.Instantiate(prefab);
		go.transform.SetParent(parent);
		go.transform.localScale = Vector3.one;
		return Util.GetOrAddComponent<T>(go);
	}

	public T ShowBaseUI<T>(string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Base/{name}");
		GameObject go = Managers.Resource.Instantiate(prefab);
		T baseUI = Util.GetOrAddComponent<T>(go);

		go.transform.SetParent(Root.transform);

		return baseUI;
	}

	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Scene/{name}");
		GameObject go = Managers.Resource.Instantiate(prefab);
		T sceneUI = Util.GetOrAddComponent<T>(go);
		_sceneUI = sceneUI;

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null) where T : UI_Popup
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		if (_popups.TryGetValue(name, out UI_Popup popup) == false)
		{
			GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{name}");
			GameObject go = Managers.Resource.Instantiate(prefab);

			popup = Util.GetOrAddComponent<T>(go);
			_popups[name] = popup;
		}

		_popupStack.Push(popup);
		popup.gameObject.SetActive(true);
		
		return popup as T;
	}

	public void CachePopupUI(Type type)
	{
		string name = type.Name;

		if (_popups.TryGetValue(name, out UI_Popup popup) == false)
		{
			GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{name}");
			GameObject go = Managers.Resource.Instantiate(prefab);
			popup = go.GetComponent<UI_Popup>();
			_popups[name] = popup;
		}
		popup.transform.SetParent(Root.transform);
		_popupStack.Push(popup);
	}

	public void ClosePopupUI(UI_Popup popup)
	{
		if (_popupStack.Count == 0)
			return;

		if (_popupStack.Peek() != popup)
		{
			Debug.Log("Close Popup Failed!");
			return;
		}

		ClosePopupUI();
	}

	public void ClosePopupUI()
	{
		if (_popupStack.Count == 0)
			return;

		UI_Popup popup = _popupStack.Pop();
		popup.gameObject.SetActive(false);
		_order--;
	}

	public void CloseAllPopupUI()
	{
		while (_popupStack.Count > 0)
			ClosePopupUI();
	}

	public int GetPopupCount()
	{
		return _popupStack.Count;
	}

	public void Clear()
	{
		CloseAllPopupUI();
		_sceneUI = null;
	}
}
