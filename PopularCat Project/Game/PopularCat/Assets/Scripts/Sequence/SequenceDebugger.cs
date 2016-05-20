using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceDebugger : MonoBehaviour
{
	#region Public Fields
	public ProgressBar progressBar;
	public GameObject indicatorPrefab;

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

			rt.anchoredPosition3D = new Vector3(i * 60, -60, 0);
			if (details.Identifier[0] == '-')
				obj.transform.Rotate(0, 0, 180);
			if (details.Identifier[1] == 'H')
				obj.transform.Rotate(0, 0, -90);

			i += 1;
		}
	}

	#endregion

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
		progressBar.Value = mySequence.TimeLimit.Remaining01 * 100;

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
				indicators[i].Renderers[0].color = Color.gray;
			}
		}
	}

	#endregion
}