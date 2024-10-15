using UnityEngine;
using UnityEditor;

public class MyCustomEditorWindow : EditorWindow
{
    private string _myString = "Hello, Unity!";
    private bool _groupEnabled;
    private bool _toggleValue = true;
    private float _sliderValue = 1.0f;

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
    }
}
