using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Stage
{
    //===============================================================
    // StageInfoData 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item
    {
        public int StageNumber { get; private set; }
        public int KillMonsterCount { get; private set; }
        public int BossDataId { get; private set; }
        public int BossBattleTimeLimit { get; private set; }
        public List<int> MonsterDataIdList { get; private set; }
        public int MonsterLevel { get; private set; }
        public int MonsterGoldReward { get; private set; }
        public int MonsterExpReward { get; private set; }

        public Item(JsonData json)
        {
            StageNumber = int.Parse(json["StageNumber"].ToString());
            KillMonsterCount = int.Parse(json["KillMonsterCount"].ToString());
            BossDataId = int.Parse(json["BossDataID"].ToString());
            BossBattleTimeLimit = int.Parse(json["BossBattleTimeLimit"].ToString());

            MonsterDataIdList = new List<int>();
            JsonData stageMonsterIdListJson = json["MonsterDataIdList"].ToString();
            string[] monsterIdArray = stageMonsterIdListJson.ToString().Split('&'); // "&"로 문자열을 분리

            foreach (string id in monsterIdArray)
            {
                if (int.TryParse(id, out int monsterId))
                {
                    MonsterDataIdList.Add(monsterId);
                }
            }

            MonsterLevel = int.Parse(json["MonsterLevel"].ToString());
            MonsterGoldReward = int.Parse(json["MonsterGoldReward"].ToString());
            MonsterExpReward = int.Parse(json["MonsterExpReward"].ToString());
        }
    }

}
