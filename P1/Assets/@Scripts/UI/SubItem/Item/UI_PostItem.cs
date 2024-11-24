using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UI_PostItem : UI_Base
{
    public enum Texts
    {
        Text_PostTitleText,
        Text_ExpirationDateText,
        Text_PostAmount
    }

    public enum Buttons
    {
        Button_Receive
    }

    public enum Images
    {
        Image_PostIcon
    }

    private BackendData.Post.PostItem _postItem;

    public delegate void ReceivePostFunc(string inDate);

    private ReceivePostFunc _receivePostFunc;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));

        GetButton((int)Buttons.Button_Receive).onClick.AddListener(OnButtonClick);
        return true;
    }

    // List에 있는 PostItem의 데이터를 이용하여 우편 아이템 생성, 우편 수령 시 성공할 경우 UI에서 우편 제거하는 버튼 연결
    public void RefreshUI(BackendData.Post.PostItem postItem, ReceivePostFunc func)
    {
        try
        {
            _postItem = postItem;

            GetTMPText((int)Texts.Text_PostTitleText).text = _postItem.title;
            GetTMPText((int)Texts.Text_PostAmount).text = "x" + Util.ConvertToTotalCurrency(int.Parse(_postItem.content));
            GetImage((int)Images.Image_PostIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Item/{_postItem.items[0].itemName}");
            GetTMPText((int)Texts.Text_ExpirationDateText).text = _postItem.expirationDate.ToString() + "까지";

            // if ((_postItem.expirationDate - DateTime.UtcNow).Days > 0)
            // {
            //     GetTMPText((int)Texts.Text_ExpirationDateText).text = (_postItem.expirationDate - DateTime.UtcNow).Days + "일 남음";
            // }
            // else
            // {
            //     GetTMPText((int)Texts.Text_ExpirationDateText).text = (_postItem.expirationDate - DateTime.UtcNow).Hours + "시간 남음";
            // }
            _receivePostFunc = func;
        }
        catch (Exception e)
        {
            throw new Exception($"{GetType().Name} : {MethodBase.GetCurrentMethod()?.ToString()} : {e.ToString()}");
        }
    }

    // 아이템을 받는 함수
    private void OnButtonClick()
    {
        try
        {
            // PostItem 객체에서 우편 받기 함수 수령후 결과값 전송
            _postItem.ReceiveItem((isSuccess) =>
            {
                if (isSuccess)
                {
                    // 성공 시 우편 UI 제거
                    _receivePostFunc.Invoke(_postItem.inDate);
                }
            });
        }
        catch (Exception e)
        {
            Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification(GetType().Name + " " + MethodBase.GetCurrentMethod()?.ToString() + " " + e.ToString());
        }
    }
}
