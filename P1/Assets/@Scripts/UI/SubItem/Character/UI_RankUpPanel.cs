using BackendData.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class UI_RankUpPanel : UI_Base
{
    public enum Texts
    {
        Text_MyRankName,
        Text_FeeAmount,
        Text_ChangeAbilityCount
    }

    public enum Images
    {
        Image_MyRankIcon
    }

    public enum Buttons
    {
        Btn_ChangeAbility
    }

    // 추가 능력 슬롯 
    public enum RankAbility
    {
        UI_RankAbilityItem_Iron,
        UI_RankAbilityItem_Bronze,
        UI_RankAbilityItem_Gold,
        UI_RankAbilityItem_Dia,
        UI_RankAbilityItem_Master,
        UI_RankAbilityItem_GrandMaster
    }

    // 랭킹 도전 아이템 
    public enum RankChallenge
    {
        UI_RankChallengeItem_Iron,
        UI_RankChallengeItem_Bronze,
        UI_RankChallengeItem_Gold,
        UI_RankChallengeItem_Dia,
        UI_RankChallengeItem_Master,
        UI_RankChallengeItem_GrandMaster
    }

    private Coroutine _coolTime;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindButtons(typeof(Buttons));
        Bind<UI_RankAbilityItem>(typeof(RankAbility));
        Bind<UI_RankChallengeItem>(typeof(RankChallenge));

        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Iron).SetInfo(ERankType.Iron);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Bronze).SetInfo(ERankType.Bronze);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Gold).SetInfo(ERankType.Gold);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Dia).SetInfo(ERankType.Dia);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Master).SetInfo(ERankType.Master);
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_GrandMaster).SetInfo(ERankType.GrandMaster);

        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Iron).SetInfo(ERankType.Iron);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Bronze).SetInfo(ERankType.Bronze);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Gold).SetInfo(ERankType.Gold);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Dia).SetInfo(ERankType.Dia);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Master).SetInfo(ERankType.Master);
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_GrandMaster).SetInfo(ERankType.GrandMaster);


        GetButton((int)Buttons.Btn_ChangeAbility).gameObject.BindEvent(OnClickButton, EUIEvent.Pressed);
        GetButton((int)Buttons.Btn_ChangeAbility).gameObject.BindEvent(OnPointerUp, EUIEvent.PointerUp);

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.HeroRankUpdated, new Action(RefreshUI));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.HeroRankUpdated, new Action(RefreshUI));
    }

    private void OnClickButton()
    {
        if (_coolTime != null)
            return;

        List<string> updatedRankKeys = new List<string>(); // 변경된 RankKey 목록

        // 무작위로 랜덤 스탯 추출
        foreach (var rankEntry in Managers.Backend.GameData.RankUpData.RankUpDic)
        {
            AbilityData abilityData = rankEntry.Value;

            // 조건 확인: RankState가 Completed 또는 Current 상태이며, RankAbilityState가 Locked나 Restricted가 아닐 때만 진행
            if ((abilityData.RankState == ERankState.Completed || abilityData.RankState == ERankState.Current) &&
                abilityData.RankAbilityState != ERankAbilityState.Locked &&
                abilityData.RankAbilityState != ERankAbilityState.Restricted)
            {
                AssignRandomAbility(rankEntry.Key, abilityData);
                updatedRankKeys.Add(rankEntry.Key); // 변경된 RankKey 추가
                _coolTime = StartCoroutine(CoStartUpgradeCoolTime(0.1f));

            }
        }

        if (updatedRankKeys.Count > 0)
        {
            RefreshUpdatedUIItems(updatedRankKeys);
        }
    }

    private void OnPointerUp()
    {
        if (_coolTime != null)
        {
            StopCoroutine(_coolTime);
            _coolTime = null;
        }
    }

    private void AssignRandomAbility(string rankKey, AbilityData abilityData)
    {
        // 유효한 스탯 타입만 포함한 배열 생성
        EHeroRankUpStatType[] validStatTypes = Enum.GetValues(typeof(EHeroRankUpStatType))
            .Cast<EHeroRankUpStatType>()
            .Where(statType => statType != EHeroRankUpStatType.None)
            .ToArray();

        ERareType[] validRareTypes = Enum.GetValues(typeof(ERareType))
            .Cast<ERareType>()
            .Where(statType => statType != ERareType.None)
            .ToArray();

        // 랜덤 스탯 타입 추출
        EHeroRankUpStatType randStatType = validStatTypes[UnityEngine.Random.Range(0, validStatTypes.Length)];

        // 딕셔너리에서 rankUpData 찾기
        if (Managers.Data.DrawRankUpChart.TryGetValue(randStatType, out var rankUpData))
        {
            int drawProbabilityIndex = Util.GetDrawProbabilityType(rankUpData.ProbabilityList);
            List<int> valueList = drawProbabilityIndex switch
            {
                0 => rankUpData.NormalValueList,
                1 => rankUpData.AdvanceValueList,
                2 => rankUpData.RareValueList,
                3 => rankUpData.LegendaryValueList,
                4 => rankUpData.MythicalValueList,
                _ => throw new ArgumentException($"Unknown rare type value: {drawProbabilityIndex}")
            };

            ERareType randRareType = validRareTypes[drawProbabilityIndex];
            int selectedValue = valueList[Util.GetDrawProbabilityType(valueList)];
            Managers.Backend.GameData.RankUpData.UpdateAbilityData(rankKey, ERankAbilityState.Acquired, randStatType, randRareType, selectedValue);
            Debug.Log($"{rankKey}에 {rankUpData.Name} {selectedValue} 능력치가 부여되었습니다.");
        }
        else
        {
            Debug.LogWarning($"Key '{randStatType}' not found in DrawRankUpChart.");
        }
    }

    private void RefreshUpdatedUIItems(List<string> updatedRankKeys)
    {
        foreach (var rankKey in updatedRankKeys)
        {
            // RankKey에 따라 해당하는 RankAbility를 찾아 RefreshUI 호출
            if (Enum.TryParse(rankKey, out ERankType rankType))
            {
                UI_RankAbilityItem rankAbilityItem = rankType switch
                {
                    ERankType.Iron => Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Iron),
                    ERankType.Bronze => Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Bronze),
                    ERankType.Gold => Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Gold),
                    ERankType.Dia => Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Dia),
                    ERankType.Master => Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Master),
                    ERankType.GrandMaster => Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_GrandMaster),
                    _ => null
                };

                rankAbilityItem?.RefreshUI();
            }
        }
    }

    public void RefreshUI()
    {
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Iron).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Bronze).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Gold).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Dia).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_Master).RefreshUI();
        Get<UI_RankAbilityItem>((int)RankAbility.UI_RankAbilityItem_GrandMaster).RefreshUI();

        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Iron).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Bronze).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Gold).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Dia).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_Master).RefreshUI();
        Get<UI_RankChallengeItem>((int)RankChallenge.UI_RankChallengeItem_GrandMaster).RefreshUI();

        ERankType rankType = Managers.Backend.GameData.RankUpData.GetCurrentRankType();
        GetImage((int)Images.Image_MyRankIcon).gameObject.SetActive(true);
        if (rankType == ERankType.Unknown)
        {
            GetTMPText((int)Texts.Text_MyRankName).text = "랭크 없음";
            GetImage((int)Images.Image_MyRankIcon).gameObject.SetActive(false);

        }
        else
        {
            GetImage((int)Images.Image_MyRankIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{rankType}");
            GetTMPText((int)Texts.Text_MyRankName).text = Managers.Data.HeroRankUpChart[rankType].Name;
        }
    }

    private IEnumerator CoStartUpgradeCoolTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coolTime = null;
    }
}
