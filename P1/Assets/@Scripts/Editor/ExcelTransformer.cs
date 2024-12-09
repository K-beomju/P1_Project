using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelDataReader;
using System.IO;
using UnityEditor;

public class ExcelTransformer : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/ParseExcel %#K")]
    public static void ParseExcelDataToCsv()
    {
        // [스테이지]
        // StageInfoData: 스테이지 정보 데이터
        ConvertExcelToCsv("Stage");

        // [던전]
        // DiaDungeonInfoData: 다이아 던전 정보 데이터
        // GoldDungeonInfoData: 골드 던전 정보 데이터
        // WorldBossDungeonInfoData: 월드 보스 던전 정보 데이터
        ConvertExcelToCsv("Dungeon");

        // [영웅]
        // HeroAttributeInfoData: 영웅 속성 정보 데이터
        // HeroAttributeCostInfoData: 영웅 속성 비용 정보 데이터
        // HeroInfoData: 영웅 정보 데이터
        // HeroUpgradeCostInfoData: 영웅 업그레이드 비용 정보 데이터
        // HeroUpgradeInfoData: 영웅 업그레이드 정보 데이터
        // HeroRankUpInfoData: 영웅 승급전 정보 데이터
        ConvertExcelToCsv("Hero");

        // [유물]
        // RelicData: 영웅 유물 정보 데이터
        ConvertExcelToCsv("Relic");

        // [가챠]
        // DrawEquipmentGachaInfoData: 장비 가챠 정보 데이터
        // DrawSkillGachaInfoData: 스킬 가챠 정보 데이터
        // DrawRankUpGachaInfoData: 랭크 슬롯 가챠 정보 데이터
        ConvertExcelToCsv("Gacha");

        // [스킬]
        // SkillInfoData: 스킬 데이터
        // EffectInfoData: 이펙트 데이터
        ConvertExcelToCsv("Skill");

        // [장비]
        // EquipmentData: 장비 정보 데이터
        ConvertExcelToCsv("Equipment");
        
        // [몬스터]
        // MonsterInfoData: 몬스터 정보 데이터
        // BossMoterInfoData: 보스 정보 데이터
        // RankUpMoterInfoData: 보스 정보 데이터
        ConvertExcelToCsv("Monster");

        // [퀘스트]
        // QuestData: 퀘스트 정보 데이터
        ConvertExcelToCsv("Quest");
        
        // [아이템]
        // ItemData: 아이템 정보 데이터
        ConvertExcelToCsv("Item");

        // 남은거 : ShopData
    }

    private static void ConvertExcelToCsv(string filename)
    {
        string path = $"Assets/Resources/Data/ExcelData/{filename}Data.xlsx";
        if (!File.Exists(path))
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {path}");
            return;
        }

        // 출력 폴더 경로 설정
        var outputFolderPath = Path.Combine(Application.dataPath, "Resources/Data/CsvData");
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath); // 폴더 생성
        }

        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var tables = reader.AsDataSet().Tables;

                for (var sheetIndex = 0; sheetIndex < tables.Count; sheetIndex++)
                {
                    var sheet = tables[sheetIndex];
                    Debug.Log(sheet.TableName);

                    string outputFilePath = Path.Combine(outputFolderPath, $"{sheet.TableName}.csv");

                    // CSV 파일로 저장
                    using (var writer = new StreamWriter(outputFilePath))
                    {
                        for (var rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
                        {
                            var row = sheet.Rows[rowIndex];
                            string[] rowData = new string[row.ItemArray.Length];

                            for (var columnIndex = 0; columnIndex < row.ItemArray.Length; columnIndex++)
                            {
                                rowData[columnIndex] = row.ItemArray[columnIndex]?.ToString() ?? "";
                            }

                            // 쉼표로 데이터를 구분
                            writer.WriteLine(string.Join(",", rowData));
                        }
                    }

                    Debug.Log($"CSV 저장 완료: {outputFilePath}");
                    AssetDatabase.Refresh();

                }

            }
        }
        AssetDatabase.Refresh();

    }
#endif
}
