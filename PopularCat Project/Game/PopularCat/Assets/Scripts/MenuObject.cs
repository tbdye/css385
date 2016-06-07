using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuObject : MonoBehaviour
{
	#region Public Fields

	public Button myButton;

	#endregion

	#region Private Fields
	
	CircularArea detectionArea;
	GameObject player;

	#endregion
	
	#region Private Methods

	void Awake()
	{
		player = GameObject.Find("Player");
		detectionArea = gameObject.GetComponent<CircularArea>();
		
	}
	
	void Start()
	{
		var c = myButton.colors;
		c.normalColor = Color.clear;
		myButton.colors = c;
	}

	void Update()
	{
		var newPos = Camera.main.WorldToScreenPoint(transform.position);
		newPos.y *= 1;

		myButton.GetComponent<RectTransform>().position = newPos;

		if (GameState.Paused)
			return;

		if (
			detectionArea.Contains(player) &&
			Input.GetButtonDown("Meow")
			)
		{
			myButton.onClick.Invoke();
		}
	}

	#endregion
}