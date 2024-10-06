using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.Chart.Equipment
{
    public class Manager : Base.Chart
    {
        private readonly Dictionary<int, Item> _dic = new();
        public IReadOnlyDictionary<int, Item> Dic => (IReadOnlyDictionary<int, Item>)_dic.AsReadOnlyCollection();

        public override string GetChartFileName()
        {
            return "equipmentChart";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);
                _dic.Add(info.DataId, info);
            }
        }
    }
}
