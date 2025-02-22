using UnityEngine;
using UnityEditor;
using System.IO;

public class MoveIconToResource
{
    [MenuItem("Assets/Move to Resource/Skill", false, 0)] // 우클릭 메뉴에 "Skill" 옵션 추가
    private static void MoveToSkill()
    {
        MoveToResource("Skill");
    }

    [MenuItem("Assets/Move to Resource/Relic", false, 1)] // 우클릭 메뉴에 "Relic" 옵션 추가
    private static void MoveToRelic()
    {
        MoveToResource("Relic");
    }

    [MenuItem("Assets/Move to Resource/Pet", false, 1)] // 우클릭 메뉴에 "Pet" 옵션 추가
    private static void MoveToPet()
    {
        MoveToResource("Pet");
    }


    private static void MoveToResource(string resourceType)
    {
        // 선택된 에셋 경로 가져오기
        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        // 이동할 목적지 폴더 경로 설정
        string destinationPath = $"Assets/Resources/Sprites/{resourceType}";

        if (!Directory.Exists(destinationPath))
        {
            Debug.LogWarning($"{destinationPath}라는 경로는 없습니다.,,ㅡㅡ");
            return;
        }

        // 파일 이름 추출
        string fileName = Path.GetFileName(selectedPath);
        
        // 원본 파일의 확장자 포함 경로
        string fullDestinationPath = Path.Combine(destinationPath, fileName);

        // 파일을 목적지로 이동
        AssetDatabase.MoveAsset(selectedPath, fullDestinationPath);
        
        // 이동 후 파일 저장 및 프로젝트 업데이트
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"파일이 {fullDestinationPath}로 이동되었습니다.");
    }

    // 메뉴가 활성화되는 조건: 선택된 것이 파일이어야 함
    [MenuItem("Assets/Move to Resource/Skill", true)]
    private static bool ValidateMoveToSkill()
    {
        return ValidateMoveToResource();
    }

    [MenuItem("Assets/Move to Resource/Relic", true)]
    private static bool ValidateMoveToRelic()
    {
        return ValidateMoveToResource();
    }

    [MenuItem("Assets/Move to Resource/Pet", true)]
    private static bool ValidateMoveToPet()
    {
        return ValidateMoveToResource();
    }

    private static bool ValidateMoveToResource()
    {
        // 선택된 것이 유효한지 확인 (파일이 선택된 경우에만 활성화)
        return Selection.activeObject != null && !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
    }
}
