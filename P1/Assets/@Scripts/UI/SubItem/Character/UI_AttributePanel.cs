using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_AttributePanel : UI_Base
{
    public enum Images
    {
        Image_SelectAbIcon
    }

    public enum Texts
    {
        Text_AttributeName,
        Text_AttributeLevel,
        Text_AttributeValue,
        Text_Amount
    }

    private Data.HeroAttributeInfoData selectAttributeInfo;
    public Data.HeroAttributeInfoData SelectAttributeInfo
    {
        get { return selectAttributeInfo; }
        set
        {
            if (selectAttributeInfo != value)
            {
                selectAttributeInfo = value;
            }
        }
    }

    public enum UI_AttributeGrowthSlots
    {
        UI_AttributeGrowthInvenSlot_Atk,
        UI_AttributeGrowthInvenSlot_MaxHp,
        UI_AttributeGrowthInvenSlot_CriRate,
        UI_AttributeGrowthInvenSlot_CriDmg,
        UI_AttributeGrowthInvenSlot_SkillTime,
        UI_AttributeGrowthInvenSlot_SkillDmg
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_AttributeGrowhInvenSlot>(typeof(UI_AttributeGrowthSlots));
        BindTMPTexts(typeof(Texts));
        BindImages(typeof(Images));

        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_Atk).SetInfo(EHeroAttrType.Growth_Atk);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_MaxHp).SetInfo(EHeroAttrType.Growth_MaxHp);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriRate).SetInfo(EHeroAttrType.Growth_CriRate);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_CriDmg).SetInfo(EHeroAttrType.Growth_CriDmg);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillTime).SetInfo(EHeroAttrType.Growth_SkillTime);
        Get<UI_AttributeGrowhInvenSlot>((int)UI_AttributeGrowthSlots.UI_AttributeGrowthInvenSlot_SkillDmg).SetInfo(EHeroAttrType.Growth_SkillDmg);

        return true;
    }

    void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.AttributeItemClick, new Action<Data.HeroAttributeInfoData>(ShowAttributeDetailUI));
    }

    void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.AttributeItemClick, new Action<Data.HeroAttributeInfoData>(ShowAttributeDetailUI));
    }


    public void ShowAttributeDetailUI(Data.HeroAttributeInfoData attriData)
    {
        if (SelectAttributeInfo == attriData)
            return;

        SelectAttributeInfo = attriData;

        if (!Managers.Backend.GameData.UserData.UpgradeAttrDic.TryGetValue(attriData.HeroAttrType.ToString(), out int level))
        {
            Debug.LogWarning($"UpdateSlotInfoUI도중 {level}이 없습니다");
            return;
        }

        GetTMPText((int)Texts.Text_AttributeLevel).text = $"Lv {level}";
        GetTMPText((int)Texts.Text_AttributeName).text = SelectAttributeInfo.Name;
        GetImage((int)Images.Image_SelectAbIcon).sprite = Managers.Resource.Load<Sprite>($"Sprites/{SelectAttributeInfo.SpriteKey}");
    }
}
