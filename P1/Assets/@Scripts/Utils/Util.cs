using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEditor.VersionControl;
using UnityEngine;
using static Define;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static Color HexToColor(string color)
    {
        if (color.Contains("#") == false)
            color = $"#{color}";

        ColorUtility.TryParseHtmlString(color, out Color parsedColor);

        return parsedColor;
    }

    #region Exp System
    public static int CalculateRequiredExp(int level)
    {
        // 레벨에 따른 경험치 증가를 더 부드럽게 조정
        float baseExp = 100f; // 초기 경험치 요구량을 더 높게 설정
        float growthFactor = 1.1f; // 경험치 증가율을 완만하게 설정

        return Mathf.RoundToInt(baseExp * Mathf.Pow(growthFactor, level - 1)); // 레벨에 따른 경험치 요구량 증가

    }
    #endregion

    #region Upgrade System 
    public static int GetUpgradeCost(EHeroUpgradeType upgradeType, int level)
    {
        List<int> ReferenceLevelList = Managers.Data.HeroUpgradeCostInfoDataDic[upgradeType].ReferenceLevelList;
        List<int> StartCostList = Managers.Data.HeroUpgradeCostInfoDataDic[upgradeType].StartCostList;
        List<int> IncreaseCostList = Managers.Data.HeroUpgradeCostInfoDataDic[upgradeType].IncreaseCostList;

        int startCost = 0;
        int increaseCost = 0;

        int i = 0;
        for (i = 0; i < ReferenceLevelList.Count; i++)
        {
            if (level <= ReferenceLevelList[i])
            {
                startCost = StartCostList[i];
                increaseCost = IncreaseCostList[i];
                break;
            }
        }

        int increaseCount = level - (i == 0 ? 0 : ReferenceLevelList[i - 1]) - 1;
        return startCost + increaseCount * increaseCost;
    }

    public static string GetHeroUpgradeString(EHeroUpgradeType type)
    {
        return type switch
        {
            EHeroUpgradeType.Growth_Atk => "공격력",
            EHeroUpgradeType.Growth_Hp => "체력",
            EHeroUpgradeType.Growth_Recovery => "체력 회복",
            EHeroUpgradeType.Growth_CriRate => "치명타 확률",
            EHeroUpgradeType.Growth_CriDmg => "치명타 피해",
            _ => throw new ArgumentException($"Unknown rare type String: {type}")
        };
    }
    #endregion

    #region Equipment System

    public static int GetUpgradeEquipmentMaxCount(int level)
    {
        return level + 1;
    }
    #endregion

    #region DrawSystem

    public static ERareType GetRareType(int value)
    {
        return value switch
        {
            0 => ERareType.Normal,
            1 => ERareType.Advanced,
            2 => ERareType.Rare,
            3 => ERareType.Legendary,
            4 => ERareType.Mythical,
            5 => ERareType.Celestial,
            _ => throw new ArgumentException($"Unknown rare type value: {value}")
        };
    }

    public static string GetRareTypeString(ERareType type)
    {
        return type switch
        {
            ERareType.Normal => "일반",
            ERareType.Advanced => "고급",
            ERareType.Rare => "희귀",
            ERareType.Legendary => "전설",
            ERareType.Mythical => "신화",
            ERareType.Celestial => "천상",
            _ => throw new ArgumentException($"Unknown rare type String: {type}")
        };
    }

    public static Color GetRareTypeColor(ERareType type)
    {
        return type switch
        {
            ERareType.Normal => HexToColor("#9F9F9F"),
            ERareType.Advanced => HexToColor("#81CA85"),
            ERareType.Rare => HexToColor("#81C0CA"),
            ERareType.Legendary => HexToColor("#CA9381"),
            ERareType.Mythical => HexToColor("#FF624E"),
            ERareType.Celestial => HexToColor("#FFE74E"),
            _ => throw new ArgumentException($"Unknown rare type String: {type}")
        };
    }


    public static string GetEquipmentString(EEquipmentType type)
    {
        string equipmentString = string.Empty;
        switch (type)
        {
            case EEquipmentType.Weapon:
                equipmentString = "무기";
                break;
            case EEquipmentType.Armor:
                equipmentString = "갑옷";
                break;
            case EEquipmentType.Ring:
                equipmentString = "반지";
                break;
        }
        return equipmentString;
    }


    public static List<int> GetEquipmentDrawResults(EEquipmentType type, int drawCount, int initialLevel)
    {
        var resultEqList = new List<int>();
        var gachaData = Managers.Data.GachaDataDic[initialLevel];
        int weightValue = GetWeightValueByType(type);
        for (int i = 0; i < drawCount; i++)
        {
            // 현재 레벨과 뽑기 시작 시의 레벨이 다른 경우, 새로운 레벨 데이터를 다시 가져옵니다.
            gachaData = CheckAndUpdateGachaData(type, ref initialLevel);

            // 뽑기 확률에 따라 장비를 뽑습니다.
            ERareType rareType = GetRandomRareType(gachaData.DrawProbability);
            int equipmentIndex = GetEquipmentIndexForRareType(gachaData, rareType);
            int dataID = GetEquipmentDataID(gachaData, rareType, equipmentIndex, weightValue);
            resultEqList.Add(dataID);
            Managers.Event.TriggerEvent(EEventType.DrawDataUpdated, type);

        }

        return resultEqList;
    }

    private static int GetWeightValueByType(EEquipmentType type)
    {
        switch (type)
        {
            case EEquipmentType.Armor:
                return 100000;
            case EEquipmentType.Ring:
                return 200000;
            default:
                return 0;
        }
    }

    private static DrawEquipmentGachaData CheckAndUpdateGachaData(EEquipmentType type, ref int initialLevel)
    {
        //현재 레벨과 뽑기 시작 시의 레벨이 다른 경우, 새로운 레벨 데이터를 다시 가져옵니다.
        if (Managers.Backend.GameData.DrawLevelData.DrawDic[type.ToString()].DrawLevel != initialLevel)
        {
            initialLevel = Managers.Backend.GameData.DrawLevelData.DrawDic[type.ToString()].DrawLevel;
        }

        return Managers.Data.GachaDataDic[initialLevel];
    }

    // 등급에 따른 가챠 
    public static ERareType GetRandomRareType(List<float> drawProbability)
    {
        return GetRareType(GetDrawProbabilityType(drawProbability));
    }

    // 장비에 따른 인덱스 가챠 
    public static int GetEquipmentIndexForRareType(DrawEquipmentGachaData gachaData, ERareType rareType)
    {
        switch (rareType)
        {
            case ERareType.Normal:
                return GetDrawProbabilityType(gachaData.NormalDrawList);
            case ERareType.Advanced:
                return GetDrawProbabilityType(gachaData.AdvancedDrawList);
            case ERareType.Rare:
                return GetDrawProbabilityType(gachaData.RareDrawList);
            case ERareType.Legendary:
                return GetDrawProbabilityType(gachaData.LegendaryDrawList);
            case ERareType.Mythical:
                return GetDrawProbabilityType(gachaData.MythicalDrawList);
            case ERareType.Celestial:
                return GetDrawProbabilityType(gachaData.CelestialDrawList);
            default:
                Debug.LogWarning($"Unknown rare type: {rareType}");
                return -1;
        }
    }

    // 등급과 인덱스에 맞는 장비 ID 반환 
    public static int GetEquipmentDataID(DrawEquipmentGachaData gachaData, ERareType rareType, int equipmentIndex, int weightValue)
    {
        List<int> equipmentIdList;

        switch (rareType)
        {
            case ERareType.Normal:
                equipmentIdList = gachaData.NormalEqIdList;
                break;
            case ERareType.Advanced:
                equipmentIdList = gachaData.AdvancedEqIdList;
                break;
            case ERareType.Rare:
                equipmentIdList = gachaData.RareEqIdList;
                break;
            case ERareType.Legendary:
                equipmentIdList = gachaData.LegendaryEqIdList;
                break;
            case ERareType.Mythical:
                equipmentIdList = gachaData.MythicalEqIdList;
                break;
            case ERareType.Celestial:
                equipmentIdList = gachaData.CelestialEqIdList;
                break;
            default:
                Debug.LogWarning($"Unknown rare type: {rareType}");
                return -1;
        }

        int dataID = equipmentIdList[equipmentIndex];
        return dataID + weightValue;  // weightValue를 더해서 반환
    }

    // 랜덤 
    public static int GetDrawProbabilityType(List<float> drawList)
    {
        float total = drawList.Sum();
        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < drawList.Count; i++)
        {
            if (randomPoint < drawList[i])
            {
                return i;
            }
            randomPoint -= drawList[i];
        }
        return drawList.Count - 1;
    }


    #endregion

    #region Stat System

    // 스탯 계산
    public static float CalculateStat(EHeroUpgradeType upgradeType, Dictionary<EHeroUpgradeType, int> levelDic)
    {
        // 기본 값 및 증가 값 가져오기
        var upgradeData = Managers.Data.HeroUpgradeInfoDataDic[upgradeType];
        float baseValue = upgradeData.Value;
        float increaseValue = upgradeData.IncreaseValue;
        int currentLevel = levelDic[upgradeType];

        // 최종 값 계산
        return baseValue + (increaseValue * (currentLevel - 1));
    }

    // 장비 효과 적용
    public static float ApplyEquipmentEffect(EEquipmentType equipmentType, float baseStat)
    {
        // 보유한 장비 효과 및 장착된 장비 효과 가져오기
        float ownedValue = Managers.Equipment.OwnedEquipmentValues(equipmentType);
        float equipValue = Managers.Equipment.EquipEquipmentValue(equipmentType);

        // 보유 효과가 존재하면 먼저 적용
        if (ownedValue != 0)
        {
            baseStat *= (1 + ownedValue / 100f);
        }

        // 장착 효과가 존재하면 추가로 적용
        if (equipValue != 0)
        {
            baseStat *= (1 + equipValue / 100f);
        }

        return baseStat; // 최종 스탯 반환
    }

    // 총전투력 계산 함수
    public static float CalculateTotalCombatPower(HeroInfo hero)
    {
        // 모든 스탯을 단순히 합산하여 총 전투력을 계산합니다.
        float totalCombatPower = 0.0f;

        // HeroInfo 클래스에서 제공하는 각 스탯의 값을 합산
        totalCombatPower += hero.Atk;       // 공격력
        totalCombatPower += hero.MaxHp;     // 체력
        totalCombatPower += hero.Recovery;  // 회복력
        totalCombatPower += hero.CriRate;   // 치명타 확률
        totalCombatPower += hero.CriDmg;    // 치명타 데미지
        totalCombatPower += hero.AttackRange; // 공격 범위
        totalCombatPower += hero.AttackDelay; // 공격 딜레이
        totalCombatPower += hero.AttackSpeedRate; // 공격 속도

        // 필요시 다른 스탯도 합산할 수 있음
        // totalCombatPower += hero.Defense; // 방어력 예시

        return totalCombatPower;
    }

    #endregion

    public static string ConvertToKoreanUnits(long number)
    {
        if (number == 0) return "0";

        // 단위 정의 (4자리마다 '만', '억', '조' 사용)
        string[] units = { "", "만", "억", "조" };
        int unitIndex = 0;
        List<string> parts = new List<string>();

        // 단위를 나누면서 나머지 값을 저장
        while (number > 0 && unitIndex < units.Length)
        {
            long remainder = number % 10000; // 4자리마다 나머지 값 추출
            if (remainder > 0)
            {
                // 나머지가 0이 아닐 때만 단위와 함께 리스트에 추가
                parts.Insert(0, $"{remainder:N0}{units[unitIndex]}");
            }
            number /= 10000;
            unitIndex++;
        }

        // 리스트를 문자열로 합쳐서 반환
        return string.Join("", parts);
    }

}