using UnityEngine;
using UnityEditor;

public class MyCustomEditorWindow : EditorWindow
{

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
        if (GUILayout.Button("레벨업 팝업 테스트", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.UI.ShowBaseUI<UI_LevelUpBase>().ShowLevelUpUI(3);
        }
        if (GUILayout.Button("스테이지 팝업 테스트", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            Managers.UI.ShowBaseUI<UI_StageDisplayBase>().RefreshShowDisplayStage(3);
        }
        if (GUILayout.Button("전투력 팝업 테스트", GUILayout.Width(position.width), GUILayout.Height(50)))
        {
            //Managers.UI.ShowBaseUI<UI_TotalPowerBase>().ShowTotalPowerUI();
        }
    }
}
