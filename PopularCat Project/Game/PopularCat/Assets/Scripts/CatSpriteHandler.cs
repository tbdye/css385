using System.Collections;
using UnityEngine;

public class CatSpriteHandler : MonoBehaviour
{
	#region Public Fields

	public Sprite[] CatSit;
	public Sprite   CatStand;
	public Sprite   CatWalk;
	public Vector2  timeInSitPosition;
	public Vector2  timeUntilCatSits;

	#endregion

	#region Private Fields

	Vector3        lastPos;
	SpriteRenderer render;
	Timer          sitLookTimer;
	Timer          sitTimer;

	#endregion

	#region Private Methods

	void OnValidate()
	{
		Utils.MinMaxValidate(ref timeUntilCatSits);
		Utils.MinMaxValidate(ref timeInSitPosition);
	}

	void SitLook()
	{
		sitLookTimer.End = Utils.RandomRange(timeInSitPosition);

		render.sprite = CatSit.RandomItem();
	}

	void SpriteDirection()
	{
		float currentX = transform.position.x;
		if (Mathf.Abs(currentX - lastPos.x) < 0.001f)
			return;
		GetComponent<SpriteRenderer>().flipX = lastPos.x > currentX;
	}

	void Start()
	{
		sitLookTimer =
			TimeManager.GetNewTimer(
				loops: true,
				onComplete: SitLook);

		sitTimer = 
			TimeManager.GetNewTimer(
				onComplete: sitLookTimer.Resume);

		lastPos = transform.position;
		render = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if ((lastPos - transform.position).magnitude < 0.01f)
		{
			if (!sitTimer.Running)
			{
				render.sprite = CatStand;
				sitTimer.End = Utils.RandomRange(timeUntilCatSits);
				sitTimer.Resume();
			}
		}
		else
		{
			render.sprite = CatWalk;
			sitLookTimer.Stop();
			sitTimer.Stop();
			SpriteDirection();
		}
		lastPos = transform.position;
	}

	#endregion
}