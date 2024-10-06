using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using static Define;

namespace BackendData.Chart.Hero
{
    public class Item
    {
        public int DataId { get; private set; }
        public string Name { get; private set; }
        public EObjectType ObjectType { get; private set; }
        public string PrefabKey { get; private set; }
        public float Atk { get; private set; }
        public float MaxHp { get; private set; }
        public float Recovery { get; private set; }
        public float CriRate { get; private set; }
        public float CriDmg { get; private set; }
        public float AttackRange { get; private set; }
        public float AttackDelay { get; private set; }
        public float AttackSpeedRate { get; private set; }

        public Item(JsonData json)
        {
            DataId = int.Parse(json["DataId"].ToString());
            Name = json["Name"].ToString();
            ObjectType = Util.ParseEnum<EObjectType>(json["ObjectType"].ToString());
            PrefabKey = json["PrefabKey"].ToString();
            Atk = float.Parse(json["Atk"].ToString());
            MaxHp = float.Parse(json["MaxHp"].ToString());
            Recovery = float.Parse(json["Recovery"].ToString());
            CriRate = float.Parse(json["CriRate"].ToString());
            CriDmg = float.Parse(json["CriDmg"].ToString());
            AttackRange = float.Parse(json["AttackRange"].ToString());
            AttackDelay = float.Parse(json["AttackDelay"].ToString());
            AttackSpeedRate = float.Parse(json["AttackSpeedRate"].ToString());
        }
    }

}