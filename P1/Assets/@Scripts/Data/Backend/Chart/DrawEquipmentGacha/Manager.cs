using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.Chart.DrawEquipmentGacha
{
    public class Manager : Base.Chart
    {
        private readonly Dictionary<int, Item> _dic = new();
        public IReadOnlyDictionary<int, Item> Dic => (IReadOnlyDictionary<int, Item>)_dic.AsReadOnlyCollection();

        public override string GetChartFileName()
        {
            return "drawEquipmentGachaInfoChart";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);
                _dic.Add(info.Level, info);
            }
        }
    }
}
