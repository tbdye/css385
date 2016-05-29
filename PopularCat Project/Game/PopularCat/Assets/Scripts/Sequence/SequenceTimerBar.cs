using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SequenceTimerBar : MonoBehaviour
{
	public RectTransform greyBar, redBar, ball;

	public float Value
	{
		get { return currentval; }
		set
		{
			currentval = Mathf.Clamp01(value);
		}
	}
	public float Visibility
	{
		get { return vis; }
		set
		{
			vis = Mathf.Clamp01(value);
		}
	}

	float vis, currentval;
	
	
	void Update ()
	{
		foreach (var i in GetComponentsInChildren<Image>())
		{
			var temp = i.color;
			temp.a = Visibility;
			i.color = temp;
		}

		{
			var temp = redBar.anchoredPosition;
			temp.x = greyBar.rect.xMin - redBar.rect.xMax;
			temp.x *= Value;
			redBar.anchoredPosition = temp;
		}

		{
			var temp = ball.anchoredPosition;
			temp.x = redBar.rect.xMax + redBar.anchoredPosition.x;
			ball.anchoredPosition = temp;
		}

	}
}
