using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    private void Awake() 
    {
        Managers.Object.Spawn<Box>(new Vector3(10,10,0));    
    }


}
