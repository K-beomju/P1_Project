using UnityEngine;
using UnityEngine.UI;
using BackendData.GameData;
using static Define;

public class UI_EquipSkillSlot : UI_Base
{
    public enum Images
    {
        Image_Lock,
        Image_Icon,
        Image_CoolTime
    }

    private int _index; // SkillSlot의 Index만 유지
    private Image _lockImage;
    private Image _iconImage;
    private Image _coolTimeImage;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));

        _lockImage = GetImage((int)Images.Image_Lock);
        _iconImage = GetImage((int)Images.Image_Icon);
        _coolTimeImage = GetImage((int)Images.Image_CoolTime);

        _iconImage.gameObject.SetActive(false);
        _lockImage.gameObject.SetActive(false);
        _coolTimeImage.gameObject.SetActive(false);
        return true;
    }

    public void SetInfo(int index)
    {
        _index = index;

        if(Init() == false)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        SkillSlot skillSlot = Managers.Backend.GameData.SkillInventory.SkillSlotList[_index];

        switch (skillSlot.SlotType)
        {
            case ESkillSlotType.Lock:
                _iconImage.gameObject.SetActive(false);
                _lockImage.gameObject.SetActive(true);
                break;
            case ESkillSlotType.None:
                _iconImage.gameObject.SetActive(false);
                _lockImage.gameObject.SetActive(false);
                break;
            case ESkillSlotType.Equipped:
                _iconImage.gameObject.SetActive(true);
                _lockImage.gameObject.SetActive(false);
                break;
        }
        if (skillSlot.SkillInfoData == null)
            return;
        
        _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{skillSlot.SkillInfoData.SpriteKey}");

    }


}
