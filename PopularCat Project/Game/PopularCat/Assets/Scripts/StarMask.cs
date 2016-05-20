using UnityEngine;
using System.Collections;

public class StarMask : MonoBehaviour
{
	float mag;
	public float Magnitude
	{
		get
		{
			return mag;
		}
		set
		{
			mag = Mathf.Clamp01(value);

			mask.localPosition = new Vector3(0, -100 * (1 - mag), 0);
			image.position = transform.position;
		}
	}

	Transform mask;
	Transform image;
	
	void Start ()
	{
		mask = transform.GetChild(1);
		image = mask.GetChild(0);
	}
}
