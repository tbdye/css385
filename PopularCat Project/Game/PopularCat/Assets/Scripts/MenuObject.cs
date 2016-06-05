using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuObject : MonoBehaviour
{
	#region Public Fields
	
	


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
	}

	void Update()
	{

		if (GameState.Paused)
			return;

		if (
			detectionArea.Contains(player) &&
			Input.GetButtonDown("Meow")
			)
		{
			//Do the thing
		}
		this.ClampToBoundary();
	}

	#endregion
}