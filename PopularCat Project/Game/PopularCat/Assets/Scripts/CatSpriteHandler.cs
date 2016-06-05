using System.Collections;
using UnityEngine;

public class CatSpriteHandler : MonoBehaviour
{
	#region Public Fields

	public Vector2 timeInSitPosition;
	public Vector2 timeUntilCatSits;
	public Sprite[] CatSit;
	public Sprite   CatStand;
	public Sprite[] CatWalk;
	public Sprite[] CatIdleDance;
	public float    walkCycleLength = 0.55f;
	public float    danceCycleLength = 0.88f;
	public bool     isPlayer;

	#endregion

	#region Private Fields

	Vector3        lastPos;
	SpriteRenderer render;
	Timer          sitLookTimer;
	Timer          sitTimer;
	Timer          walkTimer;
	Timer          idleDanceTimer;
	Rigidbody2D    rigidBody;

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

	}


	void Update()
	{
		bool play = GetComponent<Player>() != null;

		
		if(GameState.InEncounter)
			InEncounterAnim(play);
		else
			OutEncounterAnim(play);
		
		
		lastPos = transform.position;
	}

	void InEncounterAnim(bool play)
	{
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