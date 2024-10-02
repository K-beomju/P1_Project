using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

using static Define;


public class UserData
{
    public int Int = 1;
    public float Float = 3.3f;
    public string Str = string.Empty;
    public Dictionary<string, int> Dic = new Dictionary<string, int>()
    {
        { "asd", 1 },
        { "sd", 1 },
    };
    public List<string> List = new List<string>();
}

public class BackendGameData
{
    public UserData UserData = new();

    public void SaveUserData(int maxRepeatCount)
    {
        string selectedProbabilityField = "UserData";
        if (!Backend.IsLogin)
        {
            Debug.LogError("뒤끝에 로그인 되어있지 않습니다.");
            return;
        }

        if (maxRepeatCount <= 0)
        {
            Debug.LogErrorFormat("{0} 차트의 정보를 받아오지 못했습니다. ");
            return;
        }

        BackendReturnObject bro = Backend.GameData.Get(selectedProbabilityField, new Where());

        switch (Managers.BackEnd.ErrorCheck(bro))
        {
            case EBackendState.Failure:
                Debug.LogError("초기화 실패");

                break;

            case EBackendState.Maintainance:
                Debug.LogError("서버 점검 중");
                break;
            case EBackendState.Retry:
                Debug.LogWarning("연결 재시도");
                break;
            case EBackendState.Success:

                // 게임 정보DB에 자신의 정보가 존재할 경우 
                if (bro.GetReturnValuetoJSON() != null)
                {
                    // DB에는 존재하나 그 속 데이터량이 0줄일 경우 
                    if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
                    {
                        // 게임 정보DB에 자신의 데이터를 추가 
                        InsertUserData(selectedProbabilityField);
                    }
                    else
                    {
                        // 게임 정보DB에 있는 자신의 데이터 갱신 
                        UpdateUserData(selectedProbabilityField, bro.GetInDate());
                    }
                }
                else // 게임 정보DB에 자신의 정보가 없을 경우 
                {
                    // 게임 정보DB에 자신의 데이터를 추가 
                    InsertUserData(selectedProbabilityField);
                }

                Debug.LogFormat($"{selectedProbabilityField} 정보를 저장했습니다");
                break;
        }
    }

    // 게임 정보DB에 자신의 데이터를 추가하는 함수 
    public void InsertUserData(string selectedProbabilityField)
    {
        Param param = GetUserDataParam();
        if(param == null)
        {
            Debug.LogError("게임 정보 데이터가 안불러와짐");
            return;
        }
        Debug.LogFormat("게임 정보 데이터 삽입 요청");
        Managers.BackEnd.GameDataInsert(selectedProbabilityField, 10, param);
    }

    // 게임 정보DB에 존재하는 자신의 데이터를 갱신하는 함수 
    public void UpdateUserData(string selectedProbabilityField, string inDate) 
    {
        Param param = GetUserDataParam();
        if (param == null)
        {
            Debug.LogError("게임 정보 데이터가 안불러와짐");
            return;
        }
        Debug.LogFormat("게임 정보 데이터 수정 요청");
        Managers.BackEnd.GameDataUpdate(selectedProbabilityField, inDate, 10, param);
    }

    // 서버에 전달할 유저 정보를 가진 Param 클래스를 반환하는 함수 
    public Param GetUserDataParam()
    {
        Param param = new Param();
        param.Add("Int", UserData.Int);
        param.Add("Float", UserData.Float);
        param.Add("Str", UserData.Str);
        param.Add("Dic", UserData.Dic);
        param.Add("List", UserData.List);
        return param;
    }

}
