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

    public static ObjectManager Object { get { return Instance?._object; } }
    #endregion

    #region Core
    private ResourceManager _resource = new ResourceManager();

    public static ResourceManager Resource { get { return Instance?._resource; } }
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
