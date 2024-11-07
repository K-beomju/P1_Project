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
        UI_DungeonInfoItem_WorldBoss
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindTMPTexts(typeof(Texts));
        Bind<UI_DungeonInfoItem>(typeof(UI_DungeonInfoItems));

        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Gold).SetInfo(EDungeonType.Gold);
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Dia).SetInfo(EDungeonType.Dia);
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_WorldBoss).SetInfo(EDungeonType.WorldBoss);

        return true;
    }

    public void RefreshUI()
    {
        var data = Managers.Backend.GameData.DungeonData;

        // 열쇠 지급 가능한 타임인지 체크 
        data.CheckDungeonKeyRecharge();

        GetTMPText((int)Texts.Text_Timer).text = $"남은 시간: {data.RemainChargeHour}시간"; 
    
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Gold).RefreshUI();
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_Dia).RefreshUI();
        Get<UI_DungeonInfoItem>((int)UI_DungeonInfoItems.UI_DungeonInfoItem_WorldBoss).RefreshUI();

    }
}
