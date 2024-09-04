using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_Joystick : UI_Base
{
	enum GameObjects
	{
		JoystickBG,
		JoystickCursor,
	}

	private float radius;

	private GameObject joystickBG;
	private GameObject joystickCursor;

	private Vector2 joystickTouchPos;
	private Vector2 joystickOriginPos;

	protected override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindObjects(typeof(GameObjects));

		joystickBG = GetObject((int)GameObjects.JoystickBG);
		joystickCursor = GetObject((int)GameObjects.JoystickCursor);
		radius = joystickBG.GetComponent<RectTransform>().sizeDelta.y / 5;

		gameObject.BindEvent(OnPointerDown, type: Define.EUIEvent.PointerDown);
		gameObject.BindEvent(OnPointerUp, type: Define.EUIEvent.PointerUp);
		gameObject.BindEvent(OnDrag, type: Define.EUIEvent.Drag);

		joystickOriginPos = joystickBG.transform.position;

		return true;
	}

	#region Event
	public void OnPointerDown(PointerEventData eventData)
	{
		joystickBG.transform.position = eventData.position;
		joystickCursor.transform.position = eventData.position;
		joystickTouchPos = eventData.position;

		Managers.Game.JoystickState = EJoystickState.PointerDown;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		joystickCursor.transform.position = joystickOriginPos;
		joystickBG.transform.position = joystickOriginPos;

		Managers.Game.MoveDir = Vector2.zero;
		Managers.Game.JoystickState = EJoystickState.PointerUp;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 touchDir = (eventData.position - joystickTouchPos);

		float moveDist = Mathf.Min(touchDir.magnitude, radius);
		Vector2 moveDir = touchDir.normalized;
		Vector2 newPosition = joystickTouchPos + moveDir * moveDist;
		joystickCursor.transform.position = newPosition;

		Managers.Game.MoveDir = moveDir;
		Managers.Game.JoystickState = EJoystickState.Drag;
	}
	#endregion
}
