using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_StageDisplayBase : UI_Base
{
    enum Texts 
    {
        StageDisplayText
    }

    protected override bool Init()
    {
        if(base.Init() == false)
        return false;

        BindTexts(typeof(Texts));
        return true;
    }

    public void RefreshShowDisplayStage(int stage)
    {
        Text text = GetText((int)Texts.StageDisplayText);
        text.text = $"스테이지 {stage} !!";
        text.DOFade(0, 0.5f).SetDelay(1).OnComplete(() => 
        {
            Managers.Resource.Destroy(this.gameObject);
        });
    }
}
