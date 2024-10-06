using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.Chart.HeroUpgrade
{
    public class Manager : Base.Chart
    {
        private readonly Dictionary<string, Item> _dic = new();
        public IReadOnlyDictionary<string, Item> Dic => (IReadOnlyDictionary<string, Item>)_dic.AsReadOnlyCollection();

        public override string GetChartFileName()
        {
            return "heroUpgradeInfoChart";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);
                _dic.Add(info.HeroUpgradeType.ToString(), info);
            }
        }
    }
}
