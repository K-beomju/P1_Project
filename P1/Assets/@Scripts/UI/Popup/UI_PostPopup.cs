using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Reflection;

public class UI_PostPopup : UI_Popup
{
    public enum Buttons 
    {
        Btn_Exit
    }

    public enum GameObjects 
    {
        Text_NoPostAlert,
        PostContent
    }

    private Dictionary<string, GameObject> _postItemDictionary = new();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        return true;
    }


    public void RefreshUI()
    {
        // 우편함에 우편이 없을 경우 텍스트 출력
        GetObject((int)GameObjects.Text_NoPostAlert).SetActive(Managers.Backend.Post.Dictionary.Count <= 0);

        // 우편 목록 조회 후 결과값을 이용하여 포스트 아이템 생성 
        Managers.Backend.Post.GetPostList(PostType.Rank, (success, info) => 
        {
            if (success)
            {
                foreach (var list in Managers.Backend.Post.Dictionary)
                {
                    // indate가 중복일 경우에는 패스 
                    if(_postItemDictionary.ContainsKey(list.Value.inDate))
                        continue;

                    // 아이템 생성, 아이템의 수령 버튼에 RemovePost 함수 설정 
                    UI_PostItem postItem = Managers.UI.MakeSubItem<UI_PostItem>(GetObject((int)GameObjects.PostContent).transform);
                    postItem.RefreshUI(list.Value, RemovePost);
                    _postItemDictionary.Add(list.Value.inDate, postItem.gameObject);
                }
            }
            else
            {
                Managers.UI.ShowBaseUI<UI_NotificationBase>(GetType().Name + " " + MethodBase.GetCurrentMethod()?.ToString() + " " + info);
            }
        });




    }

    // 우편 아이템의 수령 버튼 클릭시 호출 
    // UI 리스트에서 해당 우편 제거 
    private void RemovePost(string inDate)
    {
        if(_postItemDictionary.ContainsKey(inDate))
        {
            Destroy(_postItemDictionary[inDate]);
            _postItemDictionary.Remove(inDate);
        }
    }
}
