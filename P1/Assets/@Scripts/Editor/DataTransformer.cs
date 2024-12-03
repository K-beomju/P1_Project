using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using Data;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
	[MenuItem("Tools/ParseExcel %#K")]
	public static void ParseExcelDataToJson()
	{
		Debug.Log("DataTransformer Completed");
		ParseExcelDataToJson<StageInfoDataLoader, StageInfoData>("StageInfo");
		ParseExcelDataToJson<MonsterInfoDataLoader, MonsterInfoData>("MonsterInfo");
		ParseExcelDataToJson<BossMonsterInfoDataLoader, BossMonsterInfoData>("BossMonsterInfo");
		ParseExcelDataToJson<RankUpMonsterInfoDataLoader, RankUpMonsterInfoData>("RankUpMonsterInfo");

		ParseExcelDataToJson<HeroInfoDataLoader, HeroInfoData>("HeroInfo");
		ParseExcelDataToJson<HeroUpgradeInfoDataLoader, HeroUpgradeInfoData>("HeroUpgradeInfo");
		ParseExcelDataToJson<HeroUpgradeCostInfoDataLoader, HeroUpgradeCostInfoData>("HeroUpgradeCostInfo");
		ParseExcelDataToJson<HeroAttributeInfoDataLoader, HeroAttributeInfoData>("HeroAttributeInfo");
		ParseExcelDataToJson<HeroAttributeCostInfoDataLoader, HeroAttributeCostInfoData>("HeroAttributeCostInfo");
		ParseExcelDataToJson<HeroRelicInfoDataLoader, HeroRelicInfoData>("HeroRelicInfo");

		ParseExcelDataToJson<DrawEquipmentGachaDataLoader, DrawEquipmentGachaData>("DrawEquipmentGachaInfo");
        ParseExcelDataToJson<DrawSkillGachaDataLoader, DrawSkillGachaData>("DrawSkillGachaInfo");

		ParseExcelDataToJson<EquipmentDataLoader, EquipmentData>("Equipment");
        ParseExcelDataToJson<SkillDataLoader, SkillData>("Skill");
        ParseExcelDataToJson<EffectDataLoader, EffectData>("Effect");

        ParseExcelDataToJson<GoldDungeonInfoDataLoader, GoldDungeonInfoData>("GoldDungeonInfo");
        ParseExcelDataToJson<DiaDungeonInfoDataLoader, DiaDungeonInfoData>("DiaDungeonInfo");
		ParseExcelDataToJson<WorldBossDungeonInfoDataLoader, WorldBossDungeonInfoData>("WorldBossDungeonInfo");

        ParseExcelDataToJson<ItemDataLoader, ItemData>("Item");
        ParseExcelDataToJson<RankUpInfoDataLoader, RankUpInfoData>("RankUpInfo");
        ParseExcelDataToJson<DrawRankUpGachaInfoDataLoader, DrawRankUpGachaInfoData>("DrawRankUpGachaInfo");
        ParseExcelDataToJson<ShopDataLoader, ShopData>("Shop");
        ParseExcelDataToJson<QuestDataLoader, QuestData>("Quest");

	}

	#region Helpers
	private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
	{
		Loader loader = new Loader();
		FieldInfo field = loader.GetType().GetFields()[0];
		field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/JsonData/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

	private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new()
	{
		List<LoaderData> loaderDatas = new List<LoaderData>();

		string[] lines = File.ReadAllText($"{Application.dataPath}/Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

		for (int l = 1; l < lines.Length; l++)
		{
			string[] row = lines[l].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			LoaderData loaderData = new LoaderData();
			var fields = GetFieldsInBase(typeof(LoaderData));

			for (int f = 0; f < fields.Count; f++)
			{
				FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
				if (field == null)
{
    Debug.LogError($"Field '{fields[f].Name}' not found in {loaderData.GetType()}.");
    continue;
}
				Type type = field.FieldType;

				if (type.IsGenericType)
				{
					//object value = ConvertList(row[f], type);
					object value = ConvertListOrDictionary(row[f], type);

					field.SetValue(loaderData, value);
				}
				else
				{
					object value = ConvertValue(row[f], field.FieldType);
					field.SetValue(loaderData, value);
				}
			}

			loaderDatas.Add(loaderData);
		}

		return loaderDatas;
	}

	private static object ConvertValue(string value, Type type)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		TypeConverter converter = TypeDescriptor.GetConverter(type);
		return converter.ConvertFromString(value);
	}

	private static object ConvertListOrDictionary(string value, Type type)
	{
		if(string.IsNullOrEmpty(value))
		return null;

		if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
		{
			return ConvertDictionary(value, type);
		}
    	else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			return ConvertList(value, type);
		}

		    return null;

	}

	private static object ConvertDictionary(string value, Type type)
	{
		if(string.IsNullOrEmpty (value))
		return null;

		value = value.Trim();

		Type[] genericArgs = type.GetGenericArguments();
		Type keyType = genericArgs[0];
		Type valueType = genericArgs[1];
		Type genericDictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
		var genericDictionary = Activator.CreateInstance(genericDictionaryType) as IDictionary;

		var pairs = value.Split('&');
		foreach (var pair in pairs)
		{
			var keyValue = pair.Split(':');
			if(keyValue.Length != 2) continue;

			object key = ConvertValue(keyValue[0], keyType);
			object val = ConvertValue(keyValue[1], valueType);
			genericDictionary.Add(key, val);
		}

		return genericDictionary;
	}

	private static object ConvertList(string value, Type type)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		// Reflection
		Type valueType = type.GetGenericArguments()[0];
		Type genericListType = typeof(List<>).MakeGenericType(valueType);
		var genericList = Activator.CreateInstance(genericListType) as IList;

		// Parse Excel
		var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

		foreach (var item in list)
			genericList.Add(item);

		return genericList;
	}



	public static List<FieldInfo> GetFieldsInBase(Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
	{
		List<FieldInfo> fields = new List<FieldInfo>();
		HashSet<string> fieldNames = new HashSet<string>(); // 중복방지
		Stack<Type> stack = new Stack<Type>();

		while (type != typeof(object))
		{
			stack.Push(type);
			type = type.BaseType;
		}

		while (stack.Count > 0)
		{
			Type currentType = stack.Pop();

			foreach (var field in currentType.GetFields(bindingFlags))
			{
				if (fieldNames.Add(field.Name))
				{
					fields.Add(field);
				}
			}
		}

		return fields;
	}
	#endregion

#endif
}