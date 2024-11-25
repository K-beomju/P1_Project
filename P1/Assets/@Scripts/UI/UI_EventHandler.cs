using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action OnClickHandler = null;
    public Action OnPressedHandler = null;
    public Action OnPointerDownHandler = null;
    public Action OnPointerUpHandler = null;

    private float _pressInterval = 0.1f; // 반복 실행 간격 (초 단위)

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownHandler?.Invoke();

        // 반복 실행 시작
        if (!IsInvoking(nameof(InvokePressedHandler)))
        {
            InvokeRepeating(nameof(InvokePressedHandler), 0f, _pressInterval);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpHandler?.Invoke();

        // 반복 실행 중지
        CancelInvoke(nameof(InvokePressedHandler));
    }

    private void InvokePressedHandler()
    {
        OnPressedHandler?.Invoke();
    }
}
