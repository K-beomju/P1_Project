using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BuffManager
{
    private Dictionary<EAdBuffType, int> _buffRemainingTime = new Dictionary<EAdBuffType, int>();
    private Dictionary<EAdBuffType, Coroutine> _buffCoroutines = new Dictionary<EAdBuffType, Coroutine>();

    public event Action<EAdBuffType> OnBuffTimeUpdated;
    public event Action<EAdBuffType> OnBuffExpired;

    public void Init()
    {

        bool missionClear = Managers.Backend.GameData.MissionData.GetCurrentMission() == null;
        foreach (EAdBuffType buffType in Enum.GetValues(typeof(EAdBuffType)))
        {
            // `PlayerPrefs`에서 저장된 남은 시간을 불러옵니다.
            int savedTime = PlayerPrefs.GetInt($"Buff_{buffType}", 0);
            Debug.Log($"Buff_{buffType} : {savedTime}");


            if (savedTime > 0)
            {
                if (missionClear)
                {
                    // `savedTime`을 `Dictionary`에 설정
                    _buffRemainingTime[buffType] = savedTime;
                }
                else
                {
                    PlayerPrefs.DeleteKey($"Buff_{buffType}");
                    Managers.Backend.GameData.ShopData.ResetAdData();
                }
            }

        }
    }

    public void StartBuff(EAdBuffType buffType, int durationMinutes)
    {
        // 이미 남은 시간이 설정된 경우 초기화하지 않고 남은 시간을 유지
        if (!_buffRemainingTime.ContainsKey(buffType) || _buffRemainingTime[buffType] <= 0)
        {
            _buffRemainingTime[buffType] = durationMinutes;
        }

        // 이미 코루틴이 실행 중인 경우 새로운 코루틴을 시작하지 않음
        if (_buffCoroutines.ContainsKey(buffType))
            return;

        // 새로운 코루틴 시작 및 Dictionary에 추가
        Coroutine buffCoroutine = Managers.Instance.StartCoroutine(UpdateBuffTime(buffType));
        _buffCoroutines[buffType] = buffCoroutine;

        Managers.Hero.PlayerHeroInfo.ApplyAdBuff(buffType);
        Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
    }

    public int GetRemainingTime(EAdBuffType buffType)
    {
        return _buffRemainingTime.ContainsKey(buffType) ? _buffRemainingTime[buffType] : 0;
    }

    public void RemoveBuff(EAdBuffType buffType)
    {
        if (_buffRemainingTime.ContainsKey(buffType))
            _buffRemainingTime.Remove(buffType);

        // 코루틴이 실행 중이면 중지하고 제거
        if (_buffCoroutines.ContainsKey(buffType))
        {
            Managers.Instance.StopCoroutine(_buffCoroutines[buffType]);
            _buffCoroutines.Remove(buffType);
        }

        Managers.Hero.PlayerHeroInfo.RemoveAdBuff(buffType);
        Managers.Hero.PlayerHeroInfo.CalculateInfoStat();

        PlayerPrefs.SetInt($"Buff_{buffType}", 0);
        PlayerPrefs.Save();
    }

    private IEnumerator UpdateBuffTime(EAdBuffType buffType)
    {
        WaitForSeconds delay = new WaitForSeconds(60);
        while (_buffRemainingTime.ContainsKey(buffType) && _buffRemainingTime[buffType] > 0)
        {
            yield return delay; // 1분마다 감소
            _buffRemainingTime[buffType]--;

            // UI 업데이트를 위한 이벤트 호출
            OnBuffTimeUpdated?.Invoke(buffType);

            // 시간이 다 되면 버프 해제
            if (_buffRemainingTime[buffType] <= 0)
            {
                OnBuffExpired?.Invoke(buffType);
                RemoveBuff(buffType);
            }

            PlayerPrefs.SetInt($"Buff_{buffType}", 0);
            PlayerPrefs.Save();
        }
    }

    // 앱 종료 시 남은 버프 시간을 저장
    public void SaveBuffData()
    {
        foreach (var buff in _buffRemainingTime)
        {
            PlayerPrefs.SetInt($"Buff_{buff.Key}", buff.Value);
        }
        PlayerPrefs.Save();
    }

    public bool IsAnyBuffActive()
    {
        foreach (var buffTime in _buffRemainingTime.Values)
        {
            if (buffTime > 0)
                return true;
        }
        return false;
    }
}
