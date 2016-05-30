using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceDebugger : MonoBehaviour
{
	#region Public Fields

	public ArrowRegion arrowRegion;
	public SequenceTimerBar progressBar;
	public GameObject indicatorPrefab;
	public float randomTipRange = 5;
	public float curvePower = 13;
	public float verticalOffset = -45;

	#endregion

	#region Private Fields

	static SequenceDebugger instance;
	Sequence mySequence;
	List<InputIndicator> indicators;

	#endregion

	#region Public Methods

	public static void End()
	{
		if (instance.mySequence == null)
			return;

		instance.mySequence = null;
		foreach (var i in instance.indicators)
		{
			foreach (var r in i.Renderers)
				r.color = Color.clear;
			Destroy(i.gameObject);
		}
		instance.indicators.Clear();
		instance.indicators = null;
	}

	public static void Setup(Sequence seq)
	{
		if (instance.indicators != null)
			End();

		instance.mySequence = seq;
		instance.indicators = new List<InputIndicator>();

		float i = (float)-seq.Length / 2;
		i += 0.5f;
		foreach (SequenceItemDetails details in seq.Details)
		{
			var obj =
				Instantiate(instance.indicatorPrefab,
					Vector3.zero,
					instance.transform.rotation)
						as GameObject;

			instance.indicators.Add(obj.GetComponent<InputIndicator>());
			var rt = obj.GetComponent<RectTransform>();

			rt.anchorMin = new Vector2(0.5f, 1);
			rt.anchorMax = new Vector2(0.5f, 1);

			var yval = Mathf.Abs(i) / seq.Length * -instance.curvePower;

			rt.anchoredPosition3D = new Vector3(i * 60, yval+instance.verticalOffset, 0);

			var sr = obj.GetComponent<Image>();

			sr.sprite = details.Sprite;
			float r = Random.Range(-instance.randomTipRange, instance.randomTipRange);
			obj.transform.Rotate(0, 0, details.SpriteRotation + r);

			i += 1;
		}
	}

	#endregion

	public static void FailAnim()
	{
		var rect = instance.arrowRegion.GetComponent<RectTransform>();
		var temp = rect.anchoredPosition;

		int alt = 1;
		Timer t = TimeManager.GetNewTimer(0.245f);
		Timer alternator = TimeManager.GetNewTimer(0.025f, loops: true);
		alternator.Current = 0.0125f;
		alternator.OnComplete = () =>
		{
			alt *= -1;
		};

		t.OnTick = (dt) =>
		{
			rect.anchoredPosition += new Vector2(alt * 600 * dt, 0);
		};

		t.OnComplete = () =>
		{
			temp.x = 0;
			rect.anchoredPosition = temp;
			alternator.Dispose();
			t.Dispose();
		};

		alternator.Run();
		t.Run();
	}

	public static void VictoryAnim()
	{
		Timer t = TimeManager.GetNewTimer(0.245f);
		t.OnTick = (dt) =>
		{
			if (GameState.EndOfLevel)
			{
				return;
			}

			bool alt = false;
			foreach (var i in instance.indicators)
			{
				int d = (alt = !alt) ? 1 : -1;
				i.transform.Rotate(0, 0, 360 * dt * 4 * d);
			}
		};
		t.OnComplete = t.Dispose;
		t.Run();
	}

	public static void TimeOutAnim()
	{
		foreach (var i in instance.indicators)
		{
			i.Renderers[0].color = new Color(0.333f, 0, 0);
		}
	}
	#region Private Methods

	void Awake()
	{
		instance = this;
	}

	void LateUpdate()
	{
		if (GameState.EndOfLevel && mySequence != null)
		{
			End();
		}

		if (mySequence == null)
		{
			progressBar.Visibility = 0;
			return;
		}

		progressBar.Visibility = 1;
		progressBar.Value = mySequence.TimeLimit.Remaining01;

		if(mySequence.TimeLimit.Running)
		for (int i = 0; i < mySequence.Details.Count; i++)
		{
			if (mySequence.Details[i].Passed)
			{
				indicators[i].Renderers[0].color = Color.green;
			}
			else if (mySequence.Details[i].Failed)
			{
				indicators[i].Renderers[0].color = Color.red;
			}
			else
			{
				indicators[i].Renderers[0].color = Color.white;
			}
		}
	}

	#endregion
}