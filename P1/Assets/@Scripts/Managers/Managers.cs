using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;

    private static Managers s_instacne;
    private static Managers Instance { get { Init(); return s_instacne; } }

    #region Contents 
    private ObjectManager _object = new ObjectManager();
    private GameManager _game = new GameManager();
    private EventManager _event = new EventManager();
    private HeroManager _hero = new HeroManager();
    private EquipmentManager _equipment = new EquipmentManager();

    public static ObjectManager Object { get { return Instance?._object; } }
    public static GameManager Game { get { return Instance?._game; } }
    public static EventManager Event { get { return Instance?._event; } }
    public static HeroManager Hero { get { return Instance?._hero; } }
    public static EquipmentManager Equipment { get { return Instance?._equipment; } }
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

    #region Server
    private BackendManager _backend = new BackendManager();

    public static BackendManager Backend { get {  return Instance?._backend; } }
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


    public static void Clear()
    {
        Event.Clear();
        Scene.Clear();
        UI.Clear();
        Object.Clear();
    }
}