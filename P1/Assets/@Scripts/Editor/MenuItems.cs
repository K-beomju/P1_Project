using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuItems : MonoBehaviour
{
    [MenuItem("GameSettings/Save Data")]
    static void SaveData()
    {
        Managers.Backend.UpdateAllGameData(callback => {
            if (callback == null)
            {
                Debug.LogWarning("저장 데이터 미존재, 저장할 데이터가 존재하지 않습니다.");
                return;
            }

            if (callback.IsSuccess())
            {
                Debug.Log("저장 성공, 저장에 성공했습니다.");
            }
            else
            {
                Debug.LogWarning($"수동 저장 실패, 수동 저장에 실패했습니다. {callback.ToString()}");
            }

        });
    }
}
