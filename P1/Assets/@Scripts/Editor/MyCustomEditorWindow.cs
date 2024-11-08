using UnityEngine;
using UnityEditor;
using System.Reflection;

public class MyCustomEditorWindow : EditorWindow
{
    private string currencyInputValue = "1000"; // 재화 기본값

    // 메뉴에 'My Window' 항목 추가
    [MenuItem("Window/My Custom Editor")]
    public static void ShowWindow()
    {
        // 새 창을 띄우고 에디터 창의 제목을 설정
        EditorWindow.GetWindow(typeof(MyCustomEditorWindow), false, "My Custom Editor");
    }

    // 창에 표시될 내용 정의
    private void OnGUI()
    {
        if (GUILayout.Button("플레이어 이동 제어", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.Object.Hero.isStopAI = true;
        }
        if (GUILayout.Button("플레이어 이동 활성화", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.Object.Hero.isStopAI = false;
        }
        if (GUILayout.Button("게임 데이터 저장", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.Backend.UpdateAllGameData(callback =>
            {
                if (callback == null)
                {
                    Debug.LogWarning("저장 데이터 미존재, 저장할 데이터가 존재하지 않습니다.");
                    return;
                }

                if (callback.IsSuccess())
                {
                    Debug.Log("저장 성공, 저장에 성공했습니다.");
                }
                else
                {
                    Debug.LogWarning($"수동 저장 실패, 수동 저장에 실패했습니다. {callback.ToString()}");
                }

            });
        }
        if (GUILayout.Button("랭킹 데이터 저장", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.Backend.UpdateRankScore();
        }

        // 재화 충전 입력 필드
        GUILayout.Label("충전할 재화 수량 입력:", GUILayout.Width(position.width));
        currencyInputValue = GUILayout.TextField(currencyInputValue, GUILayout.Width(position.width)); // 사용자 입력

        // 버튼을 수평으로 배치
        GUILayout.BeginVertical(); // 세로 정렬 시작

        GUILayout.BeginHorizontal(); // 수평 정렬 시작
        if (GUILayout.Button("골드 충전", GUILayout.Width(position.width / 3), GUILayout.Height(50)))
        {
            if (float.TryParse(currencyInputValue, out float goldAmount))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(Define.EItemType.Gold, goldAmount);
                Debug.Log($"충전된 골드: {goldAmount}");
            }
            else
            {
                Debug.LogError("유효하지 않은 골드 입력입니다.");
            }
        }

        if (GUILayout.Button("경험치 충전", GUILayout.Width(position.width / 3), GUILayout.Height(50)))
        {
            if (int.TryParse(currencyInputValue, out int expAmount))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(Define.EItemType.ExpPoint, expAmount);
                Debug.Log($"충전된 경험치: {expAmount}");
            }
            else
            {
                Debug.LogError("유효하지 않은 경험치 입력입니다.");
            }
        }

        if (GUILayout.Button("Dia 충전", GUILayout.Width(position.width / 3), GUILayout.Height(50)))
        {
            if (int.TryParse(currencyInputValue, out int diaAmount))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(Define.EItemType.Dia, diaAmount);
                Debug.Log($"충전된 다이아: {diaAmount}");
            }
            else
            {
                Debug.LogError("유효하지 않은 다이아 입력입니다.");
            }
        }
        GUILayout.EndHorizontal(); // 수평 정렬 종료
        GUILayout.EndVertical(); // 세로 정렬 종료

            // 버튼을 수평으로 배치
        GUILayout.BeginVertical(); // 세로 정렬 시작
        GUILayout.BeginHorizontal(); // 수평 정렬 시작
        if (GUILayout.Button("골드 던전 열쇠 충전", GUILayout.Width(position.width / 3), GUILayout.Height(50)))
        {
            Managers.Backend.GameData.DungeonData.AddKey(Define.EDungeonType.Gold, 1);
        }
        if (GUILayout.Button("다이아 던전 열쇠 충전", GUILayout.Width(position.width / 3), GUILayout.Height(50)))
        {
            Managers.Backend.GameData.DungeonData.AddKey(Define.EDungeonType.Dia, 1);
        }
        if (GUILayout.Button("월드 보스 던전 충전", GUILayout.Width(position.width / 3), GUILayout.Height(50)))
        {
            Managers.Backend.GameData.DungeonData.AddKey(Define.EDungeonType.WorldBoss, 1);
        }
        GUILayout.EndHorizontal(); // 수평 정렬 종료
        GUILayout.EndVertical(); // 세로 정렬 종료


        if (GUILayout.Button("플레이어 공격력 증가", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.Backend.GameData.CharacterData.LevelUpHeroUpgrade(Define.EHeroUpgradeType.Growth_Atk);
        }
        if (GUILayout.Button("플레이어 사망", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.Object.Hero.OnDead();
            Managers.Object.Hero.CreatureState = Define.ECreatureState.Dead;
        }

    }
}
