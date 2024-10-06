using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using static Define;
using System;

namespace BackendData.Chart.HeroUpgradeCost
{
    public class Item
    {
        public EHeroUpgradeType HeroUpgradeType { get; private set; }
        public string Remark { get; private set; }
        public List<int> ReferenceLevelList { get; private set; }
        public List<EGoodType> GoodList { get; private set; }
        public List<int> StartCostList { get; private set; }
        public List<int> IncreaseCostList { get; private set; }

        public Item(JsonData json)
        {
            HeroUpgradeType = Util.ParseEnum<EHeroUpgradeType>(json["HeroUpgradeType"].ToString());
            Remark = json["Remark"].ToString();
            ReferenceLevelList = Util.ParseList<int>(json["ReferenceLevelList"].ToString());
            GoodList = Util.ParseList<EGoodType>(json["GoodList"].ToString());
            StartCostList = Util.ParseList<int>(json["StartCostList"].ToString());
            IncreaseCostList = Util.ParseList<int>(json["IncreaseCostList"].ToString());


        }
    }

}