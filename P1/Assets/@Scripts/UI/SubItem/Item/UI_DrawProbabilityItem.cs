using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackendData.GameData;

using static Define;

public class UI_DrawProbabilityItem : UI_Base
{
    public enum Images
    {
        Image_Rare
    }

    public enum Texts
    {
        Text_EquipmentName,
        Text_EquipmentProbability

    }
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));



        return true;
    }

    public void RefreshUI(EquipmentInfoData equipmentInfo, float drawProbability)
    {
        GetImage((int)Images.Image_Rare).color = Util.GetRareTypeColor(equipmentInfo.Data.RareType);
        GetTMPText((int)Texts.Text_EquipmentName).text = equipmentInfo.Data.Name;
        GetTMPText((int)Texts.Text_EquipmentProbability).text = $"{drawProbability}%";

    }
}
