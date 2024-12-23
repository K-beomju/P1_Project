using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PetProbabilityPopup : UI_Popup
{
    public enum Buttons
    {
        Btn_Exit
    }

    public enum GameObjects
    {
        BG,
        ItemGroup
    }

    private List<UI_PetProbabilityItem> items = new();
    private bool _init = false;

    protected override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        GetObject((int)GameObjects.BG).gameObject.BindEvent(ClosePopupUI);

        foreach (var pet in Managers.Data.PetChart)
        {
            UI_PetProbabilityItem item = Managers.UI.MakeSubItem<UI_PetProbabilityItem>(GetObject((int)GameObjects.ItemGroup).transform);
            item.SetInfo(pet.Value.ChapterLevel, pet.Value.PetCraftSpriteKey, pet.Value.DropCraftItemRate);
            items.Add(item);
        }
        return true;
    }

    public void RefreshUI()
    {
        if (_init)
            return;

        items.ForEach(item => item.RefreshUI());
        _init = true;
    }
}
