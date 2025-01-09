using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using BackendData.Base;
using UnityEngine;
using static Define;
using Debug = UnityEngine.Debug;



public class BackendManager
{
    public class Policy
    {
        public string terms;
        public string termsURL;
        public string privacy;
        public string privacyURL;
        public override string ToString()
        {
            string str = $"terms : {terms}\n" +
            $"termsURL : {termsURL}\n" +
            $"privacy : {privacy}\n" +
            $"privacyURL : {privacyURL}\n";
            return str;
        }
    }

    //뒤끝 콘솔에 업로드한 차트 데이터만 모아놓은 클래스
    // public class BackendChart {
    //     public readonly BackendData.Chart.AllChart ChartInfo = new(); // 모든 차트
    //     public readonly BackendData.Chart.Stage.Manager Stage = new(); // 스테이지 정보
    //     public readonly BackendData.Chart.Hero.Manager Hero = new(); // 영웅 정보
    //     public readonly BackendData.Chart.HeroUpgrade.Manager HeroUpgrade = new(); // 영웅 업그레이드 스탯 정보
    //     public readonly BackendData.Chart.HeroUpgradeCost.Manager HeroUpgradeCost = new(); // 영웅 업그레이드 가격 정보
    //     public readonly BackendData.Chart.CreatureUpgradeStat.Manager CreatureUpgradeStat = new(); // 생명체 업그레이드 스탯 정보
    //     public readonly BackendData.Chart.Monster.Manager Monster = new(); // 일반 몬스터 정보
    //     public readonly BackendData.Chart.BossMonster.Manager BossMonster = new(); // 보스 몬스터 정보 
    //     public readonly BackendData.Chart.Equipment.Manager Equipment = new(); // 장비 정보
    //     public readonly BackendData.Chart.DrawEquipmentGacha.Manager DrawEquipmentGacha = new(); // 장비 뽑기 확률 정보
    //     public readonly BackendData.Chart.Skill.Manager Skill = new(); // 스킬 정보
    //     public readonly BackendData.Chart.DrawSkillGacha.Manager DrawSkillGacha = new(); // 스킬 뽑기 확률 저옵
    // }

    // 게임 정보 관리 데이터만 모아놓은 클래스
    public class BackendGameData
    {
        public readonly BackendData.GameData.CharacterData CharacterData = new();
        public readonly BackendData.GameData.EquipmentInventory EquipmentInventory = new();
        public readonly BackendData.GameData.SkillInventory SkillInventory = new();
        public readonly BackendData.GameData.PetInventory PetInventory = new();
        public readonly BackendData.GameData.DrawLevelData DrawLevelData = new();
        public readonly BackendData.GameData.DungeonData DungeonData = new();
        public readonly BackendData.GameData.RankUpData RankUpData = new();
        public readonly BackendData.GameData.ShopData ShopData = new();
        public readonly BackendData.GameData.QuestData QuestData = new();
        public readonly BackendData.GameData.MissionData MissionData = new();

        public readonly Dictionary<string, GameData>
            GameDataList = new Dictionary<string, GameData>();

        public BackendGameData()
        {
            GameDataList.Add("내 미션 정보", MissionData);
            GameDataList.Add("내 퀘스트 정보", QuestData);
            GameDataList.Add("내 상점 정보", ShopData);
            GameDataList.Add("내 랭크 정보", RankUpData);
            GameDataList.Add("내 던전 정보", DungeonData);
            GameDataList.Add("내 장비 정보", EquipmentInventory);
            GameDataList.Add("내 스킬 정보", SkillInventory);
            GameDataList.Add("내 펫 정보", PetInventory);
            GameDataList.Add("내 뽑기 정보", DrawLevelData);
            GameDataList.Add("내 유저 정보", CharacterData);
        }
    }

    //public BackendChart Chart = new(); // 차트 모음 클래스 생성
    public BackendGameData GameData = new(); // 게임 데이터 관리 클래스 생성 
    public BackendData.Rank.Manager Rank = new(); // 랭킹 관리 클래스 생성
    public BackendData.Post.Manager Post = new(); // 우편 클래스 생성 

    // 치명적인 에러 발생 여부
    private bool _isErrorOccured = false;


    public void Init()
    {

        var initalizeBro = Backend.Initialize();

        if (initalizeBro.IsSuccess())
        {
            Debug.Log("뒤끝 초기화가 완료되었습니다.");
            //이선영
            //Backend.BMember.CustomLogin("user1", "1234");
            //김범주
            //Backend.BMember.CustomLogin("5dxwin", "owen2602");
            //우지호
            //Backend.BMember.CustomLogin("uziho", "1234");
            //제주옥탑
            //Backend.BMember.CustomLogin("jejuRooftop", "1234");

            CreateSendQueueMgr();
            SetErrorHandler();
        }
        else
        {
            Debug.LogError("초기화 실패");
        }
    }



    // 모든 뒤끝 함수에서 에러 발생 시, 각 에러에 따라 호출해주는 핸들러
    private void SetErrorHandler()
    {
        // 서버 점검 에러 발생 시
        Backend.ErrorHandler.OnMaintenanceError = () =>
        {
            Debug.LogError("점검 에러 발생!!!");
        };
        // 403 에러 발생시
        Backend.ErrorHandler.OnTooManyRequestError = () =>
        {
            Debug.LogError("비정상적인 행동 감지 " + "비정상적인 행동이 감지되었습니다.\n타이틀로 돌아갑니다.");
        };
        // 액세스토큰 만료 후 리프레시 토큰 실패 시
        Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () =>
        {
            Debug.LogError("다른 기기 접속 감지 " + "다른 기기에서 로그인이 감지되었습니다.\n타이틀로 돌아갑니다.");
        };
    }

    // 로딩씬에서 할당할 뒤끝 정보 클래스 초기화
    public void InitInGameData()
    {
        GameData = new();
        Rank = new();
        Post = new();
    }

    //SendQueue를 관리해주는 SendQueue 매니저 생성
    private void CreateSendQueueMgr()
    {
        var obj = new GameObject();
        obj.name = "SendQueueMgr";
        obj.transform.SetParent(GameObject.Find("@Managers").transform);
        obj.AddComponent<SendQueueMgr>();
    }

    // 호출 시, 코루틴 내 함수들의 동작을 멈추게 하는 함수
    public void StopUpdate()
    {
        Debug.Log("자동 저장을 중지합니다.");
        _isErrorOccured = true;
    }

    // 일정주기마다 데이터를 저장/불러오는 코루틴 시작(인게임 시작 시)
    public IEnumerator UpdateGameDataTransaction()
    {
        var seconds = new WaitForSeconds(300);
        yield return seconds;

        while (!_isErrorOccured)
        {
            UpdateAllGameData(callback =>
            {
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
            yield return seconds;
        }

    }

    public float saveDataCoolTime { get; private set; }
    public bool IsSaveCoolTimeActive => saveDataCoolTime > 0;

    // 수동으로 데이터를 저장하는 코루틴
    public void StartSaveCoolTime(Action onCoolDownComplete)
    {
        if (IsSaveCoolTimeActive)
            return;

        Managers.Instance.StartCoroutine(SaveCoolTimeCoroutine(onCoolDownComplete));
    }

    private IEnumerator SaveCoolTimeCoroutine(Action onCoolDownComplete)
    {
        saveDataCoolTime = 5;
        while (saveDataCoolTime > 0)
        {
            saveDataCoolTime -= Time.deltaTime;
            yield return null;
        }
        saveDataCoolTime = 0; // 정확한 쿨타임 종료를 보장
        onCoolDownComplete?.Invoke();
    }

    // 업데이트가 발생한 이후에 호출에 대한 응답을 반환해주는 대리자 함수
    public delegate void AfterUpdateFunc(BackendReturnObject callback);

    // 값이 바뀐 데이터가 있는지 체크후 바뀐 데이터들은 바로 저장 혹은 트랜잭션에 묶어 저장을 진행하는 함수
    public void UpdateAllGameData(AfterUpdateFunc afterUpdateFunc)
    {
        if (Managers.Backend.GameData.MissionData.GetCurrentMission() != null)
        {
            Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification("튜토리얼을 완료한 후 저장이 가능합니다");
            return;
        }

        string info = string.Empty;

        // 바뀐 데이터가 몇개 있는지 체크
        List<GameData> gameDatas = new List<GameData>();

        foreach (var gameData in GameData.GameDataList)
        {
            if (gameData.Value.IsChangedData)
            {
                info += gameData.Value.GetTableName() + "\n";
                gameDatas.Add(gameData.Value);
            }
        }

        if (gameDatas.Count <= 0)
        {
            afterUpdateFunc(null); // 지정한 대리자 함수 호출

            Debug.Log("업데이트할 목록이 존재하지 않습니다.");
        }
        else if (gameDatas.Count == 1)
        {

            //하나라면 찾아서 해당 테이블만 업데이트
            foreach (var gameData in gameDatas)
            {
                if (gameData.IsChangedData)
                {
                    gameData.Update(callback =>
                    {

                        //성공할경우 데이터 변경 여부를 false로 변경
                        if (callback.IsSuccess())
                        {
                            gameData.IsChangedData = false;
                        }
                        else
                        {
                            SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString() + "\n" + info);
                        }
                        Debug.Log($"UpdateV2 : {callback}\n업데이트 테이블 : \n{info}");
                        if (afterUpdateFunc == null)
                        {

                        }
                        else
                        {
                            afterUpdateFunc(callback); // 지정한 대리자 함수 호출
                        }
                    });
                }
            }
        }
        else
        {
            // 2개 이상이라면 트랜잭션에 묶어서 업데이트
            // 단 10개 이상이면 트랜잭션 실패 주의
            List<TransactionValue> transactionList = new List<TransactionValue>();
            // 변경된 데이터만큼 트랜잭션 추가
            foreach (var gameData in gameDatas)
            {
                transactionList.Add(gameData.GetTransactionUpdateValue());
                Debug.LogWarning(transactionList);
            }

            SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, transactionList, callback =>
            {
                Debug.Log($"Backend.BMember.TransactionWriteV2 : {callback}");

                if (callback.IsSuccess())
                {
                    foreach (var data in gameDatas)
                    {
                        data.IsChangedData = false;
                    }
                }
                else
                {
                    SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString() + "\n" + info);
                }

                Debug.LogWarning($"TransactionWriteV2 : {callback}\n업데이트 테이블 : \n{info}");

                if (afterUpdateFunc == null)
                {

                }
                else
                {
                    afterUpdateFunc(callback);  // 지정한 대리자 함수 호출
                }
            });
        }

        PlayerPrefs.Save();
    }

    // 단일 게임 데이터를 저장하는 함수
    public void UpdateSingleGameData(GameData gameData, AfterUpdateFunc afterUpdateFunc)
    {
        // 데이터가 변경되었는지 확인
        if (gameData.IsChangedData)
        {
            gameData.Update(callback =>
            {
                // 성공 시, 변경된 데이터 여부를 false로 변경
                if (callback.IsSuccess())
                {
                    gameData.IsChangedData = false;
                    Debug.Log($"Single Game Data Updated Successfully: {gameData.GetTableName()}");
                }
                else
                {
                    // 실패 시, 버그 리포트 발송
                    SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString());
                    Debug.LogWarning($"Failed to update single game data: {gameData.GetTableName()}");
                }

                // 지정한 대리자 함수 호출
                afterUpdateFunc?.Invoke(callback);
            });
        }
        else
        {
            Debug.Log("No changes detected in the specified game data. Update skipped.");
            afterUpdateFunc?.Invoke(null);
        }
    }

    // 일정 주기마다 랭킹 데이터 업데이트 호출 
    public IEnumerator UpdateRankScore()
    {
        var seconds = new WaitForSeconds(600);
        yield return seconds;

        while (!_isErrorOccured)
        {
            if (Managers.Backend.GameData.CharacterData.WorldBossCombatPower != 0)
            {
                foreach (var li in Rank.List)
                {
                    Debug.Log("랭킹 업데이트 주기");
                    UpdateUserRankScore(li.uuid, callback =>
                    {
                        if (callback == null)
                        {
                            Debug.LogWarning("랭킹 데이터 미존재, 저장할 랭킹 데이터가 존재하지 않습니다.");
                            return;
                        }

                        if (callback.IsSuccess())
                        {
                            Debug.Log("랭킹 성공, 랭킹에 성공했습니다.");
                            Managers.Event.TriggerEvent(EEventType.MyRankingUpdated);
                        }
                        else
                        {
                            Debug.LogWarning($"랭킹 저장 실패, 수동 저장에 실패했습니다. {callback.ToString()}");
                        }
                    });
                }
            }
            yield return seconds;
        }

    }

    public void UpdateUserRankScore(string uuid, AfterUpdateFunc afterUpdateFunc)
    {
        // 쓰기 비용의 부담이 클 경우에는 각 랭킹별로 Param을 업데이트 하도록 설정. (현재는 일괄 처리)
        // 바뀐 데이터가 몇개 있는지 체크 
        List<GameData> gameDatas = new List<GameData>();

        foreach (var gameData in GameData.GameDataList)
        {
            gameDatas.Add(gameData.Value);
        }

        foreach (var li in Rank.List)
        {
            //업데이트하고자 하는 uuid 존재하는지 확인
            if (li.uuid.Equals(uuid))
            {
                // 랭크 리스트에 있는 테이블 이름과 현재 테이블 이름이 있는지 확인하고 존재한다면 해당 게임 테이블을 전체 업데이트 
                int index = gameDatas.FindIndex(item => item.GetTableName().Equals(li.table));
                if (index < 0)
                {
                    afterUpdateFunc?.Invoke(null);
                }
                SendQueue.Enqueue(Backend.URank.User.UpdateUserScore, li.uuid, li.table,
                    gameDatas[index].GetInDate(), gameDatas[index].GetParam(),
                    callback =>
                    {
                        Debug.Log($"Backend.URank.User.UpdateUserScore({li.uuid}, {li.table}, {gameDatas[index].GetInDate()}) : {callback}");
                        if (!callback.IsSuccess())
                        {
                            SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString());
                        }

                        if (afterUpdateFunc != null)
                        {
                            afterUpdateFunc.Invoke(callback);
                        }
                    });
            }
        }

    }

    // 일정 주기마다 우편을 불러오는 코루틴 함수 
    public IEnumerator GetAdminPostList()
    {
        var seconds = new WaitForSeconds(600);
        yield return seconds;

        while (!_isErrorOccured)
        {
            // 현재 post 함수 체크 
            int postCount = Post.Dictionary.Count;

            //랭크보상은 수동으로 체크하도록 구성
            // 관리자 우편은 자동으로 일정 주기마다 호출하도록 구성

            Post.GetPostList(PostType.Admin, (success, info) =>
            {
                if (success)
                {
                    //호출하기 전 우편의 갯수와 동일하지 않다면 우편 아이콘 오른쪽에 표시 
                    if (postCount != Post.Dictionary.Count)
                    {
                        if (Post.Dictionary.Count > 0)
                        {
                            //FindObjectOfType<InGameScene.RightButtonGroupManager>().SetPostIconAlert(true);
                        }
                    }
                }
                else
                {
                    //에러가 발생할 경우 버그 리포트 발송
                    SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), info);
                }
            });
            yield return seconds;

        }
    }

    // 게임 플레이 타임 코루틴 함수
    public IEnumerator UpdateGamePlayTime()
    {
        var seconds = new WaitForSeconds(1);
        yield return seconds;

        while (!_isErrorOccured)
        {
            Managers.Backend.GameData.QuestData.UpdateQuest(EQuestType.PlayTime);
            Managers.Event.TriggerEvent(EEventType.QuestItemUpdated);
            yield return seconds;
        }
    }

    // 에러 발생시 게임로그를 삽입하는 함수
    public void SendBugReport(string className, string functionName, string errorInfo, int repeatCount = 3)
    {


        // 에러가 실패할 경우 재귀함수를 통해 최대 3번까지 호출을 시도한다.
        if (repeatCount <= 0)
        {
            return;
        }

        // 아직 로그인되지 않을 경우 뒤끝 함수 호출이 불가능하여 UI에 띄운다.
        if (string.IsNullOrEmpty(Backend.UserInDate))
        {
            return;
        }

        Param param = new Param();
        param.Add("className", className);
        param.Add("functionName", functionName);
        param.Add("errorPath", errorInfo);

        // [뒤끝] 로그 삽입 함수
        Backend.GameLog.InsertLogV2("error", param, 7, callback =>
        {
            // 에러가 발생할 경우 재귀
            if (callback.IsSuccess() == false)
            {
                SendBugReport(className, functionName, errorInfo, repeatCount - 1);
            }
        });
    }

    
    #region Logout
    //         GetButton((int)Buttons.Btn_Logout).onClick.AddListener(() =>
    //         {
    //             SendQueue.Enqueue(Backend.BMember.Logout, callback =>
    //             {
    //                 Debug.Log($"Backend.BMember.Logout : {callback}");
    // #if UNITY_ANDROID
    //                 TheBackend.ToolKit.GoogleLogin.Android.GoogleSignOut(true, GoogleSignOutCallback);
    // #endif

    //                 if (callback.IsSuccess())
    //                 {
    //                     Debug.Log("로그아웃 성공");
    //                     Managers.Scene.LoadScene(EScene.TitleScene);
    //                 }
    //             });
    //         });
    #endregion
}
