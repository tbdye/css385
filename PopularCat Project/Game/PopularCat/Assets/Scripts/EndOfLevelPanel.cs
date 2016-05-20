using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndOfLevelPanel : MonoBehaviour
{
	#region Public Fields

	public float moveSpeed = 400;
	public Text scoreText;
	#endregion

	#region Private Fields

	Vector3 destination;
	Vector3 hiddenPositon;
	Vector3 homePositon;
	float mag;
	float slowMag;
	StarMask[] stars;
	RectTransform transf;
	bool vis;

	#endregion

	#region Public Properties

	public Text EndMessage { get { return GameObject.Find("EoLHeaderText").GetComponent<Text>(); } }

	public bool Visible
	{
		get { return vis; }
		set
		{
			vis = value;

			destination = vis ? homePositon : hiddenPositon;

			scoreText.text = ScoreManager.ScoreString;
		}
	}

	#endregion

	#region Private Methods

	void Awake()
	{
		transf = GetComponent<RectTransform>();
		homePositon = transf.anchoredPosition3D;

		hiddenPositon = homePositon + new Vector3(0, 800);
		destination = hiddenPositon;
		transf.anchoredPosition3D = hiddenPositon;
	}

	void Update()
	{
		var direction = transf.anchoredPosition3D.DistanceTo(destination);

		if (direction.magnitude > moveSpeed * Time.deltaTime)
			direction = direction.normalized * moveSpeed * Time.deltaTime;
		transf.anchoredPosition3D += direction;
	}

	#endregion
}