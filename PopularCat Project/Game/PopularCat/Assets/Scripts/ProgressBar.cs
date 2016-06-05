using System;
using System.Collections;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
	#region Private Fields

	bool  previouslyEmpty;
	bool  previouslyFull;
	float val;

	#endregion

	#region Public Properties

	public float  End        { get; private set; }
	public float  Magnitude  { get { return Value / End; } }

	public float Value
	{
		get
		{ return val; }
		set
		{
			val = Mathf.Clamp(value, 0, End);
		}
	}

	public float Visibility { get; set; }

	#endregion

	#region Private Properties

	SpriteRenderer BarBaseRenderer { get; set; }
	SpriteRenderer BarRenderer { get; set; }
	Transform BarTransform { get; set; }
	Gradient ColorGradient { get; set; }

	#endregion

	#region Private Methods

	void Awake()
	{
		BarBaseRenderer = GetComponent<SpriteRenderer>();
		BarRenderer = transform.FindChild("ProgressBar").gameObject.GetComponent<SpriteRenderer>();
		BarTransform = BarRenderer.transform;
		{
			ColorGradient = new Gradient();
			var colorKeys = new GradientColorKey[4];
			colorKeys[0].color = Color.red;
			colorKeys[0].time = 0f;
			colorKeys[1].color = Color.yellow;
			colorKeys[1].time = 0.3f;
			colorKeys[2].color = Color.green;
			colorKeys[2].time = 0.9f;
			colorKeys[3].color = Color.blue;
			colorKeys[3].time = 1f;
			var alphaKeys = new GradientAlphaKey[1];
			alphaKeys[0].alpha = 1;
			alphaKeys[0].time = 0;
			ColorGradient.SetKeys(colorKeys, alphaKeys);
		}
		End = 100;
		Value = 33;
	}

	void Start()
	{
	}

	void Update()
	{


		float root = -0.065f;
		root += Magnitude * 0.065f;
		BarTransform.localPosition = new Vector3(root, 0, 0);
		BarTransform.localScale = new Vector3(Magnitude, 1, 1);
		Color c = ColorGradient.Evaluate(Magnitude);
		c.a = Visibility;
		BarRenderer.color = c;

		BarBaseRenderer.color = new Color(0, 0, 0, Visibility);
	}

	#endregion
}