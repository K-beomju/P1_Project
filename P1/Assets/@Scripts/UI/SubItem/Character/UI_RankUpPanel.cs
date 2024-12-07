using BackendData.GameData;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class UI_RankUpPanel : UI_Base
{
    public enum GameObjects
    {
        BestOptionAlert,
        BestOptionBG
    }

    public enum Texts
    {
        Text_MyRankName,
        Text_FeeAmount,
        Text_ChangeAbilityCount
    }

    public enum Images
    {
        Image_MyRankIcon,
        Image_ChallengingRank
    }

    public enum Buttons
    {
        Btn_ChangeAbility,
        Btn_RestAbility,
        Btn_ExitBestRankOption,
        Btn_RankUpDesc
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
    private bool _bestOption;

    protected override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));
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

        // Rest Option
        GetObject((int)GameObjects.BestOptionAlert).gameObject.SetActive(false);
        GetButton((int)Buttons.Btn_RestAbility).gameObject.BindEvent(() =>
        {
            // BestOptionAlert 창 닫기
            GetObject((int)GameObjects.BestOptionAlert).SetActive(false);
            _bestOption = false;

            // 능력 변경 함수 호출
            ResetAbility();
        });
        GetButton((int)Buttons.Btn_ExitBestRankOption).gameObject.BindEvent(() =>
        {
            GetObject((int)GameObjects.BestOptionAlert).SetActive(false);
            _bestOption = false;
        });
        GetObject((int)GameObjects.BestOptionBG).gameObject.BindEvent(() =>
        {
            GetObject((int)GameObjects.BestOptionAlert).SetActive(false);
            _bestOption = false;
        });
        GetImage((int)Images.Image_ChallengingRank).gameObject.SetActive(false);


        // Desc
        GetButton((int)Buttons.Btn_RankUpDesc).onClick.AddListener(() =>
        {
            UI_RankUpProbabilityPopup popupUI = Managers.UI.ShowPopupUI<UI_RankUpProbabilityPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
            popupUI.RefreshUI();
        });
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.HeroRankUpdated, new Action(RefreshUI));
        Managers.Event.AddEvent(EEventType.HeroRankChallenging, new Action<bool>(IsChallengingRank));

    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.HeroRankUpdated, new Action(RefreshUI));
        Managers.Event.RemoveEvent(EEventType.HeroRankChallenging, new Action<bool>(IsChallengingRank));

    }


    private void OnClickButton()
    {
        if (_coolTime != null)
        {
            return;
        }
        
        bool containHighest = ContainsHighestRankAbility();
        if (containHighest)
        {
            _bestOption = true;
            GetObject((int)GameObjects.BestOptionAlert).SetActive(true);
            return;
        }

        ProcessAbilityChange((abilityData, rankKey) =>
        {
            AssignRandomAbility(rankKey, abilityData);
            _coolTime = StartCoroutine(CoStartUpgradeCoolTime(0.2f));
        });
    }

    private void ResetAbility()
    {
        ProcessAbilityChange((abilityData, rankKey) =>
        {
            _bestOption = false;
            AssignRandomAbility(rankKey, abilityData);
        });
    }

    private void ProcessAbilityChange(Action<AbilityData, string> actionOnAbility)
    {
        int price = GetCheckCountAbilityInventory() * 5;
        if (Managers.Backend.GameData.CharacterData.PurseDic[EItemType.AbilityPoint.ToString()] >= price)
        {
            List<string> updatedRankKeys = new();

            foreach (KeyValuePair<string, AbilityData> rankEntry in Managers.Backend.GameData.RankUpData.RankUpDic)
            {
                AbilityData abilityData = rankEntry.Value;

                if (CanChangeAbility(abilityData))
                {
                    actionOnAbility.Invoke(abilityData, rankEntry.Key);
                    updatedRankKeys.Add(rankEntry.Key);
                }
            }

            if (updatedRankKeys.Count > 0 && !_bestOption)
            {
                RefreshUpdatedUIItems(updatedRankKeys);
            }

            if (!_bestOption)
            {
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.AbilityPoint, -price);
            }
        }
        else
        {
            Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification("어빌리티 포인트가 부족합니다.");
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
        EHeroRankUpStatType randStatType = validStatTypes[Random.Range(0, validStatTypes.Length)];

        // 딕셔너리에서 rankUpData 찾기
        if (Managers.Data.DrawRankUpChart.TryGetValue(randStatType, out DrawRankUpGachaInfoData rankUpData))
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
            Managers.Backend.GameData.RankUpData.UpdateAbilityData(rankKey, ERankAbilityState.Acquired, randStatType,
                randRareType, selectedValue);
            //Debug.Log($"{rankKey}에 {rankUpData.Name} {selectedValue} 능력치가 부여되었습니다.");
        }
        else
        {
            Debug.LogWarning($"Key '{randStatType}' not found in DrawRankUpChart.");
        }
    }

    private void RefreshUpdatedUIItems(List<string> updatedRankKeys)
    {
        foreach (string rankKey in updatedRankKeys)
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
        IsChallengingRank(Managers.Scene.GetCurrentScene<GameScene>().GameSceneState == EGameSceneState.RankUp);
        CheckCountAbilityInventory();
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

        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Current);
        GetImage((int)Images.Image_MyRankIcon).gameObject.SetActive(true);
        if (rankType == ERankType.Unknown)
        {
            GetTMPText((int)Texts.Text_MyRankName).text = "랭크 없음";
            GetImage((int)Images.Image_MyRankIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/Unranked");
        }
        else
        {
            GetImage((int)Images.Image_MyRankIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{rankType}");
            GetTMPText((int)Texts.Text_MyRankName).text = Managers.Data.RankUpChart[rankType].Name;
        }
    }

    private void IsChallengingRank(bool flag)
    {
        GetImage((int)Images.Image_ChallengingRank).gameObject.SetActive(flag);
    }

    private void CheckCountAbilityInventory()
    {
        int changeCount = GetCheckCountAbilityInventory();
        // 아무 것도 변경할 게 없을 때 
        GetButton((int)Buttons.Btn_ChangeAbility).gameObject.SetActive(changeCount != 0);
        GetTMPText((int)Texts.Text_ChangeAbilityCount).text = $"변경 x {changeCount}";
        GetTMPText((int)Texts.Text_FeeAmount).text = (changeCount * 5).ToString();
    }

    private int GetCheckCountAbilityInventory()
    {
        int changeCount = 0;
        foreach (KeyValuePair<string, AbilityData> rankEntry in Managers.Backend.GameData.RankUpData.RankUpDic)
        {
            AbilityData abilityData = rankEntry.Value;

            if (CanChangeAbility(abilityData))
            {
                changeCount++;
            }
        }

        return changeCount;
    }
    
    private bool ContainsHighestRankAbility()
    {
        foreach (KeyValuePair<string, AbilityData> rankEntry in Managers.Backend.GameData.RankUpData.RankUpDic)
        {
            AbilityData abilityData = rankEntry.Value;

            if (CanChangeAbility(abilityData))
            {
                // 최상위 등급인지 확인
                if (abilityData.RareType == ERareType.Mythical)
                {
                    return true; // 최상위 등급 발견
                }
            }
        }
        return false; // 최상위 등급이 없음
    }
    
    private bool CanChangeAbility(AbilityData abilityData)
    {
        return (abilityData.RankState == ERankState.Completed || abilityData.RankState == ERankState.Current) &&
               abilityData.RankAbilityState != ERankAbilityState.Locked &&
               abilityData.RankAbilityState != ERankAbilityState.Restricted;
    }

    private IEnumerator CoStartUpgradeCoolTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coolTime = null;
    }
}