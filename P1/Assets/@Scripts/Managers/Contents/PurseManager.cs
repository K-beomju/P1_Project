using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PurseManager
{


    // UserData와 관련된 UI와 서버에 저장될 데이터를 변경하는 함수
    // 해당 함수를 통해서만 UserData 변경 가능
    public void AddAmount(EGoodType goodType, int amount)
    {
        try {

            // 조정된 획득량만큼 GameData의 UserData 업데이트
            BackendManager.Instance.GameData.UserData.AddAmount(goodType, amount);

            // 변경된 데이터에 맞게 UserUI 변경(우측 상단)
            Managers.Event.TriggerEvent(EEventType.CurrencyUpdated);
        }
        catch(Exception e) {
            throw new Exception($"AddAmount({goodType}, {amount}) 중 에러가 발생하였습니다\n{e}");
        }
    }

    public float GetAmount(EGoodType goodType)
    {
        BackendManager.Instance.GameData.UserData.PurseDic.TryGetValue(goodType.ToString(), out float amount);
        return amount;
    }

    public void AddExp(int exp)
    {
        BackendManager.Instance.GameData.UserData.AddExp(exp);
    }

}
