using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSet : MonoBehaviour
{
	public float moveSpeed = 100;

	public bool panelElement;
	#region Private Fields

	RectTransform transf;
	bool vis;
	float mag;
	float slowMag;
	StarMask[] stars;
	Vector3 destination;
	Vector3 hiddenPositon;
	Vector3 homePositon;

	#endregion

	#region Public Properties

	float SlowMagnitude
	{
		get { return slowMag; }
		set
		{
			slowMag = slowMag.MoveToward(value, 0.5f * Time.deltaTime);

			float total = slowMag * stars.Length;
			int iterations = 0;
			foreach (var s in stars)
			{
				s.Magnitude = total - iterations;
				iterations++;
			}
		}
	}

	public float Magnitude
	{
		get { return mag; }
		set
		{
			mag = Mathf.Clamp01(value);
		}
	}

	public bool Visible
	{
		get { return vis; }
		set
		{
			vis = value;

			destination = vis ? homePositon : hiddenPositon;
		}
	}

	#endregion
	public void Floor()
	{
		int mult = stars.Length;
		float result = mag * mult;
		result = Mathf.Floor(result);
		Magnitude = result / mult;
	}
	#region Private Methods

	void Awake()
	{
		transf = GetComponent<RectTransform>();
		stars = GetComponentsInChildren<StarMask>();
		homePositon = transf.anchoredPosition3D;
		
		hiddenPositon = panelElement ? homePositon : homePositon + new Vector3(0, -150);
		destination = hiddenPositon;
		transf.anchoredPosition3D = hiddenPositon;
	}

	void Update()
	{
		var direction = transf.anchoredPosition3D.DistanceTo(destination);

		if (direction.magnitude > moveSpeed * Time.deltaTime)
			direction = direction.normalized * moveSpeed * Time.deltaTime;
		transf.anchoredPosition3D += direction;

		SlowMagnitude = Magnitude;
	}

	#endregion
}