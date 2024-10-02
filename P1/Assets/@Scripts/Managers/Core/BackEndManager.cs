using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

using static Define;
using System;

public class BackEndManager
{
    private BackendGameData _backendGameData = new();
    public void Init()
    {
        var initializeBro = Backend.Initialize();

        if (!initializeBro.IsSuccess())
        {
            Debug.LogError("초기화 실패 : " + initializeBro.ToString()); // 실패일 경우 statusCode 400대 에러 발생
            return;
        }

        Debug.Log("초기화 성공 : " + initializeBro); // 성공일 경우 statusCode 204 Success

        Backend.BMember.CustomLogin("user1", "1234");
        _backendGameData.SaveUserData(10);

    }

    // 차트 ID와 반복 횟수, 연결이 됐을 경우 실행할 함수를 받아 서버 GameData란에
    // 정보를 추가하는 함수 
    public void GameDataInsert(string selectedProbabilityField, int maxRepeatCount,
        Param param, Action<BackendReturnObject> onCompleted = null)
    {
        if (!Backend.IsLogin)
        {
            Debug.LogError("뒤끝에 로그인 되어있지 않습니다.");
            return;
        }

        if (maxRepeatCount <= 0)
        {
            Debug.LogErrorFormat($"{selectedProbabilityField} 게임 정보를 추가하지 못했습니다.");
            return;
        }

        Backend.GameData.Insert(selectedProbabilityField, param, callback =>
        {
            switch (ErrorCheck(callback))
            {
                case EBackendState.Failure:
                    Debug.LogError("연결 실패");
                    break;
                case EBackendState.Maintainance:
                    Debug.LogError("서버 점검 중");
                    break;
                case EBackendState.Retry:
                    Debug.LogWarning("연결 재시도");
                    GameDataInsert(selectedProbabilityField, maxRepeatCount - 1, param, onCompleted);
                    break;
                case EBackendState.Success:
                    Debug.Log("정보 추가 성공");
                    onCompleted?.Invoke(callback);
                    break;
            }
        });
    }

    // 차트 ID와 반복 횟수, 연결이 됐을 경우 실행할 함수를 받아 서버 GameData란에
    // 정보를 수정하는 함수 
    public void GameDataUpdate(string selectedProbabilityField, string inData, 
        int maxRepeatCount, Param param, Action<BackendReturnObject> onCompleted = null)
    {
        if (!Backend.IsLogin)
        {
            Debug.LogError("뒤끝에 로그인 되어있지 않습니다.");
            return;
        }

        if (maxRepeatCount <= 0)
        {
            Debug.LogErrorFormat($"{selectedProbabilityField} 게임 정보를 추가하지 못했습니다.");
            return;
        }

        Backend.GameData.UpdateV2(selectedProbabilityField, inData, Backend.UserInDate,
            param, callback =>
        {
            switch (ErrorCheck(callback))
            {
                case EBackendState.Failure:
                    Debug.LogError("연결 실패");
                    break;
                case EBackendState.Maintainance:
                    Debug.LogError("서버 점검 중");
                    break;
                case EBackendState.Retry:
                    Debug.LogWarning("연결 재시도");
                    GameDataUpdate(selectedProbabilityField, inData, 
                        maxRepeatCount - 1, param, onCompleted);
                    break;
                case EBackendState.Success:
                    Debug.Log("정보 추가 성공");
                    onCompleted?.Invoke(callback);
                    break;
            }
        });
    }


    // 서버와 연결 상태를 체크하고 State값을 반환하는 함수 
    public EBackendState ErrorCheck(BackendReturnObject bro)
    {
        if (bro.IsSuccess())
        {
            Debug.Log("요청 성공");
            return EBackendState.Success;
        }
        else
        {
            if (bro.IsClientRequestFailError())
            {
                Debug.LogError("일시적인 네트워크 끊김");
                return EBackendState.Retry;
            }
            else if (bro.IsServerError())
            {
                Debug.LogError("서버 이상 발생");
                return EBackendState.Retry;
            }
            else if (bro.IsMaintenanceError())
            {
                Debug.Log("게임 점검중");
                return EBackendState.Maintainance;
            }
            else if (bro.IsTooManyRequestError())
            {
                Debug.LogError("단기간에 많은 요청을 보냈습니다.");
                return EBackendState.Failure;
            }
            else if (bro.IsBadAccessTokenError())
            {
                bool isRefreshSuccess = RefreshTheBackendToken(3);

                if (isRefreshSuccess)
                {
                    Debug.LogError("토큰 발급 성공");
                    return EBackendState.Retry;
                }
                else
                {
                    Debug.LogError("토큰을 발급 받지 못하였습니다.");
                    return EBackendState.Failure;
                }

            }
            return EBackendState.Retry;
        }
    }

    /// 뒤끝 토큰 재발급 함수 , maxRepeatCount : 서버 연결 실패시 재 시도할 횟수 
    public bool RefreshTheBackendToken(int maxRepeatCount)
    {
        if (maxRepeatCount <= 0)
        {
            Debug.Log("토큰 발급 실패");
            return false;
        }

        BackendReturnObject callBack = Backend.BMember.RefreshTheBackendToken();

        if (callBack.IsSuccess())
        {
            Debug.Log("토큰 발급 성공");
            return true;
        }
        else
        {
            if (callBack.IsClientRequestFailError())
            {
                return RefreshTheBackendToken(maxRepeatCount - 1);
            }
            else if (callBack.IsServerError())
            {
                return RefreshTheBackendToken(maxRepeatCount - 1);
            }
            else if (callBack.IsMaintenanceError())
            {
                return false;
            }
            else if (callBack.IsTooManyRequestError())
            {
                return false;
            }
            else
            {
                Debug.Log("게임 접속에 문제가 발생했습니다. 로그인 화면으로 돌아감 \n"
                    + callBack.ToString());
                return false;
            }
        }
    }
}
