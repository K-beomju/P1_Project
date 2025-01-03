using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Numerics;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using Data;
using System.Globalization;

public class BigIntegerConverter : JsonConverter<BigInteger>
{
    public override void WriteJson(JsonWriter writer, BigInteger value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override BigInteger ReadJson(JsonReader reader, Type objectType, BigInteger existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return BigInteger.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture);
    }
}

public class DecimalConverter : JsonConverter<decimal>
{
    public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(CultureInfo.InvariantCulture));
    }

    public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return decimal.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture);
    }
}

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

		//ParseCsvDataToJson<TestInfoDataLoader, TestInfoData>("TestInfo");

	}

	#region Helpers
	private static void ParseCsvDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
	{
		Loader loader = new Loader();
		FieldInfo field = loader.GetType().GetFields()[0];
		field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

		 // JSON 직렬화 설정
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter>
        {
            new BigIntegerConverter(),
            new DecimalConverter()
        }
    };

    string jsonStr = JsonConvert.SerializeObject(loader, settings);
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

				
			
                if (type == typeof(BigInteger))
                {
                    // BigInteger 변환 처리
                    object value = ConvertBigInteger(row[f]);
                    field.SetValue(loaderData, value);
                }
                else if (type.IsGenericType)
                {
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
private static object ConvertBigInteger(string value)
{
    if (string.IsNullOrEmpty(value))
        return BigInteger.Zero; // 기본값으로 BigInteger.Zero 반환

    value = value.Trim();

    try
    {
        // 지수 표기법 처리
        if (value.Contains("E") || value.Contains("e"))
        {
            // Double로 변환 후 정수형 문자열로 변환
            double doubleValue = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            value = doubleValue.ToString("F0", System.Globalization.CultureInfo.InvariantCulture); // 소수점 제거
        }

        // BigInteger로 변환
        if (BigInteger.TryParse(value, out BigInteger result))
        {
            return result;
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"BigInteger 변환 실패: {value}, 에러: {ex.Message}");
    }

    Debug.LogError($"BigInteger 변환 실패: {value}");
    return BigInteger.Zero;
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