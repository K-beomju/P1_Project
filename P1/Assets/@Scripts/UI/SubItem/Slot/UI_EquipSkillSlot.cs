using UnityEngine;
using UnityEngine.UI;
using BackendData.GameData;
using DG.Tweening;
using static Define;
using System.Collections;

public class UI_EquipSkillSlot : UI_Base
{
    public enum Images
    {
        Image_Lock,
        Image_Icon,
        Image_CoolTime
    }

    private Button _button;
    private Image _lockImage;
    private Image _iconImage;
    private Image _coolTimeImage;

    private int _index; // SkillSlot의 Index만 유지
    private SkillSlot _skillSlot;
    private Coroutine _coolTimeCo;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImages(typeof(Images));
        _button = GetComponent<Button>();
        _lockImage = GetImage((int)Images.Image_Lock);
        _iconImage = GetImage((int)Images.Image_Icon);
        _coolTimeImage = GetImage((int)Images.Image_CoolTime);

        _button.onClick.AddListener(OnUseSkill);

        _iconImage.gameObject.SetActive(false);
        _lockImage.gameObject.SetActive(false);
        _coolTimeImage.gameObject.SetActive(false);
        return true;
    }

    public void SetInfo(int index)
    {
        _index = index;
        _skillSlot = Managers.Backend.GameData.SkillInventory.SkillSlotList[_index];
        _skillSlot.OnSkillUnEquipped -= StopCoolTime;
        _skillSlot.OnSkillUnEquipped += StopCoolTime;
        if (Init() == false)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        switch (_skillSlot.SlotType)
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

        if (_skillSlot.SkillInfoData == null)
            return;

        _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{_skillSlot.SkillInfoData.SpriteKey}");

        if(_skillSlot.IsCoolTimeActive)
        {
            _coolTimeCo = StartCoroutine(CoolTimeCo(_skillSlot.SkillInfoData.Data.CoolTime));
            _skillSlot.IsCoolTimeActive = false;
        }
    }

    public void OnUseSkill()
    {
        // if (_skillSlot.SkillInfoData == null || _skillSlot.SlotType == ESkillSlotType.None || _skillSlot.SlotType == ESkillSlotType.Lock)
        // {
        //     Debug.LogWarning($"현재 클릭한 {_index + 1}번째 슬롯이 비어있거나 잠겨있거나 데이터가 없습니다");
        //     return;
        // }

        // if (_coolTimeCo != null)
        // {
        //     Debug.LogWarning("스킬 쿨타임 중입니다.");
        //     return;
        // }

        if(IsSkillReady())
        {
            _coolTimeCo = StartCoroutine(CoolTimeCo(_skillSlot.SkillInfoData.Data.CoolTime));
            Debug.LogWarning($"{_index + 1}번째 슬롯의 <color=#FF0000>{_skillSlot.SkillInfoData.Name}</color> 스킬이 실행됩니다.");
        }
    }

    private IEnumerator CoolTimeCo(float coolTime)
    {
        _iconImage.color = Util.HexToColor("919191");
        _coolTimeImage.gameObject.SetActive(true);
        _coolTimeImage.fillAmount = 1;

        float elapsedTime = 0f;

        while (elapsedTime < coolTime)
        {
            elapsedTime += Time.deltaTime;
            _coolTimeImage.fillAmount = Mathf.Clamp01(1f - (elapsedTime / coolTime));
            yield return null;
        }

        _coolTimeCo = null;
        _iconImage.DOColor(Color.white, 0.3f);
        _coolTimeImage.fillAmount = 0f;
        _coolTimeImage.gameObject.SetActive(false);
        Managers.Event.TriggerEvent(EEventType.CompleteSkillCool, _index);
    }

    private void StopCoolTime(int slotIndex = -1)
    {
        // 전달받은 슬롯 인덱스가 현재 슬롯 인덱스와 일치하지 않으면 리턴 (옵션)
        if (slotIndex != -1 && slotIndex != _index)
            return;

        if(_coolTimeCo != null)
        {
            StopCoroutine(_coolTimeCo);
            _coolTimeCo = null;
        }
        
        _iconImage.color = Color.white;
        _coolTimeImage.fillAmount = 0;
        _coolTimeImage.gameObject.SetActive(false);
    }

    public bool IsSkillReady()
    {
        // 스킬이 사용 가능한 상태인지 확인하는 메서드 추가
        return _skillSlot != null && _skillSlot.SkillInfoData != null && _coolTimeCo == null && _skillSlot.SlotType == ESkillSlotType.Equipped;
    }


}