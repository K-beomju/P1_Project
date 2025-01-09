using UnityEngine.UI;

public class InGameScene_Ranking : UI_Base
{
    private Button _rankingBtn;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _rankingBtn = GetComponent<Button>();
        _rankingBtn.onClick.AddListener(OnClickButton);

        return true;
    }

    private void OnClickButton()
    {
       if (Managers.Backend.Rank.List.Count <= 0)
            {
                Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification("랭킹 미존재 오류 " + "랭킹이 존재하지 않습니다.\n랭킹을 생성해주세요.");
                return;
            }
        (Managers.UI.SceneUI as UI_GameScene).ShowTab(UI_GameScene.PlayTab.Rank);
    }
    

}
