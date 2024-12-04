using BackEnd;
using BackEnd.Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum Buttons
    {
        ShopButton,
        CharacterButton,
        EquipmentButton,
        SkillButton,
        DungeonButton,
        DrawButton,
        Btn_AutoSkill,
        Btn_Ranking,
        Btn_AdBuff,
        Btn_Post,
        Btn_Logout,
        Btn_Setting,
        Btn_Menu,
        Btn_Quest
    }

    enum Sliders
    {
        Slider_Exp,
        Slider_StageInfo,
        Slider_BossTimer
    }

    enum Texts
    {
        RemainMonsterValueText,
        ExpValueText,
        Text_StageInfo,
        Text_AutoSkill,
        RankUpDescText
    }

    enum Images
    {
        Image_RankUpIcon
    }

    enum GameObjects
    {
        TopStage,
        RemainMonster,
        RankUpStage,
        MenuGroup
    }

    enum CanvasGroups
    {
        SkillSlotGroup,
        BuffGroup
    }

    public enum PlayTab
    {
        None = -1,
        Character,
        Equipment,
        Skill,
        Dungeon,
        Draw,
        Shop,
        Rank
    }

    public enum UI_GoodItems
    {
        UI_GoodItem_Gold,
        UI_GoodItem_Dia,
        UI_GoodItem_ExpPoint,
        UI_GoodItem_AbilityPoint
    }

    public enum DisplayAdBuffItems
    {
        UI_DisplayAdBuffItem_1,
        UI_DisplayAdBuffItem_2,
        UI_DisplayAdBuffItem_3
    }

    public enum EquipSkillSlots
    {
        UI_EquipSkillSlot_1,
        UI_EquipSkillSlot_2,
        UI_EquipSkillSlot_3,
        UI_EquipSkillSlot_4,
        UI_EquipSkillSlot_5,
        UI_EquipSkillSlot_6
    }

    public PlayTab _tab { get; set; } = PlayTab.None;

    public List<UI_EquipSkillSlot> _equipSkillSlotList { get; set; } = new List<UI_EquipSkillSlot>();
    public List<UI_DisplayAdBuffItem> _displayAdBuffList { get; set; } = new List<UI_DisplayAdBuffItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindSliders(typeof(Sliders));
        BindObjects(typeof(GameObjects));
        Bind<CanvasGroup>(typeof(CanvasGroups));
        Bind<UI_GoodItem>(typeof(UI_GoodItems));
        Bind<UI_EquipSkillSlot>(typeof(EquipSkillSlots));
        Bind<UI_DisplayAdBuffItem>(typeof(DisplayAdBuffItems));

        // Main_Content
        GetButton((int)Buttons.ShopButton).onClick.AddListener(() => ShowTab(PlayTab.Shop));
        GetButton((int)Buttons.CharacterButton).onClick.AddListener(() => ShowTab(PlayTab.Character));
        GetButton((int)Buttons.EquipmentButton).onClick.AddListener(() => ShowTab(PlayTab.Equipment));
        GetButton((int)Buttons.SkillButton).onClick.AddListener(() => ShowTab(PlayTab.Skill));
        GetButton((int)Buttons.DungeonButton).onClick.AddListener(() => ShowTab(PlayTab.Dungeon));
        GetButton((int)Buttons.DrawButton).onClick.AddListener(() => ShowTab(PlayTab.Draw));

        GetButton((int)Buttons.Btn_Setting).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_SettingPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SETTINGPOPUP);
        });
        // Sub_Content
        GetButton((int)Buttons.Btn_AutoSkill).onClick.AddListener(ActiveAutoSkill);
        GetButton((int)Buttons.Btn_AdBuff).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_AdBuffPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
        });

        // Menu
        GetButton((int)Buttons.Btn_Menu).onClick.AddListener(() =>
        {
            GameObject menuGroup = GetObject((int)GameObjects.MenuGroup);
            bool isActive = !menuGroup.activeSelf;

            // 메뉴 그룹 활성/비활성 토글
            menuGroup.SetActive(isActive);

            // 버튼 이미지 스프라이트 변경
            Image buttonImage = GetButton((int)Buttons.Btn_Menu).GetComponent<Image>();
            string spritePath = isActive ? "Sprites/Icon_Close02" : "Sprites/Icon_Menu_Hamburger";
            buttonImage.sprite = Managers.Resource.Load<Sprite>(spritePath);
        });
        
        GetButton((int)Buttons.Btn_Ranking).onClick.AddListener(() =>
        {
            if (Managers.Backend.Rank.List.Count <= 0)
            {
                Managers.UI.ShowBaseUI<UI_NotificationBase>().ShowNotification("랭킹 미존재 오류 " + "랭킹이 존재하지 않습니다.\n랭킹을 생성해주세요.");
                return;
            }
            ShowTab(PlayTab.Rank);
        });
        GetButton((int)Buttons.Btn_Post).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_PostPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
            popupUI.RefreshUI();
        });
        GetButton((int)Buttons.Btn_Logout).onClick.AddListener(() =>
        {
            SendQueue.Enqueue(Backend.BMember.Logout, callback =>
            {
                Debug.Log($"Backend.BMember.Logout : {callback}");
                #if UNITY_ANDROID
                TheBackend.ToolKit.GoogleLogin.Android.GoogleSignOut(true, GoogleSignOutCallback);
                #endif

                if (callback.IsSuccess())
                {
                    Debug.Log("로그아웃 성공");
                    Managers.Scene.LoadScene(EScene.TitleScene);
                }
            });
        });

        GetButton((int)Buttons.Btn_Quest).onClick.AddListener(() => 
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_QuestPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
            popupUI.RefreshUI();
        });

        // Good
        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Gold).SetInfo(EItemType.Gold);
        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Dia).SetInfo(EItemType.Dia);
        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_ExpPoint).SetInfo(EItemType.ExpPoint);
        Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_AbilityPoint).SetInfo(EItemType.AbilityPoint);

        InitializeUIElements();

        Managers.Event.AddEvent(EEventType.MonsterCountChanged, new Action<int, int>(RefreshShowRemainMonster));
        Managers.Event.AddEvent(EEventType.ExperienceUpdated, new Action<int, float, float>(RefreshShowExp));

        // BuffManager 이벤트 구독
        Managers.Buff.OnBuffTimeUpdated += UpdateBuffUI;
        Managers.Buff.OnBuffExpired += RemoveBuffUI;

        RefreshUI();

        Managers.Sound.Play(ESound.Bgm,"Sounds/GameBGM", 0.3f);

        return true;
    }

    private void OnDestroy()
    {
        Managers.Buff.OnBuffTimeUpdated -= UpdateBuffUI;
        Managers.Buff.OnBuffExpired -= RemoveBuffUI;
    }

    private void InitializeUIElements()
    {
        // 6개의 슬롯을 초기화
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            _equipSkillSlotList.Add(Get<UI_EquipSkillSlot>(i));
            _equipSkillSlotList[i].SetInfo(index);
        }

        for (int i = 0; i < 3; i++)
        {
            _displayAdBuffList.Add(Get<UI_DisplayAdBuffItem>(i));
            _displayAdBuffList[i].gameObject.SetActive(false);
        }
        LoadExistingBuffs();

        GetSlider((int)Sliders.Slider_Exp).value = 0;
        GetSlider((int)Sliders.Slider_BossTimer).value = 0;
        GetSlider((int)Sliders.Slider_StageInfo).value = 0;

        GetTMPText((int)Texts.ExpValueText).text = string.Empty;
        GetTMPText((int)Texts.RemainMonsterValueText).text = string.Empty;

        GetGoodItem(EItemType.ExpPoint).gameObject.SetActive(false);
        GetGoodItem(EItemType.AbilityPoint).gameObject.SetActive(false);
        GetObject((int)GameObjects.RankUpStage).SetActive(false);
        GetObject((int)GameObjects.MenuGroup).SetActive(false);

        UpdateAutoSkillUI(Managers.Backend.GameData.SkillInventory.IsAutoSkill);
    }

    private void RefreshUI()
    {
        if (_init == false)
            return;

        UpdatedSkillSlotUI();

        ShowTab(_tab);
    }

    #region Sub-Content

    private void GoogleSignOutCallback(bool isSuccess, string error)
    {
        if (isSuccess == false)
        {
            Debug.Log("구글 로그아웃 에러 응답 발생 : " + error);
        }
        else
        {
            Debug.Log("로그아웃 성공");
            Managers.Scene.LoadScene(EScene.TitleScene);
        }
    }
    #endregion

    #region Stage UI

    public void UpdateStageUI(EGameSceneState sceneState)
    {
        GetObject((int)GameObjects.RemainMonster).SetActive(sceneState == EGameSceneState.Play);
        GetTMPText((int)Texts.Text_StageInfo).gameObject.
        SetActive(sceneState == EGameSceneState.Play || sceneState == EGameSceneState.Boss || sceneState == EGameSceneState.RankUp);
        GetObject((int)GameObjects.RankUpStage).SetActive(sceneState == EGameSceneState.RankUp);

        switch (sceneState)
        {
            case EGameSceneState.Play:
                GetTMPText((int)Texts.Text_StageInfo).text = $"푸른 초원 {Managers.Scene.GetCurrentScene<GameScene>().GetCurrentStage()}";
                break;
            case EGameSceneState.RankUp:
                ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Pending);
                string rankName = Managers.Data.RankUpChart[rankType].Name;
                GetTMPText((int)Texts.RankUpDescText).text = $"{rankName} 승급 도전 중";
                GetImage((int)Images.Image_RankUpIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{rankType}");

                break;
        }
    }

    public void RefreshShowRemainMonster(int killMonster, int maxMonster)
    {
        GetTMPText((int)Texts.RemainMonsterValueText).text = $"{killMonster} / {maxMonster}";

        GetSlider((int)Sliders.Slider_StageInfo).maxValue = maxMonster;
        GetSlider((int)Sliders.Slider_StageInfo).value = killMonster;
    }

    public void RefreshBossMonsterHp(Creature monster)
    {
        int currentHp = Mathf.FloorToInt(monster.Hp); // 소수점 버림
        int maxHp = Mathf.FloorToInt(monster.MaxHp); // 소수점 버림

        float hpAmount = (float)currentHp / maxHp;
        GetSlider((int)Sliders.Slider_StageInfo).maxValue = 1;
        GetSlider((int)Sliders.Slider_StageInfo).value = hpAmount;
        GetTMPText((int)Texts.Text_StageInfo).text = $"{currentHp} / {maxHp}";
    }

    public void RefreshBossStageTimer(float currentTime, float maxTime)
    {
        float timePercentage = Mathf.Clamp01(currentTime / maxTime); // 0~1 범위로 제한
        GetSlider((int)Sliders.Slider_BossTimer).value = timePercentage; // 슬라이더 값 설정
    }

    #endregion

    #region Good, Dia,

    public UI_GoodItem GetGoodItem(EItemType goodType)
    {
        return goodType switch
        {
            EItemType.Gold => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Gold),
            EItemType.Dia => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_Dia),
            EItemType.ExpPoint => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_ExpPoint),
            EItemType.AbilityPoint => Get<UI_GoodItem>((int)UI_GoodItems.UI_GoodItem_AbilityPoint),
            _ => null,
        };
    }

    public void RefreshShowExp(int currentLevel, float currentExp, float maxExp)
    {
        // 예외처리: expToNextLevel이 0이 아닌 경우에만 계산
        if (maxExp > 0)
        {
            // 경험치 슬라이더와 텍스트 갱신
            GetSlider((int)Sliders.Slider_Exp).value = currentExp / maxExp;

            float expPercentage = (currentExp / maxExp) * 100;
            // 텍스트에 반영 (소수점 2자리로 표시)
            GetTMPText((int)Texts.ExpValueText).text = $"Lv.{currentLevel} ({expPercentage:F2})%";
        }
        else
        {
            // 경험치 슬라이더가 0인 경우 처리 (경험치가 없거나 초기화 상황)
            GetSlider((int)Sliders.Slider_Exp).value = 0;
            GetTMPText((int)Texts.ExpValueText).text = $"Lv.{currentLevel} (0%)";
        }
    }

    #endregion

    #region Tab

    public void ShowTab(PlayTab tab)
    {
        // 탭 변경 없이 현재 탭을 다시 선택한 경우
        if (_tab == tab)
        {
            if (_tab == PlayTab.None)
                return;
            ToggleTab(tab, false); // 현재 탭을 비활성화
            _tab = PlayTab.None;
        }
        else
        {
            if (_tab != PlayTab.None)
            {
                ToggleTab(_tab, false); // 이전 탭 비활성화
            }

            _tab = tab;
            ToggleTab(_tab, true); // 새 탭 활성화
        }
    }

    public void ToggleTab(PlayTab tab, bool isActive)
    {
        if (isActive)
        {
            switch (tab)
            {
                case PlayTab.Character:
                    Managers.UI.ShowPopupUI<UI_CharacterPopup>().RefreshUI();
                    break;
                case PlayTab.Equipment:
                    Managers.UI.ShowPopupUI<UI_EquipmentPopup>().RefreshUI(true);
                    break;
                case PlayTab.Skill:
                    Managers.UI.ShowPopupUI<UI_SkillPopup>().RefreshUI();
                    break;
                case PlayTab.Dungeon:
                    Managers.UI.ShowPopupUI<UI_DungeonPopup>().RefreshUI();
                    break;
                case PlayTab.Draw:
                    Managers.UI.ShowPopupUI<UI_DrawPopup>().RefreshUI();
                    break;
                case PlayTab.Shop:
                    Managers.UI.ShowPopupUI<UI_ShopPopup>();
                    break;
                case PlayTab.Rank:
                    var popupUI = Managers.UI.ShowPopupUI<UI_RankingPopup>();
                    Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
                    popupUI.RefreshUI();
                    break;

            }
            ShowPopupActiveGameUI(false);
        }
        else
        {
            Managers.UI.ClosePopupUI();
            ShowPopupActiveGameUI(true);

            switch (tab)
            {
                case PlayTab.Character:
                    GetGoodItem(EItemType.ExpPoint).gameObject.SetActive(false);
                    GetGoodItem(EItemType.AbilityPoint).gameObject.SetActive(false);
                    break;
                case PlayTab.Equipment:
                    break;
                case PlayTab.Skill:
                    break;
                case PlayTab.Dungeon:
                    break;
                case PlayTab.Draw:
                    break;
                case PlayTab.Shop:
                    break;
            }
        }
    }



    #endregion

    #region Skill Slot
    public void UpdatedSkillSlotUI()
    {
        foreach (var slot in Managers.Backend.GameData.SkillInventory.SkillSlotList)
        {
            _equipSkillSlotList[slot.Index].RefreshUI();
        }
    }

    public void ActiveAutoSkill()
    {
        Managers.Backend.GameData.SkillInventory.ActiveAutoSkill();
        UpdateAutoSkillUI(Managers.Backend.GameData.SkillInventory.IsAutoSkill);
    }

    private void UpdateAutoSkillUI(bool isAutoSkill)
    {
        if (isAutoSkill)
        {
            Debug.Log("Auto Skill 기능이 활성화되었습니다.");
            GetTMPText((int)Texts.Text_AutoSkill).text = "AUTO\nON";
            CheckUseSkillSlot(-1);
            Managers.Event.AddEvent(EEventType.CompleteSkillCool, new Action<int>(CheckUseSkillSlot));
        }
        else
        {
            Debug.Log("Auto Skill 기능이 비활성화되었습니다.");
            GetTMPText((int)Texts.Text_AutoSkill).text = "AUTO\nOFF";
            Managers.Event.RemoveEvent(EEventType.CompleteSkillCool, new Action<int>(CheckUseSkillSlot));
        }
    }

    #region AdBuff

    private void LoadExistingBuffs()
    {
        foreach (EAdBuffType buffType in Enum.GetValues(typeof(EAdBuffType)))
        {
            int remainingTime = Managers.Buff.GetRemainingTime(buffType);
            if (remainingTime > 0)
            {
                UpdateAdBuffItem(buffType, remainingTime);
            }
        }
    }

    public void UpdateAdBuffItem(EAdBuffType buffType, int durationMinutes)
    {
        foreach (var buffUI in _displayAdBuffList)
        {
            if (!buffUI.gameObject.activeSelf)
            {
                buffUI.gameObject.SetActive(true);
                buffUI.SetInfo(buffType, durationMinutes);
                break;
            }
        }
    }

    private void UpdateBuffUI(EAdBuffType buffType)
    {
        foreach (var buffUI in _displayAdBuffList)
        {
            if (buffUI.BuffType == buffType)
            {
                buffUI.UpdateRemainingTimeText();
                break;
            }
        }
    }

    private void RemoveBuffUI(EAdBuffType buffType)
    {
        foreach (var buffUI in _displayAdBuffList)
        {
            if (buffUI.gameObject.activeSelf && buffUI.BuffType == buffType)
            {
                buffUI.gameObject.SetActive(false);
                break;
            }
        }
    }

    #endregion

    public void CheckUseSkillSlot(int slotIndex = -1)
    {

        // slotIndex가 -1인 경우 모든 슬롯을 검사하여 사용할 수 있는 스킬이 있는지 확인
        if (slotIndex == -1)
        {
            foreach (var slot in _equipSkillSlotList)
            {
                if (slot.IsSkillReady())
                    slot.OnUseSkill();
            }
            return;
        } // 특정 슬롯의 스킬만 검사하여 사용할 수 있는지 확인
        else if (slotIndex >= 0 && slotIndex < _equipSkillSlotList.Count)
        {
            if (_equipSkillSlotList[slotIndex].IsSkillReady())
                _equipSkillSlotList[slotIndex].OnUseSkill();
        }
    }
    #endregion



    public void ShowPopupActiveGameUI(bool active)
    {
        if(!active)
        GetObject((int)GameObjects.MenuGroup).SetActive(false);
        
        GetObject((int)GameObjects.TopStage).SetActive(active);
        Get<CanvasGroup>((int)CanvasGroups.SkillSlotGroup).alpha = active ? 1 : 0;
        Get<CanvasGroup>((int)CanvasGroups.SkillSlotGroup).blocksRaycasts = active;
        Get<CanvasGroup>((int)CanvasGroups.BuffGroup).alpha = active ? 1 : 0;
        Get<CanvasGroup>((int)CanvasGroups.BuffGroup).blocksRaycasts = active;

    }
}
