using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_RelicsPanel : UI_Base
{
    public enum Buttons
    {
        Btn_DrawTen,
        Btn_DrawThirty
    }

    public enum UI_RelicGrowthSlots
    {
        UI_RelicGrowInvenSlot_Atk,
        UI_RelicGrowInvenSlot_MaxHp,
        UI_RelicGrowInvenSlot_Recovery,
        UI_RelicGrowInvenSlot_MonsterDmg,
        UI_RelicGrowInvenSlot_BossMonsterDmg,
        UI_RelicGrowInvenSlot_ExpRate,
        UI_RelicGrowInvenSlot_GoldRate
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_RelicGrowInvenSlot>(typeof(UI_RelicGrowthSlots));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawRelic(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawRelic(100));

        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_Atk).SetInfo(EHeroRelicType.Relic_Atk);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_MaxHp).SetInfo(EHeroRelicType.Relic_MaxHp);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_Recovery).SetInfo(EHeroRelicType.Relic_Recovery);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_MonsterDmg).SetInfo(EHeroRelicType.Relic_MonsterDmg);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_BossMonsterDmg).SetInfo(EHeroRelicType.Relic_BossMonsterDmg);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_ExpRate).SetInfo(EHeroRelicType.Relic_ExpRate);
        Get<UI_RelicGrowInvenSlot>((int)UI_RelicGrowthSlots.UI_RelicGrowInvenSlot_GoldRate).SetInfo(EHeroRelicType.Relic_GoldRate);

        return true;
    }

    private void OnDrawRelic(int drawCount)
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_ItemGainPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_RESULTPOPUP);

        if (AreAllRelicsMaxedOut())
        {
            ShowAllRelicsMaxedPopup();
            return;
        }

        Dictionary<Enum, int> drawRelicDic = new();
        Array values = Enum.GetValues(typeof(EHeroRelicType));

        int attempts = 0;
        for (int i = 0; i < drawCount; i++)
        {
            // 최대 시도 횟수를 설정하여 무한 루프 방지
            if (attempts > drawCount * 2) break;

            int random = UnityEngine.Random.Range(0, values.Length);
            EHeroRelicType selectedRelic = (EHeroRelicType)values.GetValue(random);

            int currentCount = Managers.Backend.GameData.UserData.OwnedRelicDic[selectedRelic.ToString()];
            int maxCount = Managers.Data.HeroRelicChart[selectedRelic].MaxCount;

            // 유물이 MaxCount 미만일 경우에만 drawRelicDic에 추가
            if (currentCount < maxCount)
            {
                if (drawRelicDic.ContainsKey(selectedRelic))
                {
                    drawRelicDic[selectedRelic]++;
                }
                else
                {
                    drawRelicDic.Add(selectedRelic, 1);
                }
                attempts = 0; // 추가가 성공한 경우 시도 횟수 초기화
            }
            else
            {
                i--; // 뽑기 실패시 i 유지
                attempts++; // 시도 횟수 증가
            }

            // 모든 유물이 MaxCount에 도달한 경우 중지
            if (AreAllRelicsMaxedOut())
            {
                ShowAllRelicsMaxedPopup();
                break;
            }
        }


        // 실제 추가 로직: drawRelicDic을 사용하여 AddRelic 호출
        foreach (var relic in drawRelicDic)
        {
            int availableSpace = Managers.Data.HeroRelicChart[(EHeroRelicType)relic.Key].MaxCount - Managers.Backend.GameData.UserData.OwnedRelicDic[relic.Key.ToString()];
            int countToAdd = Mathf.Min(relic.Value, availableSpace);

            // AddRelic을 호출하여 초과하지 않는 개수만 추가
            Managers.Backend.GameData.UserData.AddRelic((EHeroRelicType)relic.Key, countToAdd);
        }

        popupUI.RefreshUI(EItemType.Relic, drawRelicDic);
    }

    // 모든 유물이 MaxCount에 도달했는지 확인하는 메서드
    private bool AreAllRelicsMaxedOut()
    {
        foreach (var relicPair in Managers.Backend.GameData.UserData.OwnedRelicDic)
        {
            var relicType = (EHeroRelicType)System.Enum.Parse(typeof(EHeroRelicType), relicPair.Key);
            if (relicPair.Value < Managers.Data.HeroRelicChart[relicType].MaxCount)
            {
                return false; // 하나라도 MaxCount 미만이면 false 반환
            }
        }
        return true; // 모든 유물이 MaxCount에 도달했을 때 true 반환
    }

    private void ShowAllRelicsMaxedPopup()
    {
        Debug.Log("모든 유물이 최대 개수에 도달했습니다!");
        // 실제 UI 팝업 호출 코드 추가
    }

}
