using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action OnClickHandler = null;
    public Action OnPressedHandler = null;
    public Action OnPointerDownHandler = null;
    public Action OnPointerUpHandler = null;

    private bool _pressed = false;
    private float _pressInterval = 0.2f; // 반복 실행 간격 (초 단위)

    private void Update()
    {
        // 매 프레임이 아니라 일정 간격으로 반복 실행하도록 함
        if (_pressed && !IsInvoking(nameof(InvokePressedHandler)))
        {
            InvokeRepeating(nameof(InvokePressedHandler), 0f, _pressInterval);
        }
    }

    private void InvokePressedHandler()
    {
        OnPressedHandler?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        OnPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        OnPointerUpHandler?.Invoke();
        CancelInvoke(nameof(InvokePressedHandler)); // 버튼에서 손을 뗄 때 반복 중지
    }
}
