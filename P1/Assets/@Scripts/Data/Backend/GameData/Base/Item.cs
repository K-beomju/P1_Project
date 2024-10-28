using System;
using static Define;

namespace BackendData.GameData
{
    // 공통 부모 클래스 정의
    public abstract class Item
    {
        public int DataTemplateID { get; private set; }
        public EOwningState OwningState { get; set; }
        public int Level { get; set; }
        public int Count { get; set; }
        public bool IsEquipped { get; set; }

        // 장비 또는 스킬의 공통 속성
        public abstract string Name { get; }
        public abstract string SpriteKey { get; }
        public abstract ERareType RareType { get; }
        //public abstract EItemType ItemType { get; } // 아이템 타입(스킬, 장비) 구분용 Enum

        protected Item(int dataTemplateID, EOwningState owningState, int level, int count, bool isEquipped)
        {
            DataTemplateID = dataTemplateID;
            OwningState = owningState;
            Level = level;
            Count = count;
            IsEquipped = isEquipped;
        }
    }
}
