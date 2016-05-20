using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a filter that darkens the play area.
/// </summary>
public class Blackout : MonoBehaviour
{
	#region Public Fields

	[Range(0, 1)]
	public float activeAlpha = 0.3f;

	[Range(0.001f, 0.1f)]
	public float lerpStrength = 0.01f;

	#endregion

	#region Private Fields
	
	float alphaValue;

	#endregion

	#region Public Properties

	public static bool             Active     { get; private set; }
	public static List<GameObject> IgnoreList { get; private set; }

	#endregion

	#region Public Methods

	public static void Activate()
	{
		foreach (var o in IgnoreList)
		{
			o.GetComponent<SpriteRenderer>().sortingLayerName = "Blackout";
		}
		Active = true;
	}

	public static void Deactivate()
	{
		foreach (var o in IgnoreList)
		{
			o.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		}
		Active = false;
	}

	#endregion

	#region Private Methods

	void Awake()
	{
		IgnoreList = new List<GameObject>();

		{// Start of game fade in
			var sr = GetComponent<SpriteRenderer>();
			sr.color = new Color(0, 0, 0, 1);
		}
	}

	void Update()
	{
		var sr = GetComponent<SpriteRenderer>();
		var c = sr.color;
		float dest = Active ? activeAlpha : 0;

		c.a += (dest - c.a) * lerpStrength;
		sr.color = c;
	}

	#endregion
}