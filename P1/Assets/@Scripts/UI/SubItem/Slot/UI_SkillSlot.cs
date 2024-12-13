using BackendData.GameData;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_SkillSlot : UI_Base
{
    public Button _button { get; set; }
    private UI_CompanionItem _companionItem;
    public Image _lockImage;

    private int _index; // SkillSlot의 Index만 유지
    private SkillSlot _skillSlot;

    public enum Images
    {
        Image_Lock
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));

        _button = GetComponent<Button>();
        _lockImage = GetImage((int)Images.Image_Lock);
        _companionItem = GetComponentInChildren<UI_CompanionItem>();
        _companionItem.gameObject.SetActive(false);
        _button.onClick.AddListener(OnClickButton);
        return true;
    }

    public void SetInfo(int index)
    {
        _index = index;
        _skillSlot = Managers.Backend.GameData.SkillInventory.SkillSlotList[_index];

        if (Init() == false)
        {
            RefreshUI();
        }
    }

    private void OnClickButton()
    {
        if(_skillSlot.SlotType == ESkillSlotType.Lock)
        {
            ShowAlertUI(Managers.Backend.GameData.SkillInventory.GetSkillUnlockStageMessage(_index));
            return; 
        }
    }

    public void RefreshUI()
    {
        switch (_skillSlot.SlotType)
        {
            // 잠금 상태 -> 좌물쇠 이미지 활성화, 누르면 (해제 조건 팝업), _companionItem 비활성화 
            case ESkillSlotType.Lock:
                _lockImage.gameObject.SetActive(true);
                _companionItem.gameObject.SetActive(false);
                break;
            // 비어있는 상태 -> 좌물쇠 이미지 비활성화, _companionItem 비활성화 
            case ESkillSlotType.None:
                _lockImage.gameObject.SetActive(false);
                _companionItem.gameObject.SetActive(false);
                break;
            // 장착중인 상태 -> 좌물쇠 이미지 비활성화 _companionItem 활성화 정보 입력 
            case ESkillSlotType.Equipped:
                _lockImage.gameObject.SetActive(false);
                _companionItem.gameObject.SetActive(true);
                break;
        }
        if (_skillSlot.SkillInfoData == null)
            return;

        _companionItem.DisplayItem(_skillSlot.SkillInfoData, EItemDisplayType.SlotItem);
    }

    public void EnableButton(bool enable) 
    {
        _companionItem.EnableButton(enable);
    }
}
