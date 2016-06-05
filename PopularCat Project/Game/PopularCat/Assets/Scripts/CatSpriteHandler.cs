using System;
using System.Collections;
using UnityEngine;

public class CatSpriteHandler : MonoBehaviour
{
	[Serializable]
	public class SpritePair
	{
		public Sprite first,second;
	}
	#region Public Fields

	public Vector2 timeInSitPosition;
	public Vector2 timeUntilCatSits;
	public Sprite[] CatSit;
	public Sprite   CatStand;
	public Sprite[] CatWalk;
	public Sprite[] CatIdleDance;
	public SpritePair[] CatActiveDances;
	public Sprite[] CatFail;
	public float    walkCycleLength = 0.55f;
	public float    danceCycleLength = 0.88f;

	#endregion

	#region Private Fields

	Vector3        lastPos;
	SpriteRenderer render;
	Timer          sitLookTimer;
	Timer          sitTimer;
	Timer          walkTimer;
	Timer          idleDanceTimer;
	Timer          failAnimTimer;
	Timer          activeDanceTimer;
	Rigidbody2D    rigidBody;

	static float activePoseSelector;
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

		Sprite last = render.sprite;
		do
		{
			render.sprite = CatSit.RandomItem();
		} while (render.sprite == last);
	}

	void SpriteDirection()
	{
		if (rigidBody != null)
		{
			if (rigidBody.velocity.x.Abs() > 0.02f)
				render.flipX = rigidBody.velocity.x < 0;
			
			return;
		}

		float currentX = transform.position.x;

		if (Mathf.Abs(currentX - lastPos.x) > 0.01f * Time.deltaTime)
			render.flipX = lastPos.x > currentX;
	}

	void MatchPlayerDirection()
	{
		render.flipX = Player.Instance.GetComponent<SpriteRenderer>().flipX;
	}

	void Start()
	{
		rigidBody = GetComponent<Rigidbody2D>();

		sitLookTimer =
			TimeManager.GetNewTimer(
				loops: true,
				onComplete: SitLook);

		sitTimer =
			TimeManager.GetNewTimer(
				onComplete: sitLookTimer.Resume);

		lastPos = transform.position;
		render = GetComponent<SpriteRenderer>();
		
		walkTimer = TimeManager.GetNewTimer(
			walkCycleLength, 
			onTick: (dt) => 
			render.sprite = 
				CatWalk.AccessByMagnitude(walkTimer.Completion),
			loops: true);

		idleDanceTimer = TimeManager.GetNewTimer(
			danceCycleLength,
			onTick: (dt) => 
			render.sprite = 
				CatIdleDance.AccessByMagnitude(idleDanceTimer.Completion),
			loops: true);

		failAnimTimer = TimeManager.GetNewTimer(1.3f);
		failAnimTimer.OnTime[0] = () => render.sprite = CatFail[0];
		failAnimTimer.OnTime[0.1f] = () => render.sprite = CatFail[1];
		failAnimTimer.OnTime[0.15f] = () => render.sprite = CatFail[2];
		failAnimTimer.OnTime[0.2f] = () => render.sprite = CatFail[1];
		failAnimTimer.OnTime[0.75f] = () => render.sprite = CatFail[2];
		failAnimTimer.OnTime[0.8f] = () => render.sprite = CatFail[1];
		failAnimTimer.OnTime[0.85f] = () => render.sprite = CatFail[2];
		failAnimTimer.OnTime[0.9f] = () => render.sprite = CatFail[1];
		failAnimTimer.OnComplete = () => failQueued = false;

		activeDanceTimer = TimeManager.GetNewTimer();
		activeDanceTimer.OnTime[0.3f] = 
			() => 
			render.sprite = 
				CatActiveDances.AccessByMagnitude(activePoseSelector).second;
	}

	static int currentFrame;

	public void GoodAnim()
	{
		if (activeDanceTimer.Running)
			return;

		bool play = GetComponent<Player>() != null;
		if (!play)
		{
			var s = GetComponent<Swarmer>();
			if (s == null || !s.InSwarm)
			{
				return;
			}
		}

		if (currentFrame != Time.frameCount)
		{
			activePoseSelector = UnityEngine.Random.Range(0f, 1f);
		}
		currentFrame = Time.frameCount;

		sitTimer.Stop();
		sitLookTimer.Stop();
		walkTimer.Stop();
		idleDanceTimer.Pause();
		activeDanceTimer.Run();

		render.sprite = CatActiveDances.AccessByMagnitude(activePoseSelector).first;
	}

	void FailAnim()
	{
		if(GetComponent<Player>() == null)
			render.flipX = UnityEngine.Random.Range(0, 2) > 0.5f;
		sitTimer.Stop();
		sitLookTimer.Stop();
		walkTimer.Stop();
		idleDanceTimer.Pause();
		failAnimTimer.Run();
	}

	bool failQueued;
	public void QueueFailAnim()
	{
		if (failQueued)
			return;

		bool play = GetComponent<Player>() != null;

		if (!play)
		{
			var s = GetComponent<Swarmer>();
			if (s == null || !s.InSwarm)
			{
				return;
			}
		}

		ReadOnlyTimer randDelay = TimeManager.Delay(FailAnim, UnityEngine.Random.Range(0, 0.4f));
		failQueued = true;
	}

	void Update()
	{
		bool play = GetComponent<Player>() != null;

		
		if(GameState.EndOfLevel)
		{
			if (!GameState.EndOfLevelPassed)
			{
				if (!failAnimTimer.Running && render.sprite != CatFail[1])
					QueueFailAnim();
				return;
			}
		}

		if (GameState.InEncounter)
			InEncounterAnim(play);
		else
		{

			OutEncounterAnim(play);
		}
		
		
		lastPos = transform.position;
	}

	void InEncounterAnim(bool play)
	{
		if (failAnimTimer.Running || activeDanceTimer.Running)
			return;

		bool test = play;

		if(!test)
		{
			var s = GetComponent<Swarmer>();
			if(s != null)
				test = s.InSwarm;
		}


		if(test)
		{
			walkTimer.Stop();
			sitTimer.Stop();
			sitLookTimer.Stop();

			idleDanceTimer.Resume();
			MatchPlayerDirection();
			return;
		}

		OutEncounterAnim(play);
	}
	void OutEncounterAnim(bool play)
	{
		if ((lastPos - transform.position).magnitude > 0.001f)
			activeDanceTimer.Stop();

		if (activeDanceTimer.Running)
			return;

		idleDanceTimer.Stop();

		bool test = play ? 
			Utils.InputVector.magnitude > 0 :
			(lastPos - transform.position).magnitude > 0.01f;
		if (test)
		{
			walkTimer.Resume();
			sitLookTimer.Stop();
			sitTimer.Stop();
			SpriteDirection();
		}
		else
		{
			if (!sitTimer.Running && !sitLookTimer.Running)
			{
				render.sprite = CatStand;
				sitTimer.End = Utils.RandomRange(timeUntilCatSits);
				sitTimer.Run();
				walkTimer.Stop();
			}
		}
	}

	#endregion
}