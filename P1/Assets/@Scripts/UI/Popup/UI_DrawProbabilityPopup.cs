using System.Collections.Generic;
using UnityEngine;
using BackendData.GameData;

using static Define;

public class UI_DrawProbabilityPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        ProbabilityGroup
    }

    public enum Texts
    {
        Text_DrawTypeLevel
    }

    public enum Buttons
    {
        Btn_Left,
        Btn_Right,
        Btn_Exit
    }

    private List<UI_DrawProbabilityItem> drawPbItems = new List<UI_DrawProbabilityItem>();

    // 레벨 관리 변수
    private int _currentLevel;
    private EEquipmentType _currentType;

    // 레벨별 확률 데이터 캐싱을 위한 Dictionary
    private Dictionary<int, List<float>> _cachedProbabilityData;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        var contentGroup = GetObject((int)GameObjects.ProbabilityGroup);
        for (int i = 0; i < contentGroup.transform.childCount; i++)
        {
            drawPbItems.Add(contentGroup.transform.GetChild(i).GetComponent<UI_DrawProbabilityItem>());
        }

        GetButton((int)Buttons.Btn_Left).onClick.AddListener(OnClickLeft);
        GetButton((int)Buttons.Btn_Right).onClick.AddListener(OnClickRight);
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() => Managers.UI.ClosePopupUI(this));
        GetObject((int)GameObjects.BG).BindEvent(() => Managers.UI.ClosePopupUI(this), EUIEvent.Click);
        // 확률 데이터를 캐싱하는 함수 호출
        CacheProbabilityData();

        return true;
    }

    private void CacheProbabilityData()
    {
        // 레벨별 확률 데이터를 캐싱할 Dictionary 초기화
        _cachedProbabilityData = new Dictionary<int, List<float>>();

        // 1부터 10까지의 레벨에 대한 확률 데이터를 캐싱
        for (int level = 1; level <= 10; level++)
        {
            // 레벨별 gachaData를 가져와 각 리스트의 데이터를 결합하여 List<float>로 저장
            var gachaData = Managers.Backend.Chart.DrawEquipmentGacha.Dic[level]; //Managers.Data.GachaDataDic[level];
            List<float> drawProbability = new List<float>();
            drawProbability.AddRange(gachaData.NormalDrawList);
            drawProbability.AddRange(gachaData.AdvancedDrawList);
            drawProbability.AddRange(gachaData.RareDrawList);
            drawProbability.AddRange(gachaData.LegendaryDrawList);
            drawProbability.AddRange(gachaData.MythicalDrawList);
            drawProbability.AddRange(gachaData.CelestialDrawList);

            // 해당 레벨의 확률 데이터를 Dictionary에 추가
            _cachedProbabilityData[level] = drawProbability;
        }
    }

    public void RefreshUI(EEquipmentType type)
    {
        var equipmentData = Managers.Backend.GameData.DrawLevelData.DrawDic[type.ToString()]; 
        _currentType = type;
        _currentLevel = equipmentData.DrawLevel; // 초기 레벨 설정

        UpdateLevelText();
        UpdateDrawProbabilityUI(); // UI 갱신 시 캐싱된 데이터 사용
    }

    private void UpdateDrawProbabilityUI()
    {
        // 캐싱된 데이터에서 현재 레벨의 확률 데이터를 가져옴
        if (_cachedProbabilityData.TryGetValue(_currentLevel, out var drawProbability))
        {
            List<EquipmentInfoData> equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(_currentType);

            for (int i = 0; i < drawPbItems.Count; i++)
            {
                if (i < drawProbability.Count)
                    drawPbItems[i].RefreshUI(equipmentInfos[i], drawProbability[i]);
            }
        }
    }

    // Left 버튼 클릭 시 레벨 감소
    private void OnClickLeft()
    {
        if (_currentLevel > 1) // 최소 레벨을 1로 설정
        {
            _currentLevel--;
            UpdateDrawProbabilityUI(); // 레벨 변경 후 UI 갱신
            UpdateLevelText(); // 텍스트 업데이트
        }
    }

    // Right 버튼 클릭 시 레벨 증가
    private void OnClickRight()
    {
        if (_currentLevel < 10) // 최대 레벨 10
        {
            _currentLevel++;
            UpdateDrawProbabilityUI(); // 레벨 변경 후 UI 갱신
            UpdateLevelText(); // 텍스트 업데이트
        }
    }

    private void UpdateLevelText()
    {
        GetTMPText((int)Texts.Text_DrawTypeLevel).text = $"{Util.GetEquipmentString(_currentType)} 뽑기 Lv. {_currentLevel}";
    }
}
