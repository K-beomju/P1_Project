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
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawRelic(30));

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
        List<EHeroRelicType> eligibleRelics = new List<EHeroRelicType>();

        // Prepare the list of eligible relics
        foreach (EHeroRelicType relic in values)
        {
            int ownedCount = Managers.Backend.GameData.CharacterData.OwnedRelicDic[relic.ToString()];
            int maxCount = Managers.Data.HeroRelicChart[relic].MaxCount;
            if (ownedCount < maxCount)
            {
                eligibleRelics.Add(relic);
            }
        }

        // Check if there are any eligible relics
        if (eligibleRelics.Count == 0)
        {
            Debug.Log("All relics have reached their maximum count.");
            return;
        }

        for (int i = 0; i < drawCount;)
        {
            if (eligibleRelics.Count == 0)
            {
                Debug.Log("더 이상 뽑을 유물이 없습니다.");
                break;
            }

            int randomIndex = UnityEngine.Random.Range(0, eligibleRelics.Count);
            EHeroRelicType selectedRelic = eligibleRelics[randomIndex];

            // 선택된 유물이 최대치에 도달했는지 확인
            int ownedCount = Managers.Backend.GameData.CharacterData.OwnedRelicDic[selectedRelic.ToString()];
            int maxCount = Managers.Data.HeroRelicChart[selectedRelic].MaxCount;
            if (ownedCount >= maxCount)
            {
                // 유물을 획득 가능한 리스트에서 제거
                eligibleRelics.RemoveAt(randomIndex);
                continue; // 뽑기 횟수를 증가시키지 않고 다음 루프로
            }

            // 유물 획득 및 데이터 업데이트
            if (drawRelicDic.ContainsKey(selectedRelic))
            {
                drawRelicDic[selectedRelic] += 1;
            }
            else
            {
                drawRelicDic.Add(selectedRelic, 1);
            }
            Managers.Backend.GameData.CharacterData.AddRelic(selectedRelic, 1);

            // 유물이 최대치에 도달했는지 다시 확인
            ownedCount++;
            if (ownedCount >= maxCount)
            {
                eligibleRelics.RemoveAt(randomIndex);
            }

            i++; // 뽑기 횟수 증가
        }


        Managers.Hero.PlayerHeroInfo.CalculateInfoStat();
        popupUI.RefreshUI(EItemType.Relic, drawRelicDic);
    }

    // 모든 유물이 MaxCount에 도달했는지 확인하는 메서드
    private bool AreAllRelicsMaxedOut()
    {
        foreach (var relicPair in Managers.Backend.GameData.CharacterData.OwnedRelicDic)
        {
            var relicType = (EHeroRelicType)Enum.Parse(typeof(EHeroRelicType), relicPair.Key);
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
