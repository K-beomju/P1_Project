using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_NicknamePopup : UI_Popup
{
    private TMP_InputField _nickNameInputField;
    private Button _nickNameCreateButton;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _nickNameInputField = Util.FindChild(gameObject, "InputField_Nickname", true)
        .GetComponent<TMP_InputField>();
        _nickNameCreateButton = Util.FindChild(gameObject, "Btn_CreateNickName", true)
        .GetComponent<Button>();

        if (_nickNameCreateButton != null)
            _nickNameCreateButton.onClick.AddListener(OnButtonClickCreateNickName);

        return true;
    }

    private void OnButtonClickCreateNickName()
    {
        string nickName = _nickNameInputField.text;

        if (string.IsNullOrEmpty(nickName))
        {
            Managers.UI.ShowBaseUI<UI_NotificationBase>("닉네임이 비어있습니다.");
            return;
        }

        //[뒤끝] 닉네임 업데이트 함수 
        SendQueue.Enqueue(Backend.BMember.UpdateNickname, nickName, callback =>
        {
            try
            {
                if (IsBackendError(callback)) {
                    if(callback.GetStatusCode() == "400") {
                        if (callback.GetMessage().Contains("undefined nickname")) {
                            ShowAlertUI($"닉네임이 비어있습니다.");

                        }
                        else if (callback.GetMessage().Contains("bad nickname is too long")) {
                            ShowAlertUI($"20자 이상은 입력할 수 없습니다.");

                        }
                        else if (callback.GetMessage().Contains("bad beginning or end")) {
                            ShowAlertUI($"닉네임이 앞 혹은 뒤에 공백이 존재합니다");
                        }
                        else  {
                            ShowAlertUI($"알 수 없는 에러입니다.");
                        }
                    }
                }
                else {
                    //Managers.Scene.LoadScene(EScene.LoadingScene);
                }
            }
            catch (Exception e)
            {
                ShowAlertUI(e.ToString());
            }
        });
    }

}
