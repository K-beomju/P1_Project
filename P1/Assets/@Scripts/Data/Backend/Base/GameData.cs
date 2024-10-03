using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BackendData.Base
{
    public abstract class GameData : Normal
    {
        private string _inDate;

        public string GetInDate()
        {
            return _inDate;
        }

        public bool IsChangedData { get; set; }

        public abstract string GetTableName();
        public abstract string GetColumnName();
        public abstract Param GetParam();

        protected abstract void InitializeData();

        protected abstract void SetServerDataToLocal(JsonData gameDataJson);

        public void BackendGameDataLoad(AfterBackendLoadFuc afterBackendLoadFuc) {
            string tableName = GetTableName();
            string columnName = GetColumnName();
            string className = tableName;

            bool isSuccess = false;
            string errorInfo = string.Empty;
            string funcName = MethodBase.GetCurrentMethod()?.Name;

            // [뒤끝] 내 게임정보 불러오기 함수
            SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), callback => {
                try {
                    Debug.Log($"Backend.GameData.GetMyData({tableName}) : {callback}");

                    if (callback.IsSuccess()) {
                        // 불러온 데이터가 하나라도 존재할 경우
                        if (callback.FlattenRows().Count > 0) {
                            // 이후 업데이트에 사용될 각 데이터의 indate값 저장
                            _inDate = callback.FlattenRows()[0]["inDate"].ToString();

                            //Dictionary 등 데이터 저장을 위해 컬럼값을  설정했을 경우
                            if (string.IsNullOrEmpty(columnName)) {
                                // FlattenRows의 경우, 리턴값에 ["S"], ["N"]등의 데이터 타입값을 제거한 후, Json을 리턴
                                SetServerDataToLocal(callback.FlattenRows()[0]);
                            }
                            else {
                                // 설정하지 않았을 경우(UserData)
                                // columnName까지 진입한 후, Json을 리턴
                                SetServerDataToLocal(callback.FlattenRows()[0][columnName]);
                            }

                            isSuccess = true;
                            // 불러오기가 끝난 이후에 호출되는 함수 호출
                            afterBackendLoadFuc(isSuccess, className, funcName, errorInfo);
                        }
                        else {
                            // 불러온 데이터가 없을 경우, 서버에 존재하지 않는 경우
                            // 데이터를 새로 생성
                            BackendInsert(afterBackendLoadFuc);
                        }
                    }
                    else {
                        // 데이터 존재 여부 상관없이 에러가 발생했을 경우(서버에 데이터가 존재할 수도있음)
                        errorInfo = callback.ToString();
                        afterBackendLoadFuc(isSuccess, className, funcName, errorInfo);
                    }
                }
                catch (Exception e) {
                    // 예외가 발생했을 경우
                    // 파싱 실패등
                    errorInfo = e.ToString();
                    afterBackendLoadFuc(isSuccess, className, funcName, errorInfo);
                }
            });
        }

        // 서버에 데이터가 존재하지 않을 경우, 데이터를 새로 삽입
        private void BackendInsert(AfterBackendLoadFuc afterBackendLoadFuc) {
            string tableName = GetTableName();
            bool isSuccess = false;
            string errorInfo = string.Empty;
            string className = string.Empty;
            string funcName = string.Empty;


            // 데이터 초기화(각 자식 객체가 설정)
            InitializeData();

            SendQueue.Enqueue(Backend.GameData.Insert, tableName, GetParam(), callback => {
                try {
                    Debug.Log($"Backend.GameData.Insert({tableName}) : {callback}");

                    if (callback.IsSuccess()) {
                        isSuccess = true;
                        _inDate = callback.GetInDate();
                    }
                    else {
                        errorInfo = callback.ToString();
                    }
                }
                catch (Exception e) {
                    errorInfo = e.ToString();
                }
                finally {
                    afterBackendLoadFuc(isSuccess, className, funcName, errorInfo);
                }
            });
        }

        // 업데이트가 완료된 이후 리턴값과 함께 호출되는 함수
        public delegate void AfterCallBack(BackendReturnObject callback);

        // 해당 테이블에 데이터 업데이트
        public void Update(AfterCallBack afterCallBack) {
            SendQueue.Enqueue(Backend.GameData.UpdateV2, GetTableName(), GetInDate(), Backend.UserInDate, GetParam(),
                callback => {
                    Debug.Log($"Backend.GameData.UpdateV2({GetTableName()}, {GetInDate()}, {Backend.UserInDate}) : {callback}");
                    afterCallBack(callback);
                });
        }

        // 해당 테이블에 업데이트할 데이터 트랜잭션(한번에 여러 테이블 저장)으로 만들어 리턴
        public TransactionValue GetTransactionUpdateValue() {
            return TransactionValue.SetUpdateV2(GetTableName(), GetInDate(), Backend.UserInDate, GetParam());
        }

        // 해당 테이블에 업데이트할 데이터 트랜잭션(한번에 여러 테이블 저장)으로 만들어 리턴
        public TransactionValue GetTransactionGetValue() {
            Where where = new Where();
            where.Equal("owner_inDate", Backend.UserInDate);
            return TransactionValue.SetGet(GetTableName(), where);
        }

        public void BackendGameDataLoadByTransaction(JsonData gameDataJson, AfterBackendLoadFuc afterBackendLoadFuc) {
            string columnName = GetColumnName();
            string errorInfo = string.Empty;
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;

            try {
                _inDate = gameDataJson["inDate"].ToString();

                if(string.IsNullOrEmpty(columnName)) {
                    
                    SetServerDataToLocal(gameDataJson);
                }
                else {
      
                    SetServerDataToLocal(gameDataJson[columnName]);
                }
                afterBackendLoadFuc(true, className, funcName, errorInfo);
            
            }
            catch(Exception e) {
                errorInfo = e.ToString();
                afterBackendLoadFuc(false, className, funcName, errorInfo);
            }
        
        }

    
    }
}