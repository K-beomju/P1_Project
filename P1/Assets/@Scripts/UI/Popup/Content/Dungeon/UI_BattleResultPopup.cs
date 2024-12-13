using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_BattleResultPopup : UI_Popup
{
    public enum GameObjects
    {
        BG
    }
    public enum Texts
    {
        Text_BattleResult
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));

        GetObject((int)GameObjects.BG).BindEvent(OnClickButton);

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


    public void RefreshUI(float worldBossTotalDamage)
    {
        GetTMPText((int)Texts.Text_BattleResult).text = Util.ConvertToTotalCurrency((long)worldBossTotalDamage);
    }
}
