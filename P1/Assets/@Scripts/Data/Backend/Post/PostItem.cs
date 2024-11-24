using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using BackEnd;
using System;
using static Define;
using System.Reflection;

namespace BackendData.Post
{
    // 우편에서 사용하는 차트 
    // 새로운 차트로 우편을 보내고자 할때에는 그에 맞는 로직을 추가해야함.
    public enum ChartType
    {
        forPost
    }
    //===============================================================
    //  우편의 보상에 대한 클래스
    //===============================================================
    public class PostChartItem
    {
        public ChartType chartType { get; private set; }
        public int itemID { get; private set; }
        public float itemCount { get; private set; }
        public string itemName { get; private set; }

        private delegate void ReceiveFunc();

        private ReceiveFunc _receiveFunc = null;

        // PostItem 클래스의 보상을 담당하는 차트 정보를 파싱하는 클래스
        public PostChartItem(JsonData json)
        {
            itemCount = float.Parse(json["itemCount"].ToString());
            Debug.Log(itemCount);
            string chartName = json["chartName"].ToString();

            if (!Enum.TryParse<ChartType>(chartName, out var tempChartType))
            {
                throw new Exception("지정되지 않은 Post ChartType 입니다.");
            }
            chartType = tempChartType;

            // 우편에 부탁된 아이템을 위한 차트를 타입에 맞게 변경 
            try
            {
                switch (chartType)
                {
                    case ChartType.forPost:
                        itemID = int.Parse(json["item"]["ItemID"].ToString());
                        if (itemID == 1)
                        {
                            itemName = "Gold";
                            //Receive 함수 호출 시 해당 델리게이트 호출
                            _receiveFunc = () => { Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Gold, itemCount); };
                        }
                        if (itemID == 2)
                        {
                            itemName = "Dia";
                            //Receive 함수 호출 시 해당 델리게이트 호출
                            _receiveFunc = () => { Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, itemCount); };
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception("PostChartItem : itemID를  파싱하지 못했습니다.\n" + e.ToString());
            }
        }

        public void Receive()
        {
            _receiveFunc.Invoke();
        }
    }

    //===============================================================
    //  Post.Manager 클래스의 GetPostList의 리턴값에 대한 파싱하는 클래스 
    //===============================================================
    public class PostItem
    {
        public readonly string title;
        public readonly string content;
        public readonly DateTime expirationDate;
        public readonly string inDate;
        public readonly PostType PostType;

        public readonly List<PostChartItem> items = new List<PostChartItem>();

        public PostItem(PostType postType, JsonData postListJson)
        {
            PostType = postType;

            content = postListJson["content"].ToString();
            expirationDate = DateTime.Parse(postListJson["expirationDate"].ToString());
            inDate = postListJson["inDate"].ToString();
            title = postListJson["title"].ToString();

            // 우편 보상 데이터 
            if (postListJson["items"].Count > 0)
            {
                for (int itemNum = 0; itemNum < postListJson["items"].Count; itemNum++)
                {
                    PostChartItem item = new PostChartItem(postListJson["items"][itemNum]);
                    items.Add(item);
                }
            }
        }


        // 우편 보상을 받은 후 호출되는 델리게이트 함수
        public delegate void IsReceiveSuccessFunc(bool isSuccess);

        // [뒤끝] 우편 수령 함수
        public void ReceiveItem(IsReceiveSuccessFunc isReceiveSuccessFunc)
        {
            SendQueue.Enqueue(Backend.UPost.ReceivePostItem, PostType, inDate, callback =>
            {
                bool isSuccess = false;
                try
                {
                    Debug.Log($"Backend.UPost.ReceivePostItem({PostType}, {inDate}) : {callback}");

                    // 수령할 경우
                    if (callback.IsSuccess())
                    {
                        isSuccess = true;

                        string postItemString = String.Empty;

                        // 해당 우편이 가지고 있는 item의 Receive함수를 호출하여 보상을 획득 
                        foreach (var item in items)
                        {
                            item.Receive();
                            string itemName = string.Empty;
                            if (item.itemName == "Gold")
                                itemName = "골드";
                            if (item.itemName == "Dia")
                                itemName = "다이아";

                            postItemString += $"{itemName} {item.itemCount}개 획득";
                        }

                        // 보상을 다 얻고 난 다음에는 저장
                        // 우편을 수령할 경우 우편이 제거되기 때문에 업데이트 주기에 게임을 종료하면 우편 수령에 대한 결과는 사라질 수 있다.
                        Managers.Backend.UpdateAllGameData(callback =>
                        {
                            if (callback == null)
                            {
                                Debug.LogWarning("저장 데이터 미존재, 저장할 데이터가 존재하지 않습니다.");
                                return;
                            }

                            if (callback.IsSuccess())
                            {
                                Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification(postItemString);
                                Debug.Log("저장 성공, 저장에 성공했습니다.");
                            }
                            else
                            {
                                Debug.LogWarning($"수동 저장 실패, 수동 저장에 실패했습니다. {callback.ToString()}");
                            }
                        });
                    }
                    else
                    {
                        Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification("우편 수령 실패 에러" + "우편 수령에 실패했습니다.\n" + callback.ToString());
                    }
                }
                catch (Exception e)
                {
                    Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification(GetType().Name + MethodBase.GetCurrentMethod()?.ToString() + e.ToString());
                }
                finally
                {
                    if (isSuccess)
                    {
                        // 수령이 완료될 경우 우편 목록에서 제거
                        Managers.Backend.Post.RemovePost(inDate);
                    }

                    isReceiveSuccessFunc(isSuccess);
                }
            });
        }

    }

}