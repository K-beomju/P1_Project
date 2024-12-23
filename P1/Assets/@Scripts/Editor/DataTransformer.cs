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
	[MenuItem("Tools/ParseCsv %#K")]
	public static void ParseCsvDataToJson()
	{
		Debug.Log("DataTransformer Completed");
		
		// [스테이지]
		ParseCsvDataToJson<StageInfoDataLoader, StageInfoData>("StageInfo");

		// [던전]
        ParseCsvDataToJson<GoldDungeonInfoDataLoader, GoldDungeonInfoData>("GoldDungeonInfo");
        ParseCsvDataToJson<DiaDungeonInfoDataLoader, DiaDungeonInfoData>("DiaDungeonInfo");
		ParseCsvDataToJson<WorldBossDungeonInfoDataLoader, WorldBossDungeonInfoData>("WorldBossDungeonInfo");
     	
		// [영웅]
		ParseCsvDataToJson<HeroAttributeInfoDataLoader, HeroAttributeInfoData>("HeroAttributeInfo");
		ParseCsvDataToJson<HeroAttributeCostInfoDataLoader, HeroAttributeCostInfoData>("HeroAttributeCostInfo");
		ParseCsvDataToJson<HeroInfoDataLoader, HeroInfoData>("HeroInfo");
		ParseCsvDataToJson<HeroUpgradeCostInfoDataLoader, HeroUpgradeCostInfoData>("HeroUpgradeCostInfo");
		ParseCsvDataToJson<HeroUpgradeInfoDataLoader, HeroUpgradeInfoData>("HeroUpgradeInfo");
		
		// [유물]
		ParseCsvDataToJson<RelicInfoDataLoader, RelicInfoData>("Relic");
		
		// [승급]
        ParseCsvDataToJson<RankUpInfoDataLoader, RankUpInfoData>("RankUp");

     	// [가챠]
		ParseCsvDataToJson<DrawEquipmentGachaDataLoader, DrawEquipmentGachaData>("DrawEquipmentGachaInfo");
        ParseCsvDataToJson<DrawSkillGachaDataLoader, DrawSkillGachaData>("DrawSkillGachaInfo");
        ParseCsvDataToJson<DrawRankUpGachaInfoDataLoader, DrawRankUpGachaInfoData>("DrawRankUpGachaInfo");

		// [몬스터]
		ParseCsvDataToJson<MonsterInfoDataLoader, MonsterInfoData>("MonsterInfo");
		ParseCsvDataToJson<BossMonsterInfoDataLoader, BossMonsterInfoData>("BossMonsterInfo");
		ParseCsvDataToJson<RankUpMonsterInfoDataLoader, RankUpMonsterInfoData>("RankUpMonsterInfo");

        // [장비]
		ParseCsvDataToJson<EquipmentDataLoader, EquipmentData>("Equipment");

		// [스킬]
        ParseCsvDataToJson<SkillDataLoader, SkillData>("Skill");
        ParseCsvDataToJson<EffectDataLoader, EffectData>("Effect");

		// [아이템], [상점], [퀘스트]
        ParseCsvDataToJson<ItemDataLoader, ItemData>("Item");
        ParseCsvDataToJson<ShopDataLoader, ShopData>("Shop");
        ParseCsvDataToJson<QuestDataLoader, QuestData>("Quest");

		// [펫]
        ParseCsvDataToJson<PetDataLoader, PetData>("Pet");

		// [미션]
        ParseCsvDataToJson<MissionDataLoader, MissionData>("Mission");
	}

	#region Helpers
	private static void ParseCsvDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
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

		string[] lines = File.ReadAllText($"{Application.dataPath}/Resources/Data/CsvData/{filename}Data.csv").Split("\n");

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