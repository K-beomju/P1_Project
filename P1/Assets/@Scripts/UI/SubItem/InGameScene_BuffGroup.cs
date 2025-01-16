using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class InGameScene_BuffGroup : UI_Base
{   
    public enum DisplayAdBuffItems
    {
        UI_DisplayAdBuffItem_1,
        UI_DisplayAdBuffItem_2,
        UI_DisplayAdBuffItem_3
    }

    public Image adBuffImage;
    public List<UI_DisplayAdBuffItem> _displayAdBuffList { get; set; } = new List<UI_DisplayAdBuffItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_DisplayAdBuffItem>(typeof(DisplayAdBuffItems));
        for (int i = 0; i < 3; i++)
        {
            _displayAdBuffList.Add(Get<UI_DisplayAdBuffItem>(i));
            _displayAdBuffList[i].gameObject.SetActive(false);
        }


        LoadExistingBuffs();
        Managers.Buff.OnBuffTimeUpdated += UpdateBuffUI;
        Managers.Buff.OnBuffExpired += RemoveBuffUI;
        return true;
    }

    private void OnDestroy()
    {
        Managers.Buff.OnBuffTimeUpdated -= UpdateBuffUI;
        Managers.Buff.OnBuffExpired -= RemoveBuffUI;
        Managers.Event.RemoveEvent(EEventType.UpdateAdBuffItem, new Action<EAdBuffType, int>(UpdateAdBuffItem));
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.UpdateAdBuffItem, new Action<EAdBuffType,int>(UpdateAdBuffItem));
    }

    private void LoadExistingBuffs()
    {
        foreach (EAdBuffType buffType in Enum.GetValues(typeof(EAdBuffType)))
        {
            int remainingTime = Managers.Buff.GetRemainingTime(buffType);
            if (remainingTime > 0)
            {
                UpdateAdBuffItem(buffType, remainingTime);
            }
        }
    }

    public void UpdateAdBuffItem(EAdBuffType buffType, int durationMinutes)
    {
        foreach (var buffUI in _displayAdBuffList)
        {
            if (!buffUI.gameObject.activeSelf)
            {
                buffUI.gameObject.SetActive(true);
                buffUI.SetInfo(buffType, durationMinutes);
                break;
            }
        }
        RotateAdBuffIcon();
    }

    public void RotateAdBuffIcon()
    {
        if (Managers.Buff.IsAnyBuffActive())
        {
            adBuffImage.transform.DORotate(
                new Vector3(0, 0, -360), // Z축을 기준으로 360도 회전
                1f,                     // 1초 동안 회전
                RotateMode.FastBeyond360 // 빠르게 연속 회전
            )
            .SetEase(Ease.InOutSine)        // 일정한 속도로 회전
            .SetLoops(-1);               // 무한 루프
        }
        else
        {
            // 애니메이션을 멈추거나 초기화
            adBuffImage.transform.DOKill(); // DOTween 애니메이션 중지
            adBuffImage.transform.rotation = Quaternion.identity; // 초기화
        }
    }

    private void UpdateBuffUI(EAdBuffType buffType)
    {
        foreach (var buffUI in _displayAdBuffList)
        {
            if (buffUI.BuffType == buffType)
            {
                buffUI.UpdateRemainingTimeText();
                break;
            }
        }
    }

    private void RemoveBuffUI(EAdBuffType buffType)
    {
        foreach (var buffUI in _displayAdBuffList)
        {
            if (buffUI.gameObject.activeSelf && buffUI.BuffType == buffType)
            {
                buffUI.gameObject.SetActive(false);
                break;
            }
        }
        RotateAdBuffIcon();
    }

}
