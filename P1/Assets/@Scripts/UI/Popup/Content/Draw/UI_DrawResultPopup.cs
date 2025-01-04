using BackendData.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

    private List<UI_CompanionItem> _drawItems = new List<UI_CompanionItem>();
    private EDrawType _type;

    private int _level;
    private int _drawCount;
    private bool _drawDirection;
    private bool _autoDraw = false;
    
    private int _testSpendDia = 0;


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

            if (_type.IsEquipmentType())
                Managers.Event.TriggerEvent(EEventType.DrawEquipmentUIUpdated);
            if (_type == EDrawType.Skill)
                Managers.Event.TriggerEvent(EEventType.DrawSkillUIUpdated);
        });
        GetButton((int)Buttons.Btn_RetryDraw).onClick.AddListener(() => 
        {
            int price = _drawCount == 10 ? 500 : 1500;
            if(CanDraw(price))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, -price);
                RetryDrawItem();
            }
            else
            ShowAlertUI("다이아가 부족합니다");
        });
        GetButton((int)Buttons.Btn_AutoDraw).onClick.AddListener(() => 
        {
            int price = _drawCount == 10 ? 500 : 1500;
            if(CanDraw(price))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, -price);
                AutoDrawItem();
            }
            else
            ShowAlertUI("다이아가 부족합니다");
        });
        
        for (int i = 0; i < 30; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_CompanionItem>(GetObject((int)GameObjects.DrawItemGroup).transform);
            item.gameObject.SetActive(false);
            _drawItems.Add(item);
        }

        return true;
    }

    private void OnEnable()
    {
        _autoDraw = false;
        Managers.Event.AddEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetDrawTypeString(_type)} 뽑기 Lv. {level}"));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.DrawLevelUpUIUpdated, new Action<int>((level) =>
        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetDrawTypeString(_type)} 뽑기 Lv. {level}"));
    }

    public void RefreshUI(EDrawType type, int drawCount, List<int> resultList, bool drawDirection)
    {
        _type = type;
        _level = Managers.Backend.GameData.DrawLevelData.DrawDic[_type.ToString()].DrawLevel;  // 최신 레벨 가져오기
        _drawCount = drawCount;
        _drawDirection = drawDirection;

        GetTMPText((int)Texts.Text_DrawLevel).text = $"{Util.GetDrawTypeString(_type)} 뽑기 Lv. {_level}";
        GetTMPText((int)Texts.Text_Retry).text = $"{_drawCount}회";

        InteractiveButtons(false);
        StartCoroutine(CreateEquipmentItem(resultList));
    }

    private void AutoDrawItem()
    {
        // 자동 뽑기 실행중이면 Exit로 끄게 
        if(_autoDraw == true)
        return;

        _autoDraw = true;
        List<int> resultList = Util.GetDrawSystemResults(_type, _drawCount, _level);
        RefreshUI(_type, _drawCount, resultList, _drawDirection);
    }

    private void RetryDrawItem()
    {
        List<int> resultList = Util.GetDrawSystemResults(_type, _drawCount, _level);
        RefreshUI(_type, _drawCount, resultList, _drawDirection);
    }

    private IEnumerator CreateEquipmentItem(List<int> resultList)
    {
        _testSpendDia += 30;
        _drawItems.ForEach(item => item.gameObject.SetActive(false));

        WaitForSeconds wait = new WaitForSeconds(0.03f);
        WaitForSeconds endDrawWait = new WaitForSeconds(0.3f);

        // 결과 목록에 해당하는 장비 데이터를 한 번에 미리 검증하여 유효한지 확인
        foreach (var resultId in resultList)
        {
            if (_type.IsEquipmentType())
            {
                if (!Managers.Equipment.AllEquipmentInfos.ContainsKey(resultId))
                {
                    Debug.LogWarning($"Equipment.AllEquipmentInfo에 장비 ID {resultId}가 없습니다.");
                    yield break;
                }
            }
            if (_type == EDrawType.Skill)
            {
                if (!Managers.Skill.AllSkillInfos.ContainsKey(resultId))
                {
                    Debug.LogWarning($"Equipment.AllSkillInfos에 스킬 ID {resultId}가 없습니다.");
                    yield break;
                }
            }
        }

        // 실제로 장비 아이템을 생성하고 UI에 적용
        for (int i = 0; i < Mathf.Min(resultList.Count, _drawItems.Count); i++)
        {
            int resultId = resultList[i];
            Item itemData = null;


            if (_type.IsEquipmentType())
            {
                if (!Managers.Equipment.AllEquipmentInfos.TryGetValue(resultId, out EquipmentInfoData equipmentInfoData))
                {
                    continue;
                }
                itemData = equipmentInfoData;
            }
            if (_type == EDrawType.Skill)
            {
                if (!Managers.Skill.AllSkillInfos.TryGetValue(resultId, out SkillInfoData skillInfoData))
                {
                    continue;
                }
                itemData = skillInfoData;
            }


            try
            {
                if (_type.IsEquipmentType())
                {
                    // 장비 인벤토리에 추가
                    Managers.Backend.GameData.EquipmentInventory.AddEquipment(itemData.DataTemplateID);
                    Managers.Backend.GameData.QuestData.UpdateQuest(EQuestType.DrawEquipment);

                }
                if (_type == EDrawType.Skill)
                {
                    // 스킬 인벤토리에 추가
                    Managers.Backend.GameData.SkillInventory.AddSkill(itemData.DataTemplateID);
                    Managers.Backend.GameData.QuestData.UpdateQuest(EQuestType.DrawSkill);
                }
                UI_CompanionItem drawItem = _drawItems[i];
                drawItem.DisplayItem(itemData, EItemDisplayType.Draw);
                drawItem.gameObject.SetActive(true);

            }
            catch (Exception e)
            {
                Debug.LogError($"CreateEquipmentItem({itemData}, {itemData.DataTemplateID}) 중 에러가 발생하였습니다: {e}");
                yield break;  // 에러 발생 시, 더 이상 아이템을 생성하지 않고 종료
            }

            if (!_drawDirection)
                yield return wait;
        }

        Debug.LogWarning($"뽑기에 쓴 다이아 {_testSpendDia} -> 시도 횟수 {_testSpendDia / 30}");

        Managers.Hero.PlayerHeroInfo.CalculateInfoStat((changed) =>
        {
            if (changed)
            {
                Debug.LogWarning("전투력 변경 있음.");
                Managers.UI.ShowBaseUI<UI_TotalPowerBase>().ShowTotalPowerUI();
            }
            else
            {
                Debug.LogWarning("전투력 변경 없음.");
            }
        });

        GetButton((int)Buttons.Btn_Exit).interactable = true;
        yield return endDrawWait;

        if(_autoDraw)
        {
            int price = _drawCount == 10 ? DrawPrice.DrawTenPrice : DrawPrice.DrawThirtyPrice;
            if(CanDraw(price))
            {
                Managers.Backend.GameData.CharacterData.AddAmount(EItemType.Dia, -price);
                List<int> autoDrawList = Util.GetDrawSystemResults(_type, _drawCount, _level);
                RefreshUI(_type, _drawCount, autoDrawList, _drawDirection);
            }
            else
            {
                ShowAlertUI("다이아가 부족합니다");
                yield break;
            }
            yield break;
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

    bool CanDraw(float cost)
    {
        if (!Managers.Backend.GameData.CharacterData.PurseDic.TryGetValue(EItemType.Dia.ToString(), out double amount))
            return false;

        return amount >= cost;
    }

}
