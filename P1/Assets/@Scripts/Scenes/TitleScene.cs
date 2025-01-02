using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Android;
using static Define;

public class TitleScene : BaseScene
{
    private UI_TitleScene sceneUI;

    private int _maxLoadingCount;
    private int _currentLoadingCount;

    private delegate void BackendLoadStep();
    private readonly Queue<BackendLoadStep> _initializeStep = new Queue<BackendLoadStep>();

    #region 푸시 알림 권한
    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        Debug.Log($"{permissionName} 권한이 거부되었습니다.");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        Debug.Log($"{permissionName} 권한이 부여되었습니다.");
    }

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.Log($"{permissionName} 권한이 거부되었으며 '다시 묻지 않음'이 활성화되었습니다.");
    }


    private void RequestNotificationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;

            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS", callbacks);
        }
        else
        {
            Debug.Log("POST_NOTIFICATIONS 권한이 이미 부여되었습니다.");
        }
    }
    #endregion

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Android 13(API Level 33) 이상에서 POST_NOTIFICATIONS 권한 요청
        if (Application.platform == RuntimePlatform.Android && SystemInfo.operatingSystem.Contains("API-33"))
        {
            RequestNotificationPermission();
        }

        // 씬 설정 
        SceneType = EScene.TitleScene;
        Managers.Scene.SetCurrentScene(this);

        // 뒤끝 초기화 
        Managers.Backend.Init();
        if (Backend.IsInitialized == false)
        {
            Debug.LogError("뒤끝 초기화가 안됌");
        }
        //Backend.Android.AgreeNightPushNotification(true);

        // 광고와 엑셀 데이터 
        Managers.Data.Init();
        Managers.Ad.Init();

        Managers.Sound.Init();
        sceneUI = Managers.UI.ShowSceneUI<UI_TitleScene>();
        return true;
    }


    public void InitBackendDataLoad()
    {
        //Queue에 함수 Insert
        InitStep();

        // 뒤끝 데이터 초기화
        Managers.Backend.InitInGameData();

        //Queue에 저장된 함수 순차적으로 실행
        NextStep(true, string.Empty, string.Empty, string.Empty);
    }

    private void InitStep(Action<bool> onCompleteCallback = null)
    {
        _initializeStep.Clear();

        // 트랜잭션으로 불러온 후, 안불러질 경우 각자 Get 함수로 불러오는 함수 *중요*
        _initializeStep.Enqueue(() => { ShowDataName("트랜잭션 시도 함수"); TransactionRead(NextStep); });
        // 랭킹 정보 불러오기 함수 Insert
        _initializeStep.Enqueue(() => { ShowDataName("랭킹 정보 불러오기"); Managers.Backend.Rank.BackendLoad(NextStep); });
        // 우편 정보 불러오기 함수 Insert
        _initializeStep.Enqueue(() => { ShowDataName("관리자 우편 정보 불러오기"); Managers.Backend.Post.BackendLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("랭킹 우편 정보 불러오기"); Managers.Backend.Post.BackendLoadForRank(NextStep); });
        _maxLoadingCount = _initializeStep.Count;
        _currentLoadingCount = 0;
        sceneUI.ShowDataSlider(_currentLoadingCount, _maxLoadingCount);

    }

    public override void Clear()
    {
        _initializeStep.Clear();
    }

    public void ShowDataName(string text)
    {
        string info = $"{text} 불러오는 중...({_currentLoadingCount}/{_maxLoadingCount})";
        sceneUI.ShowDataName(info);
    }

    private void NextStep(bool isSuccess, string className, string funcName, string errorInfo)
    {
        if (isSuccess)
        {
            _currentLoadingCount++;
            sceneUI.ShowDataSlider(_currentLoadingCount, _maxLoadingCount);

            if (_initializeStep.Count > 0)
            {
                _initializeStep.Dequeue().Invoke();
            }
            else
            {
                sceneUI.ShowTouchToStart();
            }
        }
        else
        {
            Debug.LogError($"{className} + {funcName} + {errorInfo}");
        }
    }

    // 트랜잭션 읽기 호출 함수
    private void TransactionRead(BackendData.Base.Normal.AfterBackendLoadFunc func)
    {
        bool isSuccess = false;
        string className = GetType().Name;
        string functionName = MethodBase.GetCurrentMethod()?.Name;
        string errorInfo = string.Empty;

        //트랜잭션 리스트 생성
        List<TransactionValue> transactionList = new List<TransactionValue>();

        // 게임 테이블 데이터만큼 트랜잭션 불러오기
        foreach (var gameData in Managers.Backend.GameData.GameDataList)
        {
            transactionList.Add(gameData.Value.GetTransactionGetValue());
        }

        // [뒤끝] 트랜잭션 읽기 함수
        SendQueue.Enqueue(Backend.GameData.TransactionReadV2, transactionList, callback =>
        {
            try
            {
                Debug.Log($"Backend.GameData.TransactionReadV2 : {callback}");

                // 데이터를 모두 불러왔을 경우
                if (callback.IsSuccess())
                {
                    JsonData gameDataJson = callback.GetFlattenJSON()["Responses"];

                    int index = 0;

                    foreach (var gameData in Managers.Backend.GameData.GameDataList)
                    {
                        _initializeStep.Enqueue(() =>
                        {
                            ShowDataName(gameData.Key);
                            // 불러온 데이터를 로컬에서 파싱
                            gameData.Value.BackendGameDataLoadByTransaction(gameDataJson[index++], NextStep);
                        });
                        _maxLoadingCount++;
                    }
                    // 최대 작업 개수 증가
                    sceneUI.ShowDataSlider(_currentLoadingCount, _maxLoadingCount);
                    isSuccess = true;

                }
                else
                {
                    // 트랜잭션으로 데이터를 찾지 못하여 에러가 발생한다면 개별로 GetMyData로 호출
                    foreach (var gameData in Managers.Backend.GameData.GameDataList)
                    {
                        _initializeStep.Enqueue(() =>
                        {
                            ShowDataName(gameData.Key);
                            // GetMyData 호출
                            gameData.Value.BackendGameDataLoad(NextStep);
                        });
                        _maxLoadingCount++;
                    }
                    sceneUI.ShowDataSlider(_currentLoadingCount, _maxLoadingCount);
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                errorInfo = e.ToString();
            }
            finally
            {
                func.Invoke(isSuccess, className, functionName, errorInfo);
            }
        });
    }


    // 인게임씬으로 이동가는 함수
    public void InGameStart()
    {
        // 코루틴을 통한 정기 데이터 업데이트 시작
        Managers.Instance.UpdateBackendData();
        _initializeStep.Clear();

        Managers.Buff.Init();
        Managers.Scene.LoadScene(EScene.GameScene);
    }

}
