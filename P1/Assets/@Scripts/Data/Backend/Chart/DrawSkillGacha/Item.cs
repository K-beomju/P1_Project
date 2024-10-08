using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

namespace BackendData.Chart.DrawSkillGacha
{
    public class Item
    {
        public int Level { get; private set; }
        public int MaxExp { get; private set; }

        public List<float> DrawProbabilityList { get; private set; }
        public List<float> NormalDrawList { get; private set; }
        public List<float> AdvancedDrawList { get; private set; }
        public List<float> RareDrawList { get; private set; }
        public List<float> LegendaryDrawList { get; private set; }
        public List<float> MythicalDrawList { get; private set; }
        public List<float> CelestialDrawList { get; private set; }

        public List<int> NormalSkillIdList { get; private set; }
        public List<int> AdvancedSkillIdList { get; private set; }
        public List<int> RareSkillIdList { get; private set; }
        public List<int> LegendarySkillIdList { get; private set; }
        public List<int> MythicalSkillIdList { get; private set; }
        public List<int> CelestialSkillIdList { get; private set; }

        public Item(JsonData json)
        {
            Level = int.Parse(json["Level"].ToString());
            MaxExp = int.Parse(json["MaxExp"].ToString());

            // 제네릭 메서드를 활용한 리스트 초기화
            DrawProbabilityList = Util.ParseList<float>(json["DrawProbabilityList"].ToString());
            NormalDrawList = Util.ParseList<float>(json["NormalDrawList"].ToString());
            AdvancedDrawList = Util.ParseList<float>(json["AdvancedDrawList"].ToString());
            RareDrawList = Util.ParseList<float>(json["RareDrawList"].ToString());
            LegendaryDrawList = Util.ParseList<float>(json["LegendaryDrawList"].ToString());
            MythicalDrawList = Util.ParseList<float>(json["MythicalDrawList"].ToString());
            CelestialDrawList = Util.ParseList<float>(json["CelestialDrawList"].ToString());

            NormalSkillIdList = Util.ParseList<int>(json["NormalSkillIdList"].ToString());
            AdvancedSkillIdList = Util.ParseList<int>(json["AdvancedSkillIdList"].ToString());
            RareSkillIdList = Util.ParseList<int>(json["RareSkillIdList"].ToString());
            LegendarySkillIdList = Util.ParseList<int>(json["LegendarySkillIdList"].ToString());
            MythicalSkillIdList = Util.ParseList<int>(json["MythicalSkillIdList"].ToString());
            CelestialSkillIdList = Util.ParseList<int>(json["CelestialSkillIdList"].ToString());
        }
    }
}
