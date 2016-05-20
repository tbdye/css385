using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBool : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	public bool IsPressed { get; private set; }

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		IsPressed = true;
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		IsPressed = false;
	}
}