using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

namespace BackendData.Chart.DrawEquipmentGacha
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

        public List<int> NormalEqIdList { get; private set; }
        public List<int> AdvancedEqIdList { get; private set; }
        public List<int> RareEqIdList { get; private set; }
        public List<int> LegendaryEqIdList { get; private set; }
        public List<int> MythicalEqIdList { get; private set; }
        public List<int> CelestialEqIdList { get; private set; }

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

            NormalEqIdList = Util.ParseList<int>(json["NormalEqIdList"].ToString());
            AdvancedEqIdList = Util.ParseList<int>(json["AdvancedEqIdList"].ToString());
            RareEqIdList = Util.ParseList<int>(json["RareEqIdList"].ToString());
            LegendaryEqIdList = Util.ParseList<int>(json["LegendaryEqIdList"].ToString());
            MythicalEqIdList = Util.ParseList<int>(json["MythicalEqIdList"].ToString());
            CelestialEqIdList = Util.ParseList<int>(json["CelestialEqIdList"].ToString());
        }


    }

}