using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;

    private static Managers s_instacne;
    public static Managers Instance { get { Init(); return s_instacne; } }

    #region Contents 
    private ObjectManager _object = new ObjectManager();
    private GameManager _game = new GameManager();
    private EventManager _event = new EventManager();
    private HeroManager _hero = new HeroManager();
    private EquipmentManager _equipment = new EquipmentManager();
    private SkillManager _skill = new SkillManager();
    private BuffManager _buff = new BuffManager();

    public static ObjectManager Object { get { return Instance?._object; } }
    public static GameManager Game { get { return Instance?._game; } }
    public static EventManager Event { get { return Instance?._event; } }
    public static HeroManager Hero { get { return Instance?._hero; } }
    public static EquipmentManager Equipment { get { return Instance?._equipment; } }
    public static SkillManager Skill { get { return Instance?._skill; } }
    public static BuffManager Buff { get { return Instance?._buff; } }
    #endregion

    #region Core
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();
    private DataManager _data = new DataManager();
    private PoolManager _pool = new PoolManager();

    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static SoundManager Sound { get { return Instance?._sound; } }
    public static UIManager UI { get { return Instance?._ui; } }
    public static DataManager Data { get { return Instance?._data; } }
    public static PoolManager Pool { get { return Instance?._pool; } }

    #endregion

    #region Server
    private BackendManager _backend = new BackendManager();
    private AdManager _ad = new AdManager();
    private IAPManager _iap = new IAPManager();

    public static BackendManager Backend { get { return Instance?._backend; } }
    public static AdManager Ad { get { return Instance?._ad; } }
    public static IAPManager IAP { get { return Instance?._iap; } }
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
            Buff.Init();
        }
    }


    public static void Clear()
    {
        Event.Clear();
        Scene.Clear();
        UI.Clear();
        Object.Clear();
        Pool.Clear();
    }

    private void OnApplicationQuit()
    {
        Buff.SaveBuffData();
    }
}