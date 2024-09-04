using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;

    private static Managers s_instacne;
    private static Managers Instance { get { Init(); return s_instacne; } }

    #region Contents 
    private ObjectManager _object = new ObjectManager();
    private GameManager _game = new GameManager();

    public static ObjectManager Object { get { return Instance?._object; } }
    public static GameManager Game { get { return Instance?._game; } }
    #endregion

    #region Core
	private ResourceManager _resource = new ResourceManager();
	private SceneManagerEx _scene = new SceneManagerEx();
	private SoundManager _sound = new SoundManager();
	private UIManager _ui = new UIManager();

	public static ResourceManager Resource { get { return Instance?._resource; } }
	public static SceneManagerEx Scene { get { return Instance?._scene; } }
	public static SoundManager Sound { get { return Instance?._sound; } }
	public static UIManager UI { get { return Instance?._ui; } }
	#endregion

    public static void Init()
    {
        if (s_instacne == null && Initialized == false)
        {
            Initialized = true;
            
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);

            s_instacne = go.GetComponent<Managers>();
        }
    }
}