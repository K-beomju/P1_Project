using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Data;

public class UI_RankChallengeItem : UI_Base
{
    public enum Images
    {
        Image_ClassIcon,
        Image_Fade
    }

    public enum Texts
    {
        Text_ClassName,
        Text_ClassPassiveAbility,
        Text_RdLevel
    }

    public enum Buttons
    {
        Btn_ChallengeClass
    }

    private ERankType _rankType;
    private ERankState _rankState;
    private RankUpInfoData _rankData;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Btn_ChallengeClass).onClick.AddListener(OnButtonClick);
        return true;
    }

    private void OnButtonClick()
    {
        // 도전 가능한 상태인지 확인
        if (_rankState == ERankState.Pending)
        {
            Debug.LogWarning($"{_rankType} 승급전 도전합니다.");
            Managers.Backend.GameData.RankUpData.UpdateRankUp(_rankType);
        }
    }

    public void SetInfo(ERankType rankType)
    {
        _rankType = rankType;
        _rankData = Managers.Data.RankUpChart[rankType];
    }

    public void RefreshUI()
    {
        GetImage((int)Images.Image_ClassIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{_rankType}");
        GetTMPText((int)Texts.Text_ClassName).text = _rankData.Name;
        GetTMPText((int)Texts.Text_ClassPassiveAbility).text = _rankData.PassiveName;
        GetTMPText((int)Texts.Text_RdLevel).text = $"권장레벨 {_rankData.RdLevel}";


        GetImage((int)Images.Image_Fade).gameObject.SetActive(true);
        GetButton((int)Buttons.Btn_ChallengeClass).gameObject.SetActive(true);
        GetButton((int)Buttons.Btn_ChallengeClass).interactable = true;

        _rankState = Util.ParseEnum<ERankState>(Managers.Backend.GameData.RankUpData.RankUpDic[_rankType.ToString()].RankState.ToString());
        switch (_rankState)
        {
            case ERankState.Locked: // 잠겨있는 상태
                GetButton((int)Buttons.Btn_ChallengeClass).interactable = false;
                GetImage((int)Images.Image_Fade).gameObject.SetActive(true);
                break;
            case ERankState.Completed:  // 이미 깬 상태
                GetButton((int)Buttons.Btn_ChallengeClass).gameObject.SetActive(false);
                GetImage((int)Images.Image_Fade).gameObject.SetActive(false);
                break;
            case ERankState.Current: // 현재 상태
                GetButton((int)Buttons.Btn_ChallengeClass).gameObject.SetActive(false);
                GetImage((int)Images.Image_Fade).gameObject.SetActive(false);
                break;
            case ERankState.Pending: // 진행해야 할 상태
                GetButton((int)Buttons.Btn_ChallengeClass).interactable = true;
                GetImage((int)Images.Image_Fade).gameObject.SetActive(true);
                break;
        }

    }
}
