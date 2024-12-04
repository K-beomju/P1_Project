using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Data;
using System.Linq;

public class UI_QuestPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        QuestContent
    }

    public enum Buttons
    {
        Btn_Exit,
        Btn_Daily,
        Btn_Repeatable,
        Btn_Achievement
    }

    private Dictionary<EQuestCategory, List<UI_QuestItem>> questItemDic = new();
    private EQuestCategory currentQuestCategory;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        GetObject((int)GameObjects.BG).BindEvent(ClosePopupUI);

        GetButton((int)Buttons.Btn_Daily).onClick.AddListener(() => OnClickButtonChangeCategory(EQuestCategory.Daily));
        GetButton((int)Buttons.Btn_Repeatable).onClick.AddListener(() => OnClickButtonChangeCategory(EQuestCategory.Repeatable));
        GetButton((int)Buttons.Btn_Achievement).onClick.AddListener(() => OnClickButtonChangeCategory(EQuestCategory.Achievement));

        // 퀘스트 항목 생성 및 초기화
        foreach (var quest in Managers.Data.QuestChart)
        {
            var questCategory = quest.Value.QuestCategory;
            var parent = GetObject((int)GameObjects.QuestContent).transform;
            var item = Managers.UI.MakeSubItem<UI_QuestItem>(parent);

            if (!questItemDic.TryGetValue(questCategory, out var itemList))
            {
                itemList = new List<UI_QuestItem>();
                questItemDic[questCategory] = itemList;
            }
            item.SetInfo(quest.Value);
            itemList.Add(item);

            item.gameObject.SetActive(false);
        }

        // 기본 카테고리 설정 및 UI 갱신
        currentQuestCategory = EQuestCategory.Daily;
        return true;
    }

    private void OnClickButtonChangeCategory(EQuestCategory questCategory)
    {
        if (currentQuestCategory == questCategory) return; // 동일 카테고리 클릭 방지
        currentQuestCategory = questCategory;
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var (category, items) in questItemDic)
        {
            bool isActive = category == currentQuestCategory;
            items.ForEach(item => 
            {
                item.gameObject.SetActive(isActive);
                if(item.gameObject.activeSelf)
                {
                    item.RefreshUI();
                }
            });
        }
    }
}
