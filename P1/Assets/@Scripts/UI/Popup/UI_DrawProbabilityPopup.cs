using System.Collections.Generic;
using UnityEngine;
using BackendData.GameData;

using static Define;
using System.Linq;

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
    private EDrawType _currentType;

    // 레벨별 확률 데이터 캐싱을 위한 Dictionary
    private Dictionary<EDrawType, Dictionary<int, List<float>>> _cachedProbabilityData;

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
        _cachedProbabilityData = new Dictionary<EDrawType, Dictionary<int, List<float>>>();

        // 장비 (3개), 스킬 (1개) = 4번 
        for (int drawInventory = 0; drawInventory < 4; drawInventory++)
        {
            EDrawType drawType = (EDrawType)drawInventory;

            // drawType에 대한 Dictionary가 존재하지 않으면 초기화
            if (!_cachedProbabilityData.ContainsKey(drawType))
            {
                _cachedProbabilityData[drawType] = new Dictionary<int, List<float>>();
            }

            // 1부터 10까지의 레벨에 대한 확률 데이터를 캐싱
            for (int level = 1; level <= 10; level++)
            {
                Data.DrawGachaData gachaData = null;

                if (drawType.IsEquipmentType())
                    gachaData = Managers.Data.DrawEquipmentChart[level];
                if (drawType == EDrawType.Skill)
                    gachaData = Managers.Data.DrawSkillChart[level];

                if (gachaData == null)
                {
                    Debug.LogWarning($"{drawType} 가챠 데이터가 존재하지 않습니다");
                    return;
                }

                // 레벨별 gachaData를 가져와 각 리스트의 데이터를 결합하여 List<float>로 저장
                List<float> drawProbability = new List<float>();
                drawProbability.AddRange(gachaData.NormalDrawList);
                drawProbability.AddRange(gachaData.AdvancedDrawList);
                drawProbability.AddRange(gachaData.RareDrawList);
                drawProbability.AddRange(gachaData.LegendaryDrawList);
                drawProbability.AddRange(gachaData.MythicalDrawList);
                drawProbability.AddRange(gachaData.CelestialDrawList);

                // 해당 레벨의 확률 데이터를 Dictionary에 추가
                _cachedProbabilityData[drawType][level] = drawProbability;
            }
        }

    }

    public void RefreshUI(EDrawType type)
    {
        var drawData = Managers.Backend.GameData.DrawLevelData.DrawDic[type.ToString()];
        _currentType = type;
        _currentLevel = drawData.DrawLevel; // 초기 레벨 설정
        drawPbItems.ForEach(x => x.gameObject.SetActive(false));
        UpdateLevelText();
        UpdateDrawProbabilityUI(); // UI 갱신 시 캐싱된 데이터 사용
    }

    private void UpdateDrawProbabilityUI()
    {
        // 캐싱된 데이터에서 현재 레벨의 확률 데이터를 가져옴
        if (_cachedProbabilityData.TryGetValue(_currentType, out var drawProbability) && drawProbability.TryGetValue(_currentLevel, out var probabilityList))
        {
            List<Item> itemInfos = null;

            if (_currentType.IsEquipmentType())
                itemInfos = Managers.Equipment.GetEquipmentTypeInfos(Util.ParseEnum<EEquipmentType>(_currentType.ToString())).Cast<Item>().ToList();
            else if (_currentType == EDrawType.Skill)
                itemInfos = Managers.Skill.GetSkillInfos().Cast<Item>().ToList();

            for (int i = 0; i < itemInfos.Count; i++)
            {
                if (i < probabilityList.Count)
                {
                    drawPbItems[i].gameObject.SetActive(true);
                    drawPbItems[i].RefreshUI(itemInfos[i], probabilityList[i]);
                }
            }
        }
        else
        {
            Debug.LogWarning($"확률 데이터가 존재하지 않습니다: Type: {_currentType}, Level: {_currentLevel}");
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
        GetTMPText((int)Texts.Text_DrawTypeLevel).text = $"{Util.GetDrawTypeString(_currentType)} 뽑기 Lv. {_currentLevel}";
    }
}
