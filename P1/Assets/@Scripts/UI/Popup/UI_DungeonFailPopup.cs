using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DungeonFailPopup : UI_Popup
{
    public enum GameObjects
    {
        BG
    }

    public enum Buttons
    {
        Btn_Exit,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetObject((int)GameObjects.BG).BindEvent(OnClickButton);
        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(OnClickButton);

        return true;
    }

    private void OnClickButton()
    {
        var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
        fadeUI.sceneMove = true;
        Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_FADEPOPUP);

        fadeUI.ShowFadeInOut(EFadeType.FadeInOut, 1, 1, 
        fadeOutCallBack: () => 
        {
            Managers.Scene.LoadScene(EScene.GameScene);
        });
    }
}
