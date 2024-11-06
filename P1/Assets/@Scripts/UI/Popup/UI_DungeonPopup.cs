using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_DungeonPopup : UI_Popup
{
    public enum Texts 
    {
        Text_Timer
    }

    public enum UI_DungeonInfoItems 
    {
        UI_DungeonInfoItem_Gold,
        UI_DungeonInfoItem_Dia,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));
        Bind<UI_DungeonInfoItem>(typeof(UI_DungeonInfoItems));

        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Gold).SetInfo(EDungeonType.Gold);
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Dia).SetInfo(EDungeonType.Dia);

        return true;
    }

    public void RefreshUI()
    {
        GetTMPText((int)Texts.Text_Timer).text = $"남은 시간: {Managers.Backend.GameData.DungeonData.RemainChargeHour}시간"; 
    
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Gold).RefreshUI();
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Dia).RefreshUI();

    }
}
