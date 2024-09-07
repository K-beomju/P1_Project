using System;
using System.Collections.Generic;
using static Define;

public class EventManager
{
    private Dictionary<EEventType, Delegate> _eventDictionary = new Dictionary<EEventType, Delegate>();

    // 이벤트 추가 (다양한 인자를 수용할 수 있도록 Delegate 사용)
    public void AddEvent(EEventType eventType, Delegate listener)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = Delegate.Combine(_eventDictionary[eventType], listener);
        }
        else
        {
            _eventDictionary[eventType] = listener;
        }
    }

    // 이벤트 호출 (여러 인자를 처리하기 위해 DynamicInvoke 사용)
    public void TriggerEvent(EEventType eventType, params object[] parameters)
    {
        if (_eventDictionary.TryGetValue(eventType, out var thisEvent))
        {
            thisEvent?.DynamicInvoke(parameters);
        }
    }

    // 이벤트 제거
    public void RemoveEvent(EEventType eventType, Delegate listener)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = Delegate.Remove(_eventDictionary[eventType], listener);
        }
    }

    // 모든 이벤트 삭제
    public void Clear()
    {
        _eventDictionary.Clear();
    }
}
