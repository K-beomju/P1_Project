using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using BackEnd;
using System;

namespace BackendData.GameData {

    public partial class UserData {
        public int Level { get; private set; }
        public float Gold { get; private set; }
    }

    public partial class UserData : Base.GameData {

        protected override void InitializeData() {
            Level = 1;
            Gold = 200;
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson) {
            Level = int.Parse(gameDataJson["Level"].ToString());
            Gold = float.Parse(gameDataJson["Gold"].ToString());
        }

        public override string GetTableName() {
            return "UserData";
        }

        public override string GetColumnName() {
            return null;
        }

        public override Param GetParam() {
            Param param = new Param();

            param.Add("Level", Level);
            param.Add("Gold", Gold);

            return param;
        }

        public void UpdateUserData(float gold) {
            IsChangedData = true;

            Gold += gold;
        }
    }

}


