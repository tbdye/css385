using UnityEngine;
using System.Collections;

public class PropTeeter : MonoBehaviour
{
	public float teeterWidth = 8;

	Timer teeterTimer;
	// Use this for initialization
	void Start ()
	{
		teeterTimer = TimeManager.GetNewTimer(
			Random.Range(0.7f, 1.5f),
			loops: true);
		teeterTimer.Run();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float rot = (Mathf.Sin(teeterTimer.Completion * Mathf.PI*2) - 0.5f) * teeterWidth;

		transform.localRotation = Quaternion.Euler(0,0, rot);
	}
}
