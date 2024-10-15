using UnityEngine;
using UnityEngine.UI;
using BackendData.GameData;
using DG.Tweening;
using static Define;
using System.Collections;
using System;

public class UI_EquipSkillSlot : UI_Base
{
    public enum DelayType
    {
        CoolTime,
        DurationTime
    }

    public enum Images
    {
        Image_Lock,
        Image_Icon,
        Image_CoolTime,
        Image_Duration
    }

    private Button _button;
    private Image _lockImage;
    private Image _iconImage;
    private Image _coolTimeImage;
    private Image _durationImage;

    private int _index; // SkillSlot의 Index만 유지
    private SkillSlot _skillSlot;
    private Coroutine _coolTimeCo;

    private DelayType _delayType = DelayType.CoolTime; 

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        _button = GetComponent<Button>();
        _lockImage = GetImage((int)Images.Image_Lock);
        _iconImage = GetImage((int)Images.Image_Icon);
        _coolTimeImage = GetImage((int)Images.Image_CoolTime);
        _durationImage = GetImage((int)Images.Image_Duration);

        _button.onClick.AddListener(OnUseSkill);
        _iconImage.gameObject.SetActive(false);
        _lockImage.gameObject.SetActive(false);
        _coolTimeImage.gameObject.SetActive(false);
        _durationImage.gameObject.SetActive(false);
        return true;
    }

    public void SetInfo(int index)
    {
        _index = index;
        _skillSlot = Managers.Backend.GameData.SkillInventory.SkillSlotList[_index];

        if(Init() == false)
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
                return;
            case ESkillSlotType.None:
                StopCoolTime(_index);
                _iconImage.gameObject.SetActive(false);
                _lockImage.gameObject.SetActive(false);
                return;
            case ESkillSlotType.Equipped:
                _iconImage.gameObject.SetActive(true);
                _lockImage.gameObject.SetActive(false);
                break;
        }

        if (_skillSlot.SkillInfoData == null)
            return;
            
        _iconImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/{_skillSlot.SkillInfoData.SpriteKey}");
        StopCoolTime(_index);
        _coolTimeCo = StartCoroutine(CoolTimeCo(_skillSlot.SkillInfoData.Data.CoolTime, DelayType.CoolTime));    
    }
    
    public void OnUseSkill()
    {
        if(IsSkillReady())
        {
            Managers.Object.Hero.Skills.UseSkill(_index);
            _coolTimeCo = StartCoroutine(CoolTimeCo(Managers.Data.EffectChart[_skillSlot.SkillInfoData.Data.EffectId].Duration, DelayType.DurationTime));
            Debug.LogWarning($"{_index + 1}번째 슬롯의 <color=#FF0000>{_skillSlot.SkillInfoData.Name}</color> 스킬이 실행됩니다.");
        }
    }

    private IEnumerator CoolTimeCo(float coolTime, DelayType delayType)
    {
        Image delayImage = delayType == DelayType.CoolTime ? _coolTimeImage : _durationImage;

        _iconImage.color = Util.HexToColor("919191");
        delayImage.gameObject.SetActive(true);
        delayImage.fillAmount = 1;

        float elapsedTime = 0f;

        while (elapsedTime < coolTime)
        {
            elapsedTime += Time.deltaTime;
            delayImage.fillAmount = Mathf.Clamp01(1f - (elapsedTime / coolTime));
            yield return null;
        }

        _coolTimeCo = null;
        _iconImage.DOColor(Color.white, 0.3f);
        delayImage.fillAmount = 0f;
        delayImage.gameObject.SetActive(false);

        switch(delayType)
        {
            // 쿨타임이 지나면 스킬 실행 가능한 상태 
            case DelayType.CoolTime:
            Managers.Event.TriggerEvent(EEventType.CompleteSkillCool, _index);
            break;

            // 스킬 실행 시간이 지나면 쿨타임으로 바꿔줘야함 
            case DelayType.DurationTime:
            _coolTimeCo = StartCoroutine(CoolTimeCo(_skillSlot.SkillInfoData.Data.CoolTime, DelayType.CoolTime));  
            break;
        }
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
        _durationImage.fillAmount = 0;
        _durationImage.gameObject.SetActive(false);
    }

    public bool IsSkillReady()
    {
        // 스킬이 사용 가능한 상태인지 확인하는 메서드 추가
        return _skillSlot != null && _skillSlot.SkillInfoData != null && _coolTimeCo == null && _skillSlot.SlotType == ESkillSlotType.Equipped;
    }


}
