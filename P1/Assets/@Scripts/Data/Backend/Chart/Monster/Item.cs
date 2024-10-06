using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace BackendData.Chart.Monster
{
    public class Item
    {
        public int DataId { get; private set; }
        public string Name { get; private set; }
        public EObjectType ObjectType { get; private set; }
        public string PrefabKey { get; private set; }
        public int Atk { get; private set; }
        public int MaxHp { get; private set; }
        public float MoveSpeed { get; private set; }

        public Item(JsonData json)
        {
            DataId = int.Parse(json["DataId"].ToString());
            Name = json["Name"].ToString();
            ObjectType = Util.ParseEnum<EObjectType>(json["ObjectType"].ToString());
            PrefabKey = json["PrefabKey"].ToString();
            Atk = int.Parse(json["Atk"].ToString());
            MaxHp = int.Parse(json["MaxHp"].ToString());
            MoveSpeed = float.Parse(json["MoveSpeed"].ToString());
        }
    }

}