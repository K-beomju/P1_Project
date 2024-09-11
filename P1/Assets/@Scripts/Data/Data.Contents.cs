using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
    #region StageData
    [Serializable]
    public class StageData
    {
        public int StageNumber;
        public EStageType StageType;
        public int MonsterCount;
        
        public bool BossStage;
        public int BossDataId;
        public int BossBattleTimeLimit;

        public List<int> MonsterDataIdList;
        public int MonsterLevel; 
        public int MonsterGoldReward; 
    }

    [Serializable]
    public class StageDataLoader : ILoader<int, StageData>
    {
        public List<StageData> heroes = new List<StageData>();
        public Dictionary<int, StageData> MakeDict()
        {
            Dictionary<int, StageData> dict = new Dictionary<int, StageData>();
            foreach (StageData hero in heroes)
                dict.Add(hero.StageNumber, hero);
            return dict;
        }
    }
    #endregion


}
