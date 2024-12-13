using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_PrologPopup : UI_Popup
{
    public enum Buttons
    {
        Button_GameStart
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        GetButton((int)Buttons.Button_GameStart).onClick.AddListener(() =>
        {
            var fadeUI = Managers.UI.ShowBaseUI<UI_FadeInBase>();
            fadeUI.sceneMove = true;
            Managers.UI.SetCanvas(fadeUI.gameObject, false, SortingLayers.UI_FADEPOPUP);

            fadeUI.ShowFadeInOut(EFadeType.FadeOut, 1f, 1f,
            fadeOutCallBack: () =>
            {
                Managers.Scene.GetCurrentScene<TitleScene>().InGameStart();
            });
        });
        return true;
    }
}
