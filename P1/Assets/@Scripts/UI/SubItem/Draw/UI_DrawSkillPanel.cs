using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DrawSkillPanel : UI_Base
{
    public enum Texts
    {
        Text_DrawSkillLevel,
        Text_DrawValue
    }

    public enum Sliders
    {
        Slider_DrawCount
    }

    public enum Buttons
    {
        Btn_GachaProbability,
        Btn_SkipDrawVisual,
        Btn_DrawTenAd,
        Btn_DrawTen,
        Btn_DrawThirty,
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTMPTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_GachaProbability).onClick.AddListener(() => ShowProbabilityPopup());

        GetButton((int)Buttons.Btn_DrawTenAd).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawTen).onClick.AddListener(() => OnDrawEquipment(10));
        GetButton((int)Buttons.Btn_DrawThirty).onClick.AddListener(() => OnDrawEquipment(30));

        return true;
    }

    public void RefreshUI()
    {

    }


    #region Draw Logic

    private void OnDrawEquipment(int drawCount)
    {
        var popupUI = Managers.UI.ShowPopupUI<UI_DrawResultPopup>();
        Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_RESULTPOPUP);

        //var equipmentIdList = Util.GetEquipmentDrawResults(_currentEquipmentType, drawCount, _drawLevel);
        //popupUI.RefreshUI(_currentEquipmentType, drawCount, equipmentIdList);
    }

    private void ShowProbabilityPopup()
    {
        //Managers.UI.ShowPopupUI<UI_DrawProbabilityPopup>().RefreshUI(_currentEquipmentType);
    }

    #endregion
}
