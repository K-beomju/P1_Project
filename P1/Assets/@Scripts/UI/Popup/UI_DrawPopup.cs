using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class UI_DrawPopup : UI_Popup
{
    public enum Buttons
    {
        Btn_DrawOnce,
        Btn_DrawTen,
        Btn_DrawThirty
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Btn_DrawOnce).onClick.AddListener(() => OnDrawEquipment(1));
        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawEquipment(30));
        return true;
    }

    public void OnDrawEquipment(int count)
    {
        int level = 1;
        var gachaData = Managers.Data.GachaDataDic[level];

        for (int i = 0; i < count; i++)
        {
            ERareType rareType = GetRandomRareType(gachaData.DrawProbability);
            int equipmentIndex = GetEquipmentIndexForRareType(gachaData, rareType);

            Debug.Log($"{rareType} 뽑은 장비 인덱스 {equipmentIndex}");
        }
    }

    private ERareType GetRandomRareType(List<float> drawProbability)
    {
        return Util.GetRareType(Util.GetDrawProbabilityType(drawProbability));
    }

    private int GetEquipmentIndexForRareType(DrawEquipmentGachaData gachaData, ERareType rareType)
    {
        switch (rareType)
        {
            case ERareType.Normal:
                return Util.GetDrawProbabilityType(gachaData.NormalDrawList);
            case ERareType.Advanced:
                return Util.GetDrawProbabilityType(gachaData.AdvancedDrawList);
            case ERareType.Rare:
                return Util.GetDrawProbabilityType(gachaData.RareDrawList);
            case ERareType.Legendary:
                return Util.GetDrawProbabilityType(gachaData.LegendaryDrawList);
            case ERareType.Mythical:
                return Util.GetDrawProbabilityType(gachaData.MythicalDrawList);
            case ERareType.Celestial:
                return Util.GetDrawProbabilityType(gachaData.CelestialDrawList);
            default:
                Debug.LogWarning($"Unknown rare type: {rareType}");
                return -1;
        }
    }
}
