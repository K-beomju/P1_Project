using BackendData.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BatchEnhancePopup : UI_Popup
{
    public enum GameObjects
    {
        BG,
        ItemGroup
    }

    public enum Buttons
    {
        Button_Exit
    }

    private List<UI_CompanionItem> equipmentItems = new List<UI_CompanionItem>();
    private WaitForSeconds createDelay = new WaitForSeconds(0.1f);
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetObject((int)GameObjects.BG).gameObject.BindEvent(ClosePopupUI, Define.EUIEvent.Click);
        GetButton((int)Buttons.Button_Exit).onClick.AddListener(ClosePopupUI);

        // 기본 값 10개 생성하고 더 필요하면 추후에 생성해서 담아두는 방식으로 
        for (int i = 0; i < 10; i++)
        {
            GameObject parent = GetObject((int)GameObjects.ItemGroup);
            UI_CompanionItem item = Managers.UI.MakeSubItem<UI_CompanionItem>(parent.transform);
            if (item != null)
            {
                equipmentItems.Add(item);
            }
        }

        return true;
    }

    public void ShowBatchEnhanceItem(Dictionary<EquipmentInfoData, int> enhanceEquipmentDic, List<EquipmentInfoData> equipmentInfos)
    {
        equipmentItems.ForEach(item => item.gameObject.SetActive(false));
        StartCoroutine(ShowBatchEnhanceItemCo(enhanceEquipmentDic, equipmentInfos));
    }

    private IEnumerator ShowBatchEnhanceItemCo(Dictionary<EquipmentInfoData, int> enhanceEquipmentDic, List<EquipmentInfoData> equipmentInfos)
    {
        int index = 0;
        foreach (var equipmentInfo in equipmentInfos)
        {
            if (enhanceEquipmentDic.ContainsKey(equipmentInfo))
            {
                // 필요한 만큼 UI 아이템 생성
                if (index >= equipmentItems.Count)
                {
                    GameObject parent = GetObject((int)GameObjects.ItemGroup);
                    UI_CompanionItem newItem = Managers.UI.MakeSubItem<UI_CompanionItem>(parent.transform);
                    equipmentItems.Add(newItem);
                }

                UI_CompanionItem item = equipmentItems[index];
                item.gameObject.SetActive(true);

                item.DisplayEnhanceLevel(enhanceEquipmentDic[equipmentInfo], equipmentInfo);
                index++;
                yield return createDelay;
            }
        }
    }

}
