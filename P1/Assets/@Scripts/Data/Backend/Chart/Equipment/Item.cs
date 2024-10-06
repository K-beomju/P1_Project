using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using static Define;

namespace BackendData.Chart.Equipment
{
    public class Item
    {
        public int DataId { get; private set; }
        public ERareType RareType { get; private set; }
        public EEquipmentType EquipmentType { get; private set; }
        public string SpriteKey { get; private set; }
        public string Name { get; private set; }
        public float OwnedValue { get; private set; }
        public float EquippedValue { get; private set; }

        public Item(JsonData json)
        {
            DataId = int.Parse(json["DataId"].ToString());
            RareType = Util.ParseEnum<ERareType>(json["RareType"].ToString());
            EquipmentType = Util.ParseEnum<EEquipmentType>(json["EquipmentType"].ToString());
            SpriteKey = json["SpriteKey"].ToString();
            Name = json["Name"].ToString();
            OwnedValue = float.Parse(json["OwnedValue"].ToString());
            EquippedValue = float.Parse(json["EquippedValue"].ToString());
        }
    }

}