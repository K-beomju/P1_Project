using BackendData.GameData;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_PetPopup : UI_Popup
{
    public enum UI_PetItems
    {
        UI_PetItem_Silver,
        UI_PetItem_Blue,
        UI_PetItem_Emerald,
        UI_PetItem_Scale,
        UI_PetItem_Wood,
        UI_PetItem_Gold,
        UI_PetItem_Flame,
        UI_PetItem_Book,
        UI_PetItem_Rune
    }

    public enum Images
    {
        Image_PetIcon
    }

    public enum Texts
    {
        Text_PetName,
        Text_PetLevel,
        Text_PetDesc,
        Text_Amount,
        Text_EquippedAtk,
        Text_EquippedHp,
        Text_OwnedAtk
    }

    public enum Buttons
    {
        Btn_Equip,
        Btn_UnEquip,
        Btn_Enhance,
        Btn_Make,
    }

    public enum Sliders
    {
        Slider_Amount
    }

    public enum GameObjects
    {
        BtnGroup_Owned,
        BtnGroup_Unowned
    }

    private PetData _petData;
    private PetInfoData _petInfoData;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_PetItem>(typeof(UI_PetItems));
        BindImages(typeof(Images));
        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindSliders(typeof(Sliders));
        BindObjects(typeof(GameObjects));

        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Silver).SetInfo(EPetType.Silver);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Blue).SetInfo(EPetType.Blue);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Emerald).SetInfo(EPetType.Emerald);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Scale).SetInfo(EPetType.Scale);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Wood).SetInfo(EPetType.Wood);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Gold).SetInfo(EPetType.Gold);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Flame).SetInfo(EPetType.Flame);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Book).SetInfo(EPetType.Book);
        Get<UI_PetItem>((int)UI_PetItems.UI_PetItem_Rune).SetInfo(EPetType.Rune);

        GetButton((int)Buttons.Btn_Equip).onClick.AddListener(OnClickEquipButton);
        GetButton((int)Buttons.Btn_UnEquip).onClick.AddListener(OnClickUnEquipButton);
        GetButton((int)Buttons.Btn_Enhance).onClick.AddListener(OnClickEnhanceButton);
        GetButton((int)Buttons.Btn_Make).onClick.AddListener(OnClickMakeButton);

        _petData = Managers.Data.PetChart[EPetType.Silver];
        return true;
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.PetItemClick, new Action<PetData>(ShowPetDetailUI));
        Managers.Event.AddEvent(EEventType.PetItemUpdated, new Action(RefreshUI));

    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.PetItemClick, new Action<PetData>(ShowPetDetailUI));
        Managers.Event.RemoveEvent(EEventType.PetItemUpdated, new Action(RefreshUI));
    }

    private void OnClickMakeButton()
    {
        if (_petData == null)
        {
            return;
        }

        Managers.Backend.GameData.PetInventory.MakePet(_petData.PetType);
        Managers.Backend.GameData.PetInventory.AddPetCraft(_petData.PetType, -_petData.MaxCount);
        RefreshUI();
    }

    private void OnClickEnhanceButton()
    {
        if (_petData == null)
        {
            return;
        }

        int maxCount = _petInfoData.Level * _petData.MaxCount;
        Managers.Backend.GameData.PetInventory.PetLevelUp(_petData.PetType, maxCount);
        RefreshUI();
    }

    private void OnClickUnEquipButton()
    {
        if (_petData == null)
        {
            return;
        }

        Managers.Backend.GameData.PetInventory.UnEquipPet(_petData.PetType);
        RefreshUI();
    }

    private void OnClickEquipButton()
    {
        if (_petData == null)
        {
            return;
        }

        Managers.Backend.GameData.PetInventory.EquipPet(_petData.PetType);
        Managers.Object.SpawnGameObject(Vector2.zero, "Object/Pet/Handrick");
        RefreshUI();
    }

    public void RefreshUI()
    {
        // 6개의 슬롯을 초기화
        for (int i = 0; i < Enum.GetValues(typeof(UI_PetItems)).Length; i++)
        {
            Get<UI_PetItem>(i).RefreshUI();
        }


        // 상세 UI 갱신 추가
        if (_petData != null)
        {
            // 현재 열린 펫이 업데이트된 펫과 동일하면 다시 표시
            if (Managers.Backend.GameData.PetInventory.PetInventoryDic.ContainsKey(_petData.PetType.ToString()))
            {
                ShowPetDetailUI(_petData);
            }
        }
    }

    private void ShowPetDetailUI(PetData petData)
    {
        _petData = petData;
        _petInfoData = Managers.Backend.GameData.PetInventory.PetInventoryDic[petData.PetType.ToString()];
        if (_petInfoData == null)
        {
            Debug.LogWarning("PetInfoData NULL");
            return;
        }

        int currentCount = _petInfoData.Count;
        int maxCount = _petInfoData.Level * petData.MaxCount;

        // 정보, 갯수 표시 
        GetImage((int)Images.Image_PetIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{Managers.Data.PetChart[petData.PetType].PetSpriteKey}");
        GetTMPText((int)Texts.Text_PetLevel).text = $"Lv. {_petInfoData.Level}";
        GetTMPText((int)Texts.Text_PetName).text = petData.PetName;
        GetTMPText((int)Texts.Text_Amount).text = $"{currentCount} / {maxCount}";
        GetTMPText((int)Texts.Text_PetDesc).text = petData.PetDesc;
        GetSlider((int)Sliders.Slider_Amount).maxValue = maxCount;
        GetSlider((int)Sliders.Slider_Amount).value = currentCount;

        // 능력치 표시
        float increaseAtkRate = 1 + (petData.EquippedAtkIncreaseRate / 100f) * _petInfoData.Level;
        float increaseHpRate = 1 + (petData.EquippedHpIncreaseRate / 100f) * _petInfoData.Level;
        float totalEqAtk = petData.EquippedAtkValue * increaseAtkRate;
        float totalEqHp = petData.EquippedHpValue * increaseHpRate;
        float totalOwnedAtk = petData.OwnedAtkPercent + (_petInfoData.Level * petData.OwnedAtkIncreasePercent);

        GetTMPText((int)Texts.Text_EquippedAtk).text = $"{Util.ConvertToTotalCurrency((long)totalEqAtk)}";
        GetTMPText((int)Texts.Text_EquippedHp).text = $"{Util.ConvertToTotalCurrency((long)totalEqHp)}";
        GetTMPText((int)Texts.Text_OwnedAtk).text = $"공격력 +{Util.ConvertToTotalCurrency((long)totalOwnedAtk)}%";

        // 펫 상태에 따른 Active
        GetObject((int)GameObjects.BtnGroup_Owned).SetActive(_petInfoData.OwningState == EOwningState.Owned);
        GetObject((int)GameObjects.BtnGroup_Unowned).SetActive(_petInfoData.OwningState == EOwningState.Unowned);
        GetButton((int)Buttons.Btn_Make).interactable = IsMakePet(petData, _petInfoData);
        GetButton((int)Buttons.Btn_Enhance).interactable = IsMakePet(petData, _petInfoData);
        GetButton((int)Buttons.Btn_Equip).gameObject.SetActive(!Managers.Backend.GameData.PetInventory.IsEquipPet(petData.PetType));
        GetButton((int)Buttons.Btn_UnEquip).gameObject.SetActive(Managers.Backend.GameData.PetInventory.IsEquipPet(petData.PetType));
    }

    private bool IsMakePet(PetData petData, PetInfoData petInfoData)
    {
        if (petInfoData.Count >= Managers.Data.PetChart[petData.PetType].MaxCount * petInfoData.Level)
        {
            return true;
        }

        return false;
    }
}
