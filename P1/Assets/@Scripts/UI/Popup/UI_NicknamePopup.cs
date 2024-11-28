using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_NicknamePopup : UI_Popup
{
    private TMP_InputField _nickNameInputField;
    private Button _nickNameCreateButton;

    private string[] _prohibitedWords; // 비속어 리스트
    private string _lineSplitRegex = @"\r\n|\n\r|\n|\r";

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

        // 비속어 파일 로드
        LoadProhibitedWords();


        return true;
    }

    private void LoadProhibitedWords()
    {
        // Resources 폴더에서 BadWord.txt 로드
        TextAsset badWordFile = Resources.Load<TextAsset>("BadWord");
        if (badWordFile != null)
        {
            try
            {
                string content = badWordFile.text;
                _prohibitedWords = Regex.Split(content, _lineSplitRegex);
            }
            catch (Exception e)
            {
                Debug.LogError($"비속어 파일 로드 중 오류 발생: {e}");
                _prohibitedWords = new string[0]; // 비속어 리스트를 비움
            }
        }
        else
        {
            Debug.LogWarning("비속어 파일이 Resources 폴더에 존재하지 않습니다.");
            _prohibitedWords = new string[0];
        }
    }

    private void OnButtonClickCreateNickName()
    {
        string nickName = _nickNameInputField.text;

        if (string.IsNullOrEmpty(nickName))
        {
            ShowAlertUI("닉네임이 비어있습니다.");
            return;
        }

        // 비속어 검사
        if (IsProhibitedWord(nickName))
        {
            ShowAlertUI("비속어는 사용할 수 없습니다.");
            return;
        }

        // 특수문자 검사
        string filteredNickName = Regex.Replace(nickName, @"[^a-zA-Z0-9가-힣 ]", "");
        if (!nickName.Equals(filteredNickName))
        {
            ShowAlertUI("특수문자는 사용할 수 없습니다.");
            return;
        }

        // 1~10자 검사
        if (nickName.Length < 1 || nickName.Length > 10)
        {
            ShowAlertUI("닉네임의 길이는\n 1자 이상 10자 이하여야 합니다.");
            return;
        }


        //[뒤끝] 닉네임 업데이트 함수 
        SendQueue.Enqueue(Backend.BMember.UpdateNickname, nickName, callback =>
        {
            try
            {
                if (IsBackendError(callback))
                {
                    if (callback.GetStatusCode() == "400")
                    {
                        if (callback.GetMessage().Contains("undefined nickname"))
                        {
                            ShowAlertUI($"닉네임이 비어있습니다.");

                        }
                        else if (callback.GetMessage().Contains("bad beginning or end"))
                        {
                            ShowAlertUI($"닉네임이 앞 혹은 뒤에 공백이 존재합니다");
                        }
                        else
                        {
                            ShowAlertUI($"알 수 없는 에러입니다.");
                        }
                    }
                }
                else
                {
                    Managers.Scene.GetCurrentScene<TitleScene>().InGameStart();
                }
            }
            catch (Exception e)
            {
                ShowAlertUI(e.ToString());
            }
        });
    }

    private bool IsProhibitedWord(string nickName)
    {
        foreach (string word in _prohibitedWords)
        {
            if (nickName.Contains(word))
                return true;
        }
        return false;
    }

}
