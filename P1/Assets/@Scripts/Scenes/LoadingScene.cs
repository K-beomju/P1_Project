using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BackEnd;
using LitJson;
using static Define;
using System.Linq;
using System;

public class LoadingScene : BaseScene       
{
    private UI_LoadingScene sceneUI;


    private int _maxLoadingCount;
    private int _currentLoadingCount;

    private delegate void BackendLoadStep();
    private readonly Queue<BackendLoadStep> _initializeStep = new Queue<BackendLoadStep>();


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.LoadingScene;
        Managers.Scene.SetCurrentScene(this);

        Managers.Backend.Init();

        if (Backend.IsInitialized == false) {
            Debug.LogError("뒤끝 초기화가 안됌");
        }


        Managers.Data.Init();


        sceneUI = Managers.UI.ShowSceneUI<UI_LoadingScene>();
        Managers.UI.SetCanvas(sceneUI.gameObject, false, SortingLayers.UI_SCENE);
        return true;
    }

    private void Start()
    {
        //Queue에 함수 Insert
        InitStep();

        // 뒤끝 데이터 초기화
        Managers.Backend.InitInGameData();

        //Queue에 저장된 함수 순차적으로 실행
        NextStep(true, string.Empty, string.Empty, string.Empty);

    }

    private void InitStep()
    {
        _initializeStep.Clear();

        // 트랜잭션으로 불러온 후, 안불러질 경우 각자 Get 함수로 불러오는 함수 *중요*
        _initializeStep.Enqueue(() => { ShowDataName("트랜잭션 시도 함수"); TransactionRead(NextStep); });
        // 랭킹 정보 불러오기 함수 Insert
        _initializeStep.Enqueue(() => { ShowDataName("랭킹 정보 불러오기"); Managers.Backend.Rank.BackendLoad(NextStep); });
        
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
        if(isSuccess) {
            _currentLoadingCount++;
            sceneUI.ShowDataSlider(_currentLoadingCount, _maxLoadingCount);

            if(_initializeStep.Count > 0) {
                _initializeStep.Dequeue().Invoke();
            }
            else {
                InGameStart();
            }
        }
        else {
            Debug.LogError($"{className} + {funcName} + {errorInfo}");
        }
    }

    // 트랜잭션 읽기 호출 함수
    private void TransactionRead(BackendData.Base.Normal.AfterBackendLoadFunc func) {
        bool isSuccess = false;
        string className = GetType().Name;
        string functionName = MethodBase.GetCurrentMethod()?.Name;
        string errorInfo = string.Empty;

        //트랜잭션 리스트 생성
        List<TransactionValue> transactionList = new List<TransactionValue>();

        // 게임 테이블 데이터만큼 트랜잭션 불러오기
        foreach (var gameData in Managers.Backend.GameData.GameDataList) {
            transactionList.Add(gameData.Value.GetTransactionGetValue());
        }

        // [뒤끝] 트랜잭션 읽기 함수
        SendQueue.Enqueue(Backend.GameData.TransactionReadV2, transactionList, callback => { 
            try { 
                Debug.Log($"Backend.GameData.TransactionReadV2 : {callback}");

                // 데이터를 모두 불러왔을 경우
                if (callback.IsSuccess()) {
                    JsonData gameDataJson = callback.GetFlattenJSON()["Responses"];

                    int index = 0;

                    foreach (var gameData in Managers.Backend.GameData.GameDataList) {
                        _initializeStep.Enqueue(() => {
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
                else {
                    // 트랜잭션으로 데이터를 찾지 못하여 에러가 발생한다면 개별로 GetMyData로 호출
                    foreach (var gameData in Managers.Backend.GameData.GameDataList) { 
                        _initializeStep.Enqueue(() => {
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
            catch(Exception e) {
                errorInfo = e.ToString();
            }
            finally { 
                func.Invoke(isSuccess, className, functionName, errorInfo);
            }
        });
    }


    // 인게임씬으로 이동가는 함수
    private void InGameStart()
    {
        sceneUI.ShowDataName("게임 시작하는 중");
        _initializeStep.Clear();
        Managers.Scene.LoadScene(EScene.GameScene);

    }
}

