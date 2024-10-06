using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using static Define;

namespace BackendData.Chart.HeroUpgrade
{
    public class Item
    {
        public EHeroUpgradeType HeroUpgradeType { get; private set; }
        public string Remark { get; private set; }
        public float Value { get; private set; }
        public float IncreaseValue { get; private set; }

        public Item(JsonData json)
        {
            HeroUpgradeType = Util.ParseEnum<EHeroUpgradeType>(json["HeroUpgradeType"].ToString());
            Remark = json["Remark"].ToString();
            Value = float.Parse(json["Value"].ToString());
            IncreaseValue = float.Parse(json["IncreaseValue"].ToString());
        }
    }

}