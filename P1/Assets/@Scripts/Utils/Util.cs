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
		string heroUpgradeString = string.Empty;
		switch (type)
		{
			case EHeroUpgradeType.Growth_Atk:
				heroUpgradeString = "공격력";
				break;
			case EHeroUpgradeType.Growth_Hp:
				heroUpgradeString = "체력";
				break;
		}
		return heroUpgradeString;
	}


	#region DrawSystem

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
        _ => throw new ArgumentException($"Unknown rare type: {type}")
    };
}

	public static string GetEquipmentString(EEquipmentType type)
	{
		string equipmentString = string.Empty;
		switch (type)
		{
			case EEquipmentType.Sword:
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

	public static List<EquipmentDrawResult> GetEquipmentDrawResults(EEquipmentType type, int drawCount, int initialLevel)
	{
		var resultEqList = new List<EquipmentDrawResult>();
		var gachaData = Managers.Data.GachaDataDic[initialLevel];

		for (int i = 0; i < drawCount; i++)
		{
			// 현재 레벨과 뽑기 시작 시의 레벨이 다른 경우, 새로운 레벨 데이터를 다시 가져옵니다.
			gachaData = CheckAndUpdateGachaData(type, ref initialLevel);

			// 뽑기 확률에 따라 장비를 뽑습니다.
			ERareType rareType = GetRandomRareType(gachaData.DrawProbability);
			int equipmentIndex = GetEquipmentIndexForRareType(gachaData, rareType);

			resultEqList.Add(new EquipmentDrawResult(rareType, equipmentIndex));
			//Managers.Event.TriggerEvent(EEventType.UpdateDraw, type);

		}

		return resultEqList;
	}

	private static DrawEquipmentGachaData CheckAndUpdateGachaData(EEquipmentType type, ref int initialLevel)
	{
		//현재 레벨과 뽑기 시작 시의 레벨이 다른 경우, 새로운 레벨 데이터를 다시 가져옵니다.
		if (Managers.Game.PlayerGameData.DrawData[type].Level != initialLevel)
		{
			initialLevel = Managers.Game.PlayerGameData.DrawData[type].Level;
		}

		return Managers.Data.GachaDataDic[initialLevel];
	}

	public static ERareType GetRandomRareType(List<float> drawProbability)
	{
		return GetRareType(GetDrawProbabilityType(drawProbability));
	}

	#endregion
}