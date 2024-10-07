using BackendData.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static UnityEditor.Progress;

public class UI_DrawResultPopup : UI_Popup
{
    public enum GameObjects
    {
        DrawItemGroup
    }

    public enum Texts
    {
        Text_DrawLevel,
        Text_Retry
    }

    public enum Buttons
    {
        Btn_Exit,
        Btn_AutoDraw,
        Btn_RetryDraw
    }

    private List<UI_EquipmentItem> _drawItems = new List<UI_EquipmentItem>();
    private EEquipmentType _type;

    private int _level;
    private int _drawCount;
    private bool _drawDirection;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(() =>
        {
            ClosePopupUI();
            Managers.Event.TriggerEvent(EEventType.DrawEquipmentUIUpdated);
        });
        GetButton((int)Buttons.Btn_RetryDraw).onClick.AddListener(() => RetryDrawEquipment());
        for (int i = 0; i < 30; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentItem>(GetObject((int)GameObjects.DrawItemGroup).transform);
            item.gameObject.SetActive(false);
            _drawItems.Add(item);
        }

        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {level}"));
    }

    private void OnDisable()
    {
        Managers.Event.AddEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
         GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {level}"));
    }

    public void RefreshUI(EEquipmentType type, int drawCount, List<int> resultList, bool drawDirection)
    {
        _type = type;
        _level = Managers.Backend.GameData.DrawLevelData.DrawDic[_type.ToString()].DrawLevel;  // 최신 레벨 가져오기
        _drawCount = drawCount;
        _drawDirection = drawDirection;

        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetEquipmentString(_type)} 뽑기 Lv. {_level}";
        GetTMPText((int)Texts.Text_Retry).text = $"{_drawCount}회";

        InteractiveButtons(false);
        StartCoroutine(CreateEquipmentItem(resultList));
    }

    private void RetryDrawEquipment()
    {
        List<int> resultList = Util.GetEquipmentDrawResults(_type, _drawCount, _level);
        RefreshUI(_type, _drawCount, resultList, _drawDirection);
    }

    private IEnumerator CreateEquipmentItem(List<int> resultList)
    {
        _drawItems.ForEach(item => item.gameObject.SetActive(false));

        WaitForSeconds wait = new WaitForSeconds(CREATRE_EQUIPMENT_DELAY);

        // 결과 목록에 해당하는 장비 데이터를 한 번에 미리 검증하여 유효한지 확인
        foreach (var resultId in resultList)
        {
            if (!Managers.Equipment.AllEquipmentInfos.ContainsKey(resultId))
            {
                Debug.LogWarning($"Equipment.AllEquipmentInfo에 장비 ID {resultId}가 없습니다.");
                yield break;
            }
        }

        // 실제로 장비 아이템을 생성하고 UI에 적용
        for (int i = 0; i < Mathf.Min(resultList.Count, _drawItems.Count); i++)
        {
            int resultId = resultList[i];

            if (!Managers.Equipment.AllEquipmentInfos.TryGetValue(resultId, out EquipmentInfoData equipmentInfoData))
            {
                continue;
            }

            try
            {
                // 장비 인벤토리에 추가
                Managers.Backend.GameData.EquipmentInventory.AddEquipment(equipmentInfoData.DataTemplateID);

                // 아이템 설정 및 활성화
                UI_EquipmentItem drawItem = _drawItems[i];
                drawItem.SetDrawInfo(equipmentInfoData);
                drawItem.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"CreateEquipmentItem({equipmentInfoData}, {equipmentInfoData.DataTemplateID}) 중 에러가 발생하였습니다: {e}");
                yield break;  // 에러 발생 시, 더 이상 아이템을 생성하지 않고 종료
            }

            if(!_drawDirection)
            yield return wait;
        }

        InteractiveButtons(true);
    }

    public void InteractiveButtons(bool active)
    {
        for (int i = 0; i < 3; i++)
        {
            GetButton(i).interactable = active;
        }
    }


}
