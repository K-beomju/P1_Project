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

    private GameObject _drawItemGroup;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        _drawItemGroup = GetObject((int)GameObjects.DrawItemGroup);
        return true;
    }

    public void ResultDrawUI(List<EquipmentDrawResult> resultList)
    {
        foreach (var result in resultList)
        {
            var item = Managers.UI.MakeSubItem<UI_EquipmentDrawItem>(_drawItemGroup.transform);
            item.SetInfo(result.RareType, result.EquipmentIndex);
        }
    }


}
