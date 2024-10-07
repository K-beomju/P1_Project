using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace BackendData.Chart.CreatureUpgradeStat
{
    public class Item
    {
        public int DataId { get; private set; }
        public string Name { get; private set; }
        public float IncreaseAtk { get; private set; }
        public float IncreaseMaxHp { get; private set; }

        public Item(JsonData json)
        {
            DataId = int.Parse(json["DataId"].ToString());
            Name = json["Name"].ToString();
            IncreaseAtk = float.Parse(json["IncreaseAtk"].ToString());
            IncreaseMaxHp = float.Parse(json["IncreaseMaxHp"].ToString());
        }
    }

}