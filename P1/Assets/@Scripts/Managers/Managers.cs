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
    private PetManager _pet = new PetManager();

    public static ObjectManager Object { get { return Instance?._object; } }
    public static GameManager Game { get { return Instance?._game; } }
    public static EventManager Event { get { return Instance?._event; } }
    public static HeroManager Hero { get { return Instance?._hero; } }
    public static EquipmentManager Equipment { get { return Instance?._equipment; } }
    public static SkillManager Skill { get { return Instance?._skill; } }
    public static BuffManager Buff { get { return Instance?._buff; } }
    public static PetManager Pet { get { return Instance?._pet; } }
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
    //private IAPManager _iap = new IAPManager();

    public static BackendManager Backend { get { return Instance?._backend; } }
    public static AdManager Ad { get { return Instance?._ad; } }
    //public static IAPManager IAP { get { return Instance?._iap; } }
    #endregion

    public static bool IsCheckIdleTime { get; set; } = false;


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

    public void UpdateBackendData()
    {
        StartCoroutine(Backend.UpdateGameDataTransaction());
        StartCoroutine(Backend.GetAdminPostList());
        StartCoroutine(Backend.UpdateRankScore());
        StartCoroutine(Backend.UpdateGamePlayTime());
    }


    // 게임 시작 후 최초 1회 호출 
    // 조건 : 오프라인 1시간 이상 경과 시, 모든 미션 완료 시 지급
    public void ProcessLoginReward()
    {
        if(!IsCheckIdleTime)
        {
            bool completeMission = Backend.GameData.MissionData.GetCurrentMission() != null;
            if(completeMission)
            {
                Debug.LogWarning("미션을 완료하지 않아 방치 보상 없음");
                return;
            }

            Backend.GameData.CharacterData.UpdateIdleTime();


            if (Backend.GameData.CharacterData.AttendanceCheck())
            {
                Debug.Log("하루가 지나 출석체크 팝업 On");
                var popupUI = UI.ShowPopupUI<UI_AttendancePopup>();
                UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SETTING_CONTENT_POPUP);
                popupUI.RefreshUI();
            }

            IsCheckIdleTime = true;
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
        PlayerPrefs.Save();
    }
}