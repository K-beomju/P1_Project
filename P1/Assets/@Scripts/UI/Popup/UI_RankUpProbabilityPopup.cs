using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Define;

public class UI_RankUpProbabilityPopup : UI_Popup
{
    public enum Buttons
    {
        Btn_Exit,
        Btn_Detail
    }

    public enum GameObjects
    {
        BG,
        Detail,
        Summary
    }

    public enum Texts
    {
        Text_AllStatProbability_Left,
        Text_AllStatProbability_Right,
        Text_RareProbability,
        Text_AllStatProbability
    }

    private bool showDetail;
    private bool initiailized = false;

    protected override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));
        BindTMPTexts(typeof(Texts));

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(ClosePopupUI);
        GetObject((int)GameObjects.BG).gameObject.BindEvent(ClosePopupUI);
        GetObject((int)GameObjects.Detail).gameObject.SetActive(false);

        GetButton((int)Buttons.Btn_Detail).onClick.AddListener(() =>
        {
            showDetail = !showDetail;
            GetObject((int)GameObjects.Detail).gameObject.SetActive(showDetail);
            GetObject((int)GameObjects.Summary).gameObject.SetActive(!showDetail);
        });
        return true;
    }

    public void RefreshUI()
    {
        if (initiailized)
        {
            return;
        }
        
        initiailized = true;
        // Surmmary
        EHeroRankUpStatType[] validStatTypes = Enum.GetValues(typeof(EHeroRankUpStatType))
            .Cast<EHeroRankUpStatType>()
            .Where(statType => statType != EHeroRankUpStatType.None)
            .ToArray();

        string descString = string.Empty;
        foreach (EHeroRankUpStatType statType in validStatTypes)
        {
            string statName = Managers.Data.DrawRankUpChart[statType].Name;
            string valueDesc =
                $"{Managers.Data.DrawRankUpChart[statType].NormalValueList.First()}~ {Managers.Data.DrawRankUpChart[statType].MythicalValueList.Last()}";

            descString += $"{statName}(%): {valueDesc}\n";
        }

        GetTMPText((int)Texts.Text_AllStatProbability).text = descString;

        // Detail
        ERareType[] validRareTypes = Enum.GetValues(typeof(ERareType))
            .Cast<ERareType>()
            .Where(statType => statType != ERareType.None)
            .ToArray();

        List<int> pbList = Managers.Data.DrawRankUpChart[EHeroRankUpStatType.RankUp_Atk].ProbabilityList;
        string rarePb = string.Empty;

        for (int i = 0; i < pbList.Count; i++)
        {
            string colorCode = Util.GetRareTypeColorCode(validRareTypes[i]);
            string rareTypeName = Util.GetRareTypeString(validRareTypes[i]);

            rarePb += $"<color={colorCode}>{rareTypeName} ({pbList[i]}%)</color>, ";
        }

        if (rarePb.EndsWith(", "))
        {
            rarePb = rarePb.Remove(rarePb.Length - 2);
        }

        GetTMPText((int)Texts.Text_RareProbability).text = rarePb;

        string leftStatRareValue = string.Empty;
        string rightStatRareValue = string.Empty;
        int changer = 0;

        foreach (EHeroRankUpStatType statType in validStatTypes)
        {
            DrawRankUpGachaInfoData rank = Managers.Data.DrawRankUpChart[statType];
            string statName = rank.Name;
            string valueRange =
                $"<color={Util.GetRareTypeColorCode(ERareType.Normal)}>{rank.NormalValueList.First()}~{rank.NormalValueList.Last()}</color> " +
                $"<color={Util.GetRareTypeColorCode(ERareType.Advanced)}>{rank.AdvanceValueList.First()}~{rank.AdvanceValueList.Last()}</color> " +
                $"<color={Util.GetRareTypeColorCode(ERareType.Rare)}>{rank.RareValueList.First()}~{rank.RareValueList.Last()}</color> \n" +
                $"<color={Util.GetRareTypeColorCode(ERareType.Legendary)}>{rank.LegendaryValueList.First()}~{rank.LegendaryValueList.Last()}</color> " +
                $"<color={Util.GetRareTypeColorCode(ERareType.Mythical)}>{rank.MythicalValueList.First()}~{rank.MythicalValueList.Last()}</color>";
            if (changer < 3)
            {
                leftStatRareValue += $"{statName}\n{valueRange}\n\n";
            }
            else
            {
                rightStatRareValue += $"{statName}\n{valueRange}\n\n";
            }

            ++changer;
        }

        GetTMPText((int)Texts.Text_AllStatProbability_Left).text = leftStatRareValue;
        GetTMPText((int)Texts.Text_AllStatProbability_Right).text = rightStatRareValue;
    }
}