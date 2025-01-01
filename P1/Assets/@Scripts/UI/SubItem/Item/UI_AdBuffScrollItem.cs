using BackendData.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_AdBuffScrollItem : UI_Base
{
    public enum Buttons
    {
        Btn_TakeBuff
    }

    public enum Texts
    {
        Text_Amount
    }

    private EAdBuffType _buffType;
    private EAdRewardType _rewardType;
    private const int DurationTimeMinutes = 30;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_TakeBuff).onClick.AddListener(OnClickButton);
        RefreshUI();
        return true;
    }

    private void OnClickButton()
    {
        if (IsBuffActive())
        {
            ShowAlertUI("버프 효과를 사용중입니다");
            return;
        }

        if (Managers.Backend.GameData.ShopData.IsCheckWatch(_rewardType))
        {
            Managers.Ad.ShowRewardedInterstitialAd(onRewardEarned =>
            {
                if (onRewardEarned)
                {
                    StartCoroutine(Managers.Ad.ExecuteAfterFrame(() =>
                    {
                        string name = Util.GetAdBuffType(_buffType);

                        if(_buffType == EAdBuffType.IncreaseGold)
                        Managers.Backend.GameData.MissionData.UpdateMission(EMissionType.AdBuffGold);

                        ShowAlertUI($"{name} 버프를 사용합니다");

                        // 광고 시청 처리
                        Managers.Backend.GameData.ShopData.WatchAd(_rewardType);

                        // 보상 지급 
                        (Managers.UI.SceneUI as UI_GameScene).UpdateAdBuffItem(_buffType, DurationTimeMinutes);

                        // UI 업데이트 
                        RefreshUI();
                    }));
                }
                else
                {
                    Debug.LogWarning("광고 시청 실패!");
                }
            });
        }
        else
        {
            ShowAlertUI("광고 시청 횟수가 모두 소진되었습니다");
        }
    }

    public void SetInfo(EAdBuffType buffType, EAdRewardType rewardType)
    {
        _buffType = buffType;
        _rewardType = rewardType;

        if (Init() == false)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        RewardAdData rewardData = Managers.Backend.GameData.ShopData.RewardAdDic[_rewardType.ToString()];
        GetTMPText((int)Texts.Text_Amount).text =
        $"({rewardData.WatchedCount}/{rewardData.MaxCount})";
    }

    private bool IsBuffActive()
    {
        switch (_buffType)
        {
            case EAdBuffType.Atk:
                return Managers.Hero.PlayerHeroInfo.AtkBuff != 0;
            case EAdBuffType.IncreaseGold:
                return Managers.Hero.PlayerHeroInfo.GoldRateBuff != 0;
            case EAdBuffType.IncreaseExp:
                return Managers.Hero.PlayerHeroInfo.ExpRateBuff != 0;
            default:
                return false;
        }
    }
}
