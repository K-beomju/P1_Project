using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace BackendData.Chart.Skill
{
    public class Item
    {
        public int DataId { get; private set; }
        public ERareType RareType { get; private set; }

        public string Name { get; private set; }
        public string Description { get; private set; } 
        public string PrefabKey { get; private set; }
        public string SpriteKey { get; private set; }
        public string SoundKey { get; private set; }

        public float OwnedValue { get; private set; }
        public float UsedValue { get; private set; }

        public float CoolTime { get; private set; }
        public float SkillDuration { get; private set; }
        public float SkillRange { get; private set; }
        public int SkillCount { get; private set; }
        public int TargetCount { get; private set; }

        public Item(JsonData json)
        {
            DataId = int.Parse(json["DataId"].ToString());
            RareType = Util.ParseEnum<ERareType>(json["RareType"].ToString());

            Name = json["Name"].ToString();
            Description = json["Description"].ToString();
            PrefabKey = json["PrefabKey"].ToString();
            SpriteKey = json["SpriteKey"].ToString();
            SoundKey = json["SoundKey"].ToString();

            OwnedValue = float.Parse(json["OwnedValue"].ToString());
            UsedValue = float.Parse(json["UsedValue"].ToString());

            CoolTime = float.Parse(json["CoolTime"].ToString());
            SkillDuration = float.Parse(json["SkillDuration"].ToString());
            SkillRange = float.Parse(json["SkillRange"].ToString());
            SkillCount = int.Parse(json["SkillCount"].ToString());
            TargetCount = int.Parse(json["TargetCount"].ToString());
        }
    }

}
