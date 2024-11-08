using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class UI_RankingPopup : UI_Popup
{
    public enum GameObjects
    {
        RankContent
    }

    public enum Buttons 
    {
        Btn_Exit
    }

    public enum Texts 
    {
        Text_MyRank
    }

    private UI_RankingItem _myRankItem;
    private List<UI_RankingItem> _userRankingItemList = new List<UI_RankingItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        _myRankItem = Util.FindChild<UI_RankingItem>(gameObject, "UI_MyRankingItem", true);
        GetButton((int)Buttons.Btn_Exit).gameObject.BindEvent(() => (Managers.UI.SceneUI as UI_GameScene).ShowTab(UI_GameScene.PlayTab.Rank));

        for (int i = 0; i < 10; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_RankingItem>(GetObject((int)GameObjects.RankContent).transform);
            _userRankingItemList.Add(item.GetOrAddComponent<UI_RankingItem>());
        }
        return true;
    }

    public void RefreshUI()
    {
        ChangeRankList(0);
    }

    // uuid에 따라 랭킹을 불러오는 함수.
    // GetRankList 함수에서 n분 이하일 경우에는 서버에서 데이터를 불러오지 않고 현재 가지고 있는 데이터로 즉시 전달한다.
    private void ChangeRankList(int index)
    {
        // n분이 지날경우 서버 불러온 후 callback 함수 실행, 지나지 않았을 경우 캐싱된 데이터 리턴
        Managers.Backend.Rank.List[index].GetRankList((isSuccess, list) => {
            if (isSuccess)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    _userRankingItemList[i].gameObject.SetActive(true);
                    _userRankingItemList[i].RefreshUI(list[i]);

                    if(i > _userRankingItemList.Count)
                    {
                        break;
                    }
                }

                // 지정한 10개보다 데이터가 적을 경우에는 남은 데이터 안보이게 
                for (int i = list.Count; i < _userRankingItemList.Count; i++)
                {
                    _userRankingItemList[i].gameObject.SetActive(false);
                }

                UpdateMyRank(index);
            }
            else
            {
                Debug.LogWarning("랭킹이 정상적으로 로드되지 않았습니다.");
            }
        });
    }

    // 내 랭킹 불러오기 함수 
    // 만약 랭킹에 없을 경우, Update 시도 
    private void UpdateMyRank(int index) 
    {
        Managers.Backend.Rank.List[index].GetMyRank((isSuccess, myRank) => 
        {
            if(isSuccess)
            {
                if(myRank != null)
                {
                    _myRankItem.RefreshUI(myRank);
                    GetTMPText((int)Texts.Text_MyRank).text = $"개인 순위 {myRank.rank}";
                }
                else
                {
                    Managers.Backend.SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(),"myRank is null");
                }
            }
            else
            {
                Debug.LogWarning("랭킹이 정상적으로 로드되지 않았습니다.");
            }
        });
    }
}
