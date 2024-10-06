using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Chart.Stage
{
    //===============================================================
    // StageInfoData 차트의 데이터를 관리하는 클래스
    //===============================================================
    public class Manager : Base.Chart
    {
        private readonly Dictionary<int, Item> _dic = new();
        public IReadOnlyDictionary<int, Item> Dic => (IReadOnlyDictionary<int, Item>)_dic.AsReadOnlyCollection();

        //private readonly List<Item> _list = new List<Item>();
        //public IReadOnlyList<Item> List => (IReadOnlyList<Item>)_list.AsReadOnlyList();

        // 차트 파일 이름 설정 함수
        // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
        public override string GetChartFileName()
        {
            return "StageInfoData";
        }

        // Backend.Chart.GetChartContents에서 각 차트 형태에 맞게 파싱하는 클래스
        // 차트 정보 불러오는 함수는 BackendData.Base.Chart의 BackendChartDataLoad를 참고
        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);
                _dic.Add(info.StageNumber, info);
            }
        }
    }


}
