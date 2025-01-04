using BackendData.GameData;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    /// <summary>
    /// 맵 경계와 이동 범위를 고려하여 랜덤한 위치를 반환합니다.
    /// </summary>
    /// <param name="tileMap">타일맵</param>
    /// <param name="currentPosition">현재 위치</param>
    /// <param name="moveRange">이동 범위</param>
    /// <returns>맵 안의 랜덤 위치</returns>
    public static UnityEngine.Vector3 GetRandomPositionWithinMap(Tilemap tileMap, UnityEngine.Vector3 currentPosition, float moveRange, float padding)
    {
        // 타일맵 경계 가져오기
        BoundsInt bounds = tileMap.cellBounds;
        UnityEngine.Vector3 minPosition = tileMap.CellToWorld(bounds.min);
        UnityEngine.Vector3 maxPosition = tileMap.CellToWorld(bounds.max);

        // 보정값 적용
        minPosition += new UnityEngine.Vector3(padding, padding, 0);
        maxPosition -= new UnityEngine.Vector3(padding, padding, 0);

        // 이동 가능한 범위 내에서 랜덤 위치 생성
        float randomX = Mathf.Clamp(
            currentPosition.x + UnityEngine.Random.Range(-moveRange, moveRange),
            minPosition.x,
            maxPosition.x
        );
        float randomY = Mathf.Clamp(
            currentPosition.y + UnityEngine.Random.Range(-moveRange, moveRange),
            minPosition.y,
            maxPosition.y
        );

        return new UnityEngine.Vector3(randomX, randomY, currentPosition.z); // Z 축은 그대로 유지
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
        List<int> ReferenceLevelList = Managers.Data.HeroUpgradeCostChart[upgradeType].ReferenceLevelList;
        List<int> StartCostList = Managers.Data.HeroUpgradeCostChart[upgradeType].StartCostList;
        List<int> IncreaseCostList = Managers.Data.HeroUpgradeCostChart[upgradeType].IncreaseCostList;

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

    public static int GetAttributeCost(EHeroAttrType attrType, int level)
    {
        List<int> ReferenceLevelList = Managers.Data.HeroAttributeCostChart[attrType].ReferenceLevelList;
        List<int> StartCostList = Managers.Data.HeroAttributeCostChart[attrType].StartCostList;
        List<int> IncreaseCostList = Managers.Data.HeroAttributeCostChart[attrType].IncreaseCostList;

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

    #endregion

    #region Equipment System

    public static int GetUpgradeEquipmentMaxCount(int level)
    {
        // 레벨에 따라 최대 장비 개수를 설정
        if (level < 1 || level > 100)
        {
            return 0; // 유효하지 않은 레벨
        }

        // 레벨에 따른 최대 장비 개수
        if (level == 1) return 2;
        if (level == 2) return 3;
        if (level == 3) return 4;
        if (level == 4) return 5;
        if (level == 5) return 7;
        if (level == 6) return 9;
        if (level == 7) return 11;
        if (level == 8) return 13;
        if (level >= 9) return 15; // 9레벨부터 100레벨까지는 15개로 일정

        return 0; // 기본적으로 0을 반환
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

    public static string GetRareTypeColorCode(ERareType type)
    {
        return type switch
        {
            ERareType.Normal => "#9F9F9F",
            ERareType.Advanced => "#81CA85",
            ERareType.Rare => "#81C0CA",
            ERareType.Legendary => "#CA9381",
            ERareType.Mythical => "#FF624E",
            ERareType.Celestial => "#FFE74E",
            _ => throw new ArgumentException($"Unknown rare type String: {type}")
        };
    }


    public static List<int> GetDrawSystemResults(EDrawType type, int drawCount, int initialLevel)
    {
        var resultEqList = new List<int>();
        DrawGachaData gachaData = null;

        if (type.IsEquipmentType())
            gachaData = Managers.Data.DrawEquipmentChart[initialLevel];
        if (type == EDrawType.Skill)
            gachaData = Managers.Data.DrawSkillChart[initialLevel];

        if (gachaData == null)
        {
            Debug.LogWarning($"{type} 가챠 데이터가 존재하지 않습니다");
            return null;
        }

        // DataId value
        int weightValue = GetWeightValueByType(type);

        for (int i = 0; i < drawCount; i++)
        {
            // 현재 레벨과 뽑기 시작 시의 레벨이 다른 경우, 새로운 레벨 데이터를 다시 가져옵니다.
            gachaData = CheckAndUpdateGachaData(type, ref initialLevel);

            // 뽑기 확률에 따라 장비를 뽑습니다.
            ERareType rareType = GetRandomRareType(gachaData.DrawProbability);
            int drawItemIndex = GetDrawItemIndexForRareType(gachaData, rareType);
            int dataID = GetItemDataID(gachaData, rareType, drawItemIndex, weightValue);
            resultEqList.Add(dataID);
            Managers.Backend.GameData.DrawLevelData.AddDrawCount(type);

        }

        return resultEqList;
    }

    private static int GetWeightValueByType(EDrawType type)
    {
        switch (type)
        {
            case EDrawType.Weapon:
            case EDrawType.Skill:
                return 0;
            case EDrawType.Armor:
                return 100000;
            case EDrawType.Ring:
                return 200000;
        }

        return 0;
    }

    private static DrawGachaData CheckAndUpdateGachaData(EDrawType type, ref int initialLevel)
    {
        //현재 레벨과 뽑기 시작 시의 레벨이 다른 경우, 새로운 레벨 데이터를 다시 가져옵니다.
        if (Managers.Backend.GameData.DrawLevelData.DrawDic[type.ToString()].DrawLevel != initialLevel)
        {
            initialLevel = Managers.Backend.GameData.DrawLevelData.DrawDic[type.ToString()].DrawLevel;
        }

        if (type.IsEquipmentType())
            return Managers.Data.DrawEquipmentChart[initialLevel];
        if (type == EDrawType.Skill)
            return Managers.Data.DrawSkillChart[initialLevel];

        return null;
    }

    // 등급에 따른 가챠 
    public static ERareType GetRandomRareType(List<float> drawProbability)
    {
        return GetRareType(GetDrawProbabilityType(drawProbability));
    }

    // 장비에 따른 인덱스 가챠 
    public static int GetDrawItemIndexForRareType(DrawGachaData gachaData, ERareType rareType)
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
    public static int GetItemDataID(DrawGachaData gachaData, ERareType rareType, int equipmentIndex, int weightValue)
    {
        List<int> itemIdList = new List<int>();

        switch (rareType)
        {
            case ERareType.Normal:
                itemIdList = gachaData.NormalEqIdList;
                break;
            case ERareType.Advanced:
                itemIdList = gachaData.AdvancedEqIdList;
                break;
            case ERareType.Rare:
                itemIdList = gachaData.RareEqIdList;
                break;
            case ERareType.Legendary:
                itemIdList = gachaData.LegendaryEqIdList;
                break;
            case ERareType.Mythical:
                itemIdList = gachaData.MythicalEqIdList;
                break;
            case ERareType.Celestial:
                itemIdList = gachaData.CelestialEqIdList;
                break;
            default:
                Debug.LogWarning($"Unknown rare type: {rareType}");
                return -1;
        }

        int dataID = itemIdList[equipmentIndex];
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

    // 랜덤 
    public static int GetDrawProbabilityType(List<int> drawList)
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

    #region Convert Value 

    public static string ConvertToTotalCurrency(double value)
    {
        // 10,000 미만일 경우 그대로 반환
        if (value < 10000)
        {
            return value.ToString("F0"); // 소수점 없이 반환
        }

        string[] koreanUnits = { "만", "억", "조", "경" };
        int unitIndex = -1;
        double mainPart = value;

        // 큰 단위로 나누면서 단위를 증가시킴
        while (mainPart >= 10000 && unitIndex < koreanUnits.Length - 1)
        {
            unitIndex++;
            mainPart /= 10000;
        }

        // 결과 문자열 생성 (mainPart는 정수 부분만 표시)
        string result = $"{(int)mainPart}{koreanUnits[unitIndex]}";

        // 10,000 미만의 나머지 값을 구함
        double remainder = value % Math.Pow(10000, unitIndex + 1);

        // 나머지 값이 10,000 미만일 경우 mainPart와 결합하여 표시
        if (remainder > 0)
        {
            result += $"{(int)remainder}"; // 소수점 없이 정수 부분만 표시
        }

        return result;
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

    public static string GetDrawTypeString(EDrawType type)
    {
        return type switch
        {
            EDrawType.Weapon => "무기",
            EDrawType.Armor => "갑옷",
            EDrawType.Ring => "반지",
            EDrawType.Skill => "스킬",
            _ => throw new ArgumentException($"Unknown DrawType String: {type}")
        };
    }

    public static string GetEquipemntType(EEquipmentType type)
    {
        return type switch
        {
            EEquipmentType.Weapon => "무기",
            EEquipmentType.Armor => "갑옷",
            EEquipmentType.Ring => "반지",
            _ => throw new ArgumentException($"Unknown EquipemntType String: {type}")
        };
    }


    public static string GetEquipmentStatType(EEquipmentType type)
    {
        return type switch
        {
            EEquipmentType.Weapon => "공격력",
            EEquipmentType.Armor => "체력",
            EEquipmentType.Ring => "스킬 공격력",
            _ => throw new ArgumentException($"Unknown EquipmentStat String: {type}")
        };
    }

    public static string GetDungeonType(EDungeonType type)
    {
        return type switch
        {
            EDungeonType.Gold => "골드 던전",
            EDungeonType.Dia => "다이아 던전",
            EDungeonType.Promotion => "승격전",
            EDungeonType.WorldBoss => "월드보스전",
            _ => throw new ArgumentException($"Unknown GetDungeonType String: {type}")
        };
    }



    #endregion


    #region Dungeon

    public static int DungenEntranceMaxValue(EDungeonType type)
    {
        return type switch
        {
            EDungeonType.Gold => 2,
            EDungeonType.Dia => 2,
            EDungeonType.Promotion => 1,
            EDungeonType.WorldBoss => 1,
            _ => throw new ArgumentException($"Unknown GetDungeonType String: {type}")
        };
    }

    #endregion


    #region Ad Buff


    public static string GetAdBuffType(EAdBuffType type)
    {
        return type switch
        {
            EAdBuffType.Atk => "전투의 서",
            EAdBuffType.IncreaseGold => "황금의 서",
            EAdBuffType.IncreaseExp => "성장의 서",
            _ => throw new ArgumentException($"Unknown GetDungeonType String: {type}")
        };
    }


    #endregion


    #region Pet Probability

    public static PetData GetPetCraftData(int currentChapterLevel)
    {
        var petChart = Managers.Data.PetChart;
        foreach (var pet in petChart.Values)
        {
            if (pet.ChapterLevel == currentChapterLevel)
            {
                return pet;
            }
        }

        return null;
    }

    #endregion

    #region UI

    public static UnityEngine.Vector2 GetCanvasPosition(UnityEngine.Vector3 rectTransform)
    {
        UnityEngine.Vector2 canvasLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Managers.UI.SceneUI.transform as RectTransform,                // 기준이 되는 캔버스의 RectTransform
            RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform), // 월드 좌표를 스크린 좌표로 변환
            Camera.main,                                                  // 사용되는 카메라
            out canvasLocalPos);                                          // 결과로 얻는 캔버스 기준 로컬 좌표

        return canvasLocalPos;
    }




    #endregion
}