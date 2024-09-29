using System.Collections.Generic;
using UnityEngine;
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

    // ���� ���� ����
    private int _currentLevel;
    private EEquipmentType _currentType;

    // ������ Ȯ�� ������ ĳ���� ���� Dictionary
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
        // Ȯ�� �����͸� ĳ���ϴ� �Լ� ȣ��
        CacheProbabilityData();

        return true;
    }

    private void CacheProbabilityData()
    {
        // ������ Ȯ�� �����͸� ĳ���� Dictionary �ʱ�ȭ
        _cachedProbabilityData = new Dictionary<int, List<float>>();

        // 1���� 10������ ������ ���� Ȯ�� �����͸� ĳ��
        for (int level = 1; level <= 10; level++)
        {
            // ������ gachaData�� ������ �� ����Ʈ�� �����͸� �����Ͽ� List<float>�� ����
            var gachaData = Managers.Data.GachaDataDic[level];
            List<float> drawProbability = new List<float>();
            drawProbability.AddRange(gachaData.NormalDrawList);
            drawProbability.AddRange(gachaData.AdvancedDrawList);
            drawProbability.AddRange(gachaData.RareDrawList);
            drawProbability.AddRange(gachaData.LegendaryDrawList);
            drawProbability.AddRange(gachaData.MythicalDrawList);
            drawProbability.AddRange(gachaData.CelestialDrawList);

            // �ش� ������ Ȯ�� �����͸� Dictionary�� �߰�
            _cachedProbabilityData[level] = drawProbability;
        }
    }

    public void RefreshUI(EEquipmentType type)
    {
        var equipmentData = Managers.Game.PlayerGameData.DrawData[type];
        _currentType = type;
        _currentLevel = equipmentData.Level; // �ʱ� ���� ����

        UpdateLevelText();
        UpdateDrawProbabilityUI(); // UI ���� �� ĳ�̵� ������ ���
    }

    private void UpdateDrawProbabilityUI()
    {
        // ĳ�̵� �����Ϳ��� ���� ������ Ȯ�� �����͸� ������
        if (_cachedProbabilityData.TryGetValue(_currentLevel, out var drawProbability))
        {
            List<EquipmentInfo> equipmentInfos = Managers.Equipment.GetEquipmentTypeInfos(_currentType);

            for (int i = 0; i < drawPbItems.Count; i++)
            {
                if (i < drawProbability.Count)
                    drawPbItems[i].RefreshUI(equipmentInfos[i], drawProbability[i]);
            }
        }
    }

    // Left ��ư Ŭ�� �� ���� ����
    private void OnClickLeft()
    {
        if (_currentLevel > 1) // �ּ� ������ 1�� ����
        {
            _currentLevel--;
            UpdateDrawProbabilityUI(); // ���� ���� �� UI ����
            UpdateLevelText(); // �ؽ�Ʈ ������Ʈ
        }
    }

    // Right ��ư Ŭ�� �� ���� ����
    private void OnClickRight()
    {
        if (_currentLevel < 10) // �ִ� ���� 10
        {
            _currentLevel++;
            UpdateDrawProbabilityUI(); // ���� ���� �� UI ����
            UpdateLevelText(); // �ؽ�Ʈ ������Ʈ
        }
    }

    private void UpdateLevelText()
    {
        GetTMPText((int)Texts.Text_DrawTypeLevel).text = $"{Util.GetEquipmentString(_currentType)} �̱� Lv. {_currentLevel}";
    }
}
