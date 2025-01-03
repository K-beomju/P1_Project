using BackEnd;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum Buttons
    {
        PetButton,
        CharacterButton,
        EquipmentButton,
        SkillButton,
        DungeonButton,
        DrawButton,
        Btn_AutoSkill,
        Btn_Ranking,
        Btn_AdBuff,
        Btn_Post,
        Btn_Setting,
        Btn_Menu,
        Btn_Quest,
        Btn_Stage
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
        RankUpDescText,
        Text_NickName,
        Text_MyRank
    }

    enum Images
    {
        Image_RankUpIcon,
        Image_MyRankIcon,
        Image_AdBuff,

        // PopupButton
        Image_PetLock,
        Image_CharacterLock,
        Image_EquipmentLock,
        Image_SkillLock,
        Image_DungeonLock,
        Image_DrawLock,
    }

    enum GameObjects
    {
        RemainMonster,
        RankUpStage,
        MenuGroup,
        Quest_NotifiBadge,
        HeroInfo
    }

    enum CanvasGroups
    {
        SkillSlotGroup,
        BuffGroup,
        TopStageGroup
    }

    public enum PlayTab
    {
        None = -1,
        Character,
        Equipment,
        Skill,
        Dungeon,
        Draw,
        Pet,
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

    private Coroutine[] _autoSkillCheckCoroutines = new Coroutine[6];

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
        GetButton((int)Buttons.PetButton).onClick.AddListener(() => ShowTab(PlayTab.Pet));
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
        //         GetButton((int)Buttons.Btn_Logout).onClick.AddListener(() =>
        //         {
        //             SendQueue.Enqueue(Backend.BMember.Logout, callback =>
        //             {
        //                 Debug.Log($"Backend.BMember.Logout : {callback}");
        // #if UNITY_ANDROID
        //                 TheBackend.ToolKit.GoogleLogin.Android.GoogleSignOut(true, GoogleSignOutCallback);
        // #endif

        //                 if (callback.IsSuccess())
        //                 {
        //                     Debug.Log("로그아웃 성공");
        //                     Managers.Scene.LoadScene(EScene.TitleScene);
        //                 }
        //             });
        //         });

        GetButton((int)Buttons.Btn_Quest).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_QuestPopup>();
            Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
            popupUI.RefreshUI();
        });

        GetButton((int)Buttons.Btn_Stage).onClick.AddListener(() =>
        {
            var popupUI = Managers.UI.ShowPopupUI<UI_StagePopup>();
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

        Managers.Sound.Play(ESound.Bgm, "Sounds/GameBGM", 0.3f);

        return true;
    }

    private void OnDestroy()
    {
        Managers.Buff.OnBuffTimeUpdated -= UpdateBuffUI;
        Managers.Buff.OnBuffExpired -= RemoveBuffUI;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.QuestCheckNotification, new Action(() =>
        {
            if (Managers.Backend.GameData.QuestData.IsReadyToClaimQuest())
                GetObject((int)GameObjects.Quest_NotifiBadge).SetActive(true);
            else
                GetObject((int)GameObjects.Quest_NotifiBadge).SetActive(false);
        }));
        Managers.Event.AddEvent(EEventType.MyRankingUpdated, new Action(UpdateMyRanking));

        var missionData = Managers.Backend.GameData.MissionData.GetCurrentMission();
        if (missionData != null)
            Managers.Event.AddEvent(EEventType.MissionCompleted, new Action(CheckLockPopupMission));
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.QuestCheckNotification, new Action(() =>
        {
            if (Managers.Backend.GameData.QuestData.IsReadyToClaimQuest())
                GetObject((int)GameObjects.Quest_NotifiBadge).SetActive(true);
            else
                GetObject((int)GameObjects.Quest_NotifiBadge).SetActive(false);
        }));
        Managers.Event.RemoveEvent(EEventType.MyRankingUpdated, new Action(UpdateMyRanking));

        var missionData = Managers.Backend.GameData.MissionData.GetCurrentMission();
        if (missionData != null)
            Managers.Event.RemoveEvent(EEventType.MissionCompleted, new Action(CheckLockPopupMission));
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

        // 광고형 버프 시간 체크 
        LoadExistingBuffs();
        UpdateMyNickName();
        UpdateMyRank();

        GetSlider((int)Sliders.Slider_Exp).value = 0;
        GetSlider((int)Sliders.Slider_BossTimer).value = 0;
        GetSlider((int)Sliders.Slider_StageInfo).value = 0;


        GetTMPText((int)Texts.ExpValueText).text = string.Empty;
        GetTMPText((int)Texts.RemainMonsterValueText).text = string.Empty;

        GetGoodItem(EItemType.ExpPoint).gameObject.SetActive(false);
        GetGoodItem(EItemType.AbilityPoint).gameObject.SetActive(false);
        GetObject((int)GameObjects.RankUpStage).SetActive(false);
        GetObject((int)GameObjects.MenuGroup).SetActive(false);
        GetObject((int)GameObjects.Quest_NotifiBadge).SetActive(false);

        UpdateAutoSkillUI(Managers.Backend.GameData.SkillInventory.IsAutoSkill);
        CheckLockPopupMission();

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
        SetActive(sceneState == EGameSceneState.Play || sceneState == EGameSceneState.Boss
        || sceneState == EGameSceneState.RankUp || sceneState == EGameSceneState.Stay);
        GetObject((int)GameObjects.RankUpStage).SetActive(sceneState == EGameSceneState.RankUp);

        switch (sceneState)
        {
            case EGameSceneState.Play:
            case EGameSceneState.Stay:
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
                case PlayTab.Pet:
                    Managers.UI.ShowPopupUI<UI_PetPopup>().RefreshUI();
                    break;
                case PlayTab.Rank:
                    var popupUI = Managers.UI.ShowPopupUI<UI_RankingPopup>();
                    Managers.UI.SetCanvas(popupUI.gameObject, false, SortingLayers.UI_SCENE + 1);
                    popupUI.RefreshUI();
                    break;

            }
            ShowPopupActiveGameUI(false);
            Managers.Sound.Play(ESound.Effect, "Sounds/OnPopup", 0.5f);

        }
        else
        {
            Managers.UI.ClosePopupUI();
            ShowPopupActiveGameUI(true);
            Managers.Sound.Play(ESound.Effect, "Sounds/OffPopup", 0.5f);

            switch (tab)
            {
                case PlayTab.Character:
                    GetGoodItem(EItemType.Gold).gameObject.SetActive(true);
                    GetGoodItem(EItemType.Dia).gameObject.SetActive(true);
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
                case PlayTab.Pet:
                    break;
            }
        }
    }



    #endregion

    #region Skill Slot

    // CompleteSkillCool 이벤트 핸들러
    private void OnCompleteSkillCool(int slotIndex)
    {
        if (_autoSkillCheckCoroutines[slotIndex] == null)
        {
            _autoSkillCheckCoroutines[slotIndex] = StartCoroutine(AutoSkillCheckRoutine(slotIndex));
        }
    }

    // 슬롯별 타겟 상태를 검사하고 스킬 실행
    private IEnumerator AutoSkillCheckRoutine(int slotIndex)
    {
        var slot = _equipSkillSlotList[slotIndex];
        if (slot == null) yield break;

        while (!Managers.Object.Hero.Target.IsValid())
        {
            // 타겟이 유효할 때까지 0.2초 대기
            yield return new WaitForSeconds(0.2f);
        }

        // 타겟이 유효해지면 스킬 실행
        if (slot.IsSkillReady())
        {
            slot.OnUseSkill();
        }

        _autoSkillCheckCoroutines[slotIndex] = null;
    }

    // 모든 슬롯의 자동 스킬 활성화
    public void EnableAutoSkill()
    {
        for (int i = 0; i < _equipSkillSlotList.Count; i++)
        {
            if (_autoSkillCheckCoroutines[i] == null)
            {
                _autoSkillCheckCoroutines[i] = StartCoroutine(AutoSkillCheckRoutine(i));
            }
        }
    }

    // 모든 슬롯의 자동 스킬 비활성화
    public void DisableAutoSkill()
    {
        for (int i = 0; i < _autoSkillCheckCoroutines.Length; i++)
        {
            if (_autoSkillCheckCoroutines[i] != null)
            {
                StopCoroutine(_autoSkillCheckCoroutines[i]);
                _autoSkillCheckCoroutines[i] = null;
            }
        }
    }

    // UI Update
    public void UpdatedSkillSlotUI()
    {
        foreach (var slot in Managers.Backend.GameData.SkillInventory.SkillSlotList)
        {
            _equipSkillSlotList[slot.Index].RefreshUI();
        }
    }

    // Auto Skill Button Logic
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
            EnableAutoSkill();
            Managers.Event.AddEvent(EEventType.CompleteSkillCool, new Action<int>(OnCompleteSkillCool));
        }
        else
        {
            Debug.Log("Auto Skill 기능이 비활성화되었습니다.");
            GetTMPText((int)Texts.Text_AutoSkill).text = "AUTO\nOFF";
            DisableAutoSkill();
            Managers.Event.RemoveEvent(EEventType.CompleteSkillCool, new Action<int>(OnCompleteSkillCool));
        }
    }

    #endregion

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
        RotateAdBuffIcon();
    }

    public void RotateAdBuffIcon()
    {
        if (Managers.Buff.IsAnyBuffActive())
        {
            GetImage((int)Images.Image_AdBuff).transform.DORotate(
                new Vector3(0, 0, -360), // Z축을 기준으로 360도 회전
                1f,                     // 1초 동안 회전
                RotateMode.FastBeyond360 // 빠르게 연속 회전
            )
            .SetEase(Ease.InOutSine)        // 일정한 속도로 회전
            .SetLoops(-1);               // 무한 루프
        }
        else
        {
            // 애니메이션을 멈추거나 초기화
            GetImage((int)Images.Image_AdBuff).transform.DOKill(); // DOTween 애니메이션 중지
            GetImage((int)Images.Image_AdBuff).transform.rotation = Quaternion.identity; // 초기화
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
        RotateAdBuffIcon();
    }

    #endregion

    public void ShowPopupActiveGameUI(bool active)
    {
        if (!active)
        {
            // 버튼 이미지 스프라이트 변경
            Image buttonImage = GetButton((int)Buttons.Btn_Menu).GetComponent<Image>();
            string spritePath = "Sprites/Icon_Menu_Hamburger";
            buttonImage.sprite = Managers.Resource.Load<Sprite>(spritePath);

            GetObject((int)GameObjects.MenuGroup).SetActive(false);
        }

        Get<CanvasGroup>((int)CanvasGroups.TopStageGroup).alpha = active ? 1 : 0;
        Get<CanvasGroup>((int)CanvasGroups.TopStageGroup).blocksRaycasts = active;
        Get<CanvasGroup>((int)CanvasGroups.SkillSlotGroup).alpha = active ? 1 : 0;
        Get<CanvasGroup>((int)CanvasGroups.SkillSlotGroup).blocksRaycasts = active;
        Get<CanvasGroup>((int)CanvasGroups.BuffGroup).alpha = active ? 1 : 0;
        Get<CanvasGroup>((int)CanvasGroups.BuffGroup).blocksRaycasts = active;

    }


    #region HeroInfo


    public void UpdateMyNickName()
    {
        // TOP - HeroInfo UI 
        SendQueue.Enqueue(Backend.BMember.GetUserInfo, (callback) =>
        {
            if (!callback.IsSuccess())
            {
                ShowAlertUI($"닉네임이 불러오기 실패");
                return;
            }
            string nickname = callback.GetReturnValuetoJSON()["row"]["nickname"].ToString();
            GetTMPText((int)Texts.Text_NickName).text = nickname;

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                GetObject((int)GameObjects.HeroInfo).GetComponent<RectTransform>());
        });
    }

    public void UpdateMyRank()
    {
        ERankType rankType = Managers.Backend.GameData.RankUpData.GetRankType(ERankState.Current);

        if (rankType != ERankType.Unknown)
        {
            GetImage((int)Images.Image_MyRankIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/{rankType}");
        }
        else
        {
            GetImage((int)Images.Image_MyRankIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/Class/Unranked");
        }

    }

    public void UpdateMyRanking()
    {
        Managers.Backend.Rank.List[0].GetMyRank((isSuccess, myRank) =>
        {
            if (isSuccess)
            {
                if (myRank != null)
                {
                    GetTMPText((int)Texts.Text_MyRank).text = Managers.Backend.GameData.CharacterData.WorldBossCombatPower == 0 ? $"-" : $"{myRank.rank}위";
                }
                else
                {
                    Managers.Backend.SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), "myRank is null");
                }
            }
            else
            {
                Debug.LogWarning("랭킹이 정상적으로 로드되지 않았습니다.");
            }
        });
    }

    #endregion



    #region Bottom

    public void CheckLockPopupMission()
    {
        var missionData = Managers.Backend.GameData.MissionData.GetCurrentMission();
        GetButton((int)Buttons.PetButton).interactable = false;
        GetButton((int)Buttons.CharacterButton).interactable = false;
        GetButton((int)Buttons.EquipmentButton).interactable = false;
        GetButton((int)Buttons.SkillButton).interactable = false;
        GetButton((int)Buttons.DungeonButton).interactable = false;
        GetButton((int)Buttons.DrawButton).interactable = false;

        // 다 깬거로 간주함. 
        if (missionData == null)
        {
            UnlockUI(Buttons.PetButton, Images.Image_PetLock);
            UnlockUI(Buttons.CharacterButton, Images.Image_CharacterLock);
            UnlockUI(Buttons.EquipmentButton, Images.Image_EquipmentLock);
            UnlockUI(Buttons.SkillButton, Images.Image_SkillLock);
            UnlockUI(Buttons.DungeonButton, Images.Image_DungeonLock);
            UnlockUI(Buttons.DrawButton, Images.Image_DrawLock);
            Managers.Event.RemoveEvent(EEventType.MissionCompleted, new Action(CheckLockPopupMission));
            return;
        }

        int missionID = missionData.Id;

        // ID별 잠금 해제 조건
        if (missionID >= 2)
        {
            UnlockUI(Buttons.CharacterButton, Images.Image_CharacterLock);
            // Vector2 pointerPosition = Util.GetCanvasPosition(GetButton((int)Buttons.CharacterButton).GetComponent<RectTransform>().position - new Vector3(-100,0,0));
            // var pointer = Managers.UI.ShowPooledUI<UI_PointerBase>();
            // pointer.SetPosition(pointerPosition);
        }
        if (missionID >= 17)
        {
            UnlockUI(Buttons.DrawButton, Images.Image_DrawLock);
        }
        if (missionID >= 18)
        {
            UnlockUI(Buttons.EquipmentButton, Images.Image_EquipmentLock);
        }
        if (missionID >= 21)
        {
            UnlockUI(Buttons.SkillButton, Images.Image_SkillLock);
        }
    }

    private void UnlockUI(Buttons button, Images lockImage)
    {
        GetButton((int)button).interactable = true;
        GetImage((int)lockImage).gameObject.SetActive(false);
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


    #region Get 

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

    public Vector2 GetPetButtonCanvasLocalPosition()
    {
        RectTransform rect = GetButton((int)Buttons.PetButton).GetComponent<RectTransform>();
        return Util.GetCanvasPosition(rect.position);
    }



    #endregion

}
