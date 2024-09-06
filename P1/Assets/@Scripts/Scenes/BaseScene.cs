using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static Define;

public abstract class BaseScene : InitBase
{
	public EScene SceneType { get; protected set; } = EScene.Unknown;

	protected override bool Init()
	{
		if (base.Init() == false)
			return false;

		Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
		if (obj == null)
		{
			GameObject go = new GameObject() { name = "@EventSystem" };
			go.AddComponent<EventSystem>();
			go.AddComponent<StandaloneInputModule>();
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 0.0f);



		return true;
	}

	public abstract void Clear();
}
