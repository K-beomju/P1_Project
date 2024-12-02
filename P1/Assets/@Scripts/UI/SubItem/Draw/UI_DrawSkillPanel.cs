using BackendData.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_DrawSkillPanel : UI_Base
{
    public enum Texts
    {
        Text_DrawSkillLevel,
        Text_DrawValue,
        Text_AdWatchedDrawSkillCount
    }

    public enum Sliders
    {
        Slider_DrawCount
    }

    public enum Buttons
    {
        Btn_GachaProbability,
        Btn_SkipDrawVisual,
        Btn_DrawTenAd,
        Btn_DrawTen,
        Btn_DrawThirty,
    }

    public enum Toggles
    {
        Toggle_DrawDirection
    }

    private DrawData _drawData;
    private EDrawType _drawType = EDrawType.Skill;

    private int _drawLevel;
    private int _totalCount;
    private bool _drawDirection = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindButtons(typeof(Buttons));
        Bind<Toggle>(typeof(Toggles));

        GetButton((int)Buttons.Btn_GachaProbability).onClick.AddListener(() => ShowProbabilityPopup());

        GetButton((int)Buttons.Btn_DrawTenAd).onClick.AddListener(() => 
        {
            Managers.Ad.ShowRewardedInterstitialAd(onRewardEarned => 
            {
                if(onRewardEarned)
                {
                    // 광고 시청 처리
                    Managers.Backend.GameData.ShopData.WatchAd(EAdRewardType.DrawSkill);

                    // 보상 지급 
                    OnDrawSkill(10);

                    // UI 업데이트 
                    RefreshUI();
                }
                else
                {
                    Debug.LogWarning("광고 시청 실패!");
                }
            });
        });
        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawSkill(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawSkill(30));

        // 버튼 클릭 시 Toggle의 값을 변경합니다.
        Toggle drawDirectionToggle = Get<Toggle>((int)Toggles.Toggle_DrawDirection);
        GetButton((int)Buttons.Btn_SkipDrawVisual).onClick.AddListener(() => drawDirectionToggle.isOn = !drawDirectionToggle.isOn);
        drawDirectionToggle.onValueChanged.AddListener((bool isOn) => _drawDirection = isOn);

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.DrawSkillUIUpdated, new Action(RefreshUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.DrawSkillUIUpdated, new Action(RefreshUI));
    }

    public void RefreshUI()
    {
        _drawData = Managers.Backend.GameData.DrawLevelData.DrawDic[_drawType.ToString()];

        if (_drawData == null)
        {
            Debug.LogWarning("장비 게임 데이터가 없음");
            return;
        }
        _drawLevel = _drawData.DrawLevel;
        _totalCount = _drawData.DrawCount;

        GetTMPText((int)Texts.Text_DrawSkillLevel).text = $"스킬 뽑기 Lv. {_drawLevel}";
        GetTMPText((int)Texts.Text_DrawValue).text = $"{_totalCount} / {Managers.Data.DrawEquipmentChart[_drawLevel].MaxExp}";

        GetSlider((int)Sliders.Slider_DrawCount).maxValue = Managers.Data.DrawSkillChart[_drawLevel].MaxExp;
        GetSlider((int)Sliders.Slider_DrawCount).value = _totalCount;

        var rewardAdDic = Managers.Backend.GameData.ShopData.RewardAdDic;
        GetTMPText((int)Texts.Text_AdWatchedDrawSkillCount).text = 
        $"({rewardAdDic[EAdRewardType.DrawSkill.ToString()].WatchedCount}/{RewardAdData.MaxCount})";
    }



    #region Draw Logic

    private void OnDrawSkill(int drawCount)
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DrawResultPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_RESULTPOPUP);

        var skillIdList = Util.GetDrawSystemResults(_drawType, drawCount, _drawLevel);
        popupUI.RefreshUI(_drawType, drawCount, skillIdList, _drawDirection);
    }

    private void ShowProbabilityPopup()
    {
        Managers.UI.ShowPopupUI<UI_DrawProbabilityPopup>().RefreshUI(_drawType);
    }

    #endregion
}
