using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class UI_ItemGainPopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        Content_Item
    }

    private List<UI_GainedItem> _relicItems = new List<UI_GainedItem>();
    private List<UI_ClearItem> _clearItems = new List<UI_ClearItem>();


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        GetObject((int)GameObjects.BG).BindEvent(() =>
        {
            _clearItems.ForEach(item => item.gameObject.SetActive(false));
            _relicItems.ForEach(item => item.gameObject.SetActive(false));
            ClosePopupUI();

        });
        for (int i = 0; i < 30; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_GainedItem>(GetObject((int)GameObjects.Content_Item).transform);
            item.gameObject.SetActive(false);
            _relicItems.Add(item);
        }

        for (int i = 0; i < 10; i++)
        {
            var item = Managers.UI.MakeSubItem<UI_ClearItem>(GetObject((int)GameObjects.Content_Item).transform);
            item.gameObject.SetActive(false);
            _clearItems.Add(item);
        }

        return true;
    }

  public void ShowCreateRelicItem(Dictionary<EHeroRelicType, int> relicDic)
{
    // Content_Item 애니메이션 시작
    var contentItem = GetObject((int)GameObjects.Content_Item).transform;
    contentItem.localScale = new Vector3(0.9f, 0.9f, 1f); // 살짝 축소된 상태에서 시작
    contentItem.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear) // 부드러운 확장
        .OnComplete(() =>
        {
            // 애니메이션이 완료된 후 아이템 생성 시작
            StartCoroutine(CreateRelicItems(relicDic));
        });
}

public void ShowCreateClearItem(Dictionary<EItemType, int> itemDic)
{
    // Content_Item 애니메이션 시작
    var contentItem = GetObject((int)GameObjects.Content_Item).transform;
    contentItem.localScale = new Vector3(0.9f, 0.9f, 1f); // 살짝 축소된 상태에서 시작
    contentItem.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear) // 부드러운 확장
        .OnComplete(() =>
        {
            // 애니메이션이 완료된 후 아이템 생성 시작
            StartCoroutine(CreateClearItems(itemDic));
        });
}



    private IEnumerator CreateRelicItems(Dictionary<EHeroRelicType, int> relicDic)
    {

        int relicIndex = 0;
        foreach (var item in relicDic)
        {
            if (relicIndex < _relicItems.Count)
            {
                var relicItem = _relicItems[relicIndex];
                DisplayRelicItem(relicItem, item);

                // DOTween 애니메이션 추가
                relicItem.transform.localScale = Vector3.zero; // 초기 스케일 설정
                relicItem.gameObject.SetActive(true); // 활성화
                relicItem.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce); // 스케일 애니메이션

                relicIndex++;
                yield return new WaitForSeconds(0.05f); // 아이템 간 애니메이션 간격 추가
            }
        }

        Managers.Event.TriggerEvent(EEventType.HeroRelicUpdated);
        yield return null;
    }

    private IEnumerator CreateClearItems(Dictionary<EItemType, int> itemDic)
    {

        int clearIndex = 0;
        foreach (var item in itemDic)
        {
            if (clearIndex < _clearItems.Count)
            {
                var clearItem = _clearItems[clearIndex];
                DisplayClearItem(clearItem, item);

                // DOTween 애니메이션 추가
                clearItem.transform.localScale = Vector3.zero; // 초기 스케일 설정
                clearItem.gameObject.SetActive(true); // 활성화
                clearItem.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce); // 스케일 애니메이션

                clearIndex++;
                yield return new WaitForSeconds(0.05f); // 아이템 간 애니메이션 간격 추가
            }
        }

        yield return null;
    }



    private void DisplayRelicItem(UI_GainedItem relicItem, KeyValuePair<EHeroRelicType, int> item)
    {
        relicItem.DisplayItem(Managers.Data.RelicChart[item.Key], item.Value);
        relicItem.gameObject.SetActive(true);
    }

    private void DisplayClearItem(UI_ClearItem clearItem, KeyValuePair<EItemType, int> item)
    {
        clearItem.DisplayItem(Managers.Data.ItemChart[item.Key], item.Value);
        clearItem.gameObject.SetActive(true);
    }
}


