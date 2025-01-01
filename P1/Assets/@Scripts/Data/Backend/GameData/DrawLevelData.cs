using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

namespace BackendData.GameData
{
    //===============================================================
    // DrawLevelData 테이블의 뽑기 관련 데이터를 담당하는 클래스
    //===============================================================
    public class DrawData
    {
        public int DrawLevel { get; set; }
        public int DrawCount { get; set; }

        public DrawData(int drawlevel, int drawCount)
        {
            DrawLevel = drawlevel;
            DrawCount = drawCount;
        }
    }

    public class DrawLevelData : Base.GameData
    {

        // Draw 각 뽑기 데이터를 담는 Dic
        private Dictionary<string, DrawData> _drawDic = new();

        public IReadOnlyDictionary<string, DrawData> DrawDic => (IReadOnlyDictionary<string, DrawData>)_drawDic.AsReadOnlyCollection();


        protected override void InitializeData()
        {
            _drawDic.Clear();
            _drawDic.Add("Weapon", new DrawData(1, 0));
            _drawDic.Add("Armor", new DrawData(1, 0));
            _drawDic.Add("Ring", new DrawData(1, 0));
            _drawDic.Add("Skill", new DrawData(1, 0));

        }

        protected override void SetServerDataToLocal(JsonData Data)
        {
            foreach (var column in Data.Keys)
            {
                int drawLevel = int.Parse(Data[column]["DrawLevel"].ToString());
                int drawCount = int.Parse(Data[column]["DrawCount"].ToString());
                _drawDic.Add(column, new DrawData(drawLevel, drawCount));
            }

            _drawDic["Weapon"].DrawLevel = 1;
        }

        public override string GetTableName()
        {
            return "DrawLevelData";
        }

        public override string GetColumnName()
        {
            return "DrawLevelData";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), _drawDic);
            return param;
        }

        #region Draw Methods 

        // 유저의 뽑기 횟수를 변경하는 함수
        public void AddDrawCount(EDrawType drawType)
        {
            string key = drawType.ToString();
            if (_drawDic[key].DrawLevel == 10)
            {
                // 최고 레벨에 도달하면 올려줄 필요 없음
                return;
            }

            IsChangedData = true;
            _drawDic[key].DrawCount++;

            if (drawType.IsEquipmentType())
            {
                while (_drawDic[key].DrawCount >= Managers.Data.DrawEquipmentChart[_drawDic[key].DrawLevel].MaxExp) //GachaDataDic[_drawDic[key].DrawLevel].MaxExp)
                {
                    DrawLevelUp(key);
                }
                Managers.Backend.GameData.MissionData.UpdateMission(EMissionType.WeaponSummon);

            }
            if (drawType == EDrawType.Skill)
            {
                while (_drawDic[key].DrawCount >= Managers.Data.DrawSkillChart[_drawDic[key].DrawLevel].MaxExp) //GachaDataDic[_drawDic[key].DrawLevel].MaxExp)
                {
                    DrawLevelUp(key);
                }
                Managers.Backend.GameData.MissionData.UpdateMission(EMissionType.SkillSummon);

            }
        }

        // 유저의 뽑기 레벨을 변경하는 함수
        public void DrawLevelUp(string key)
        {
            IsChangedData = true;
            _drawDic[key].DrawCount -= Managers.Data.DrawEquipmentChart[_drawDic[key].DrawLevel].MaxExp;
            _drawDic[key].DrawLevel++;
            Managers.Event.TriggerEvent(EEventType.DrawLevelUpUIUpdated, _drawDic[key].DrawLevel);
        }

        #endregion

    }
}
