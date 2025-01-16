using System.Collections;
using UnityEngine;
using static Define;

public class UI_DisplayAdBuffItem : UI_Base
{
    public enum Images
    {
        Image_BuffIcon
    }

    public enum Texts
    {
        Text_RemainBuffTime
    }

    public EAdBuffType BuffType { get; set; }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));

        return true;
    }

    public void SetInfo(EAdBuffType buffType, int durationMinutes)
    {
        BuffType = buffType;
        Managers.Buff.StartBuff(buffType, durationMinutes);

        GetImage((int)Images.Image_BuffIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Buff/{buffType}");
        UpdateRemainingTimeText();
    }

    public void UpdateRemainingTimeText()
    {
        int remainMinutes = Managers.Buff.GetRemainingTime(BuffType);
        string formattedTime = remainMinutes < 10 ? $"0{remainMinutes}M" : $"{remainMinutes}M";
        GetTMPText((int)Texts.Text_RemainBuffTime).text = formattedTime;
    }

}
