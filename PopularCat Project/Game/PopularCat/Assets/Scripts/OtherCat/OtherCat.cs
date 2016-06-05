using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherCat : MonoBehaviour
{
	#region Public Fields

	public float bailoutPromptWindow = 3;
	public float interestBleedRate = 2;
	public float penaltyOnFailure  = 20;
	public float penaltyOnFailureInPosse  = 25;
	public float progressOnSuccess = 34;
	public float progressOnSuccessInPosse = 10;
	public float startingInterest  = 10;
	public float angryDuration = 3;

	[Header("Sequence difficulty variables")]
	public int[] sequenceLengths;
	public float perSequenceTimeLimit = 6;

	public bool isHuman;
	public float fameRate = 10;
	
	public float timeBoostPerMember = 0.33f;
	public float fameBoostPerMember = 2;

	public InputTypes inputPool = InputTypes.NormalArrows;
	public InputTypes[] fameInputPool = 
		{InputTypes.NormalArrows,
		InputTypes.NormalArrows,
		InputTypes.NormalArrows};



	#endregion

	#region Private Fields

	Timer pissedOffTimer;
	CircularArea detectionArea;
	GameObject player;
	bool previousSequenceFailed;

	#endregion

	#region Public Properties

	public bool Impressed { get; private set; }

	#endregion

	#region Private Properties

	Sequence CurrentSequence { get; set; }
	MonitoredValue Interest { get; set; }
	Timer PassiveDrain { get; set; }
	ProgressBar Progress { get; set; }

	#endregion

	#region Private Methods

	void Awake()
	{
		player = GameObject.Find("Player");
		detectionArea = gameObject.GetComponent<CircularArea>();
		Progress = GetComponentInChildren<ProgressBar>();
		Interest = new MonitoredValue(
			min: 0,
			max: 100,
			startingValue: startingInterest,
			onModified: (v) => Progress.Value = v);

	}

	void BeginFameSequence()
	{
		GameState.BeginFameEncounter();

		Progress.Visibility = 0;

		Interest.OnEmpty = null;

		if (CurrentSequence != null)
		{
			CurrentSequence.Dispose();
			CurrentSequence = null;
		}

		int level = 0;

		level += (int) Mathf.Floor(GameState.Fame*3);



		float tboost = timeBoostPerMember;
		tboost *= ScoreManager.Cats.Count;

		CurrentSequence =
			SequenceManager.GetNewSequence
				(sequenceLengths.AccessByMagnitude(Progress.Magnitude),
				timeLimit: perSequenceTimeLimit + tboost,
				types: fameInputPool[level]);


		CurrentSequence.OnBadInput = () =>
		{
			SequenceDebugger.FailAnim();
			CurrentSequence.DelayedRetry();
		};

		CurrentSequence.OnSuccess = () =>
		{
			SequenceDebugger.VictoryAnim();

			float fboost = fameBoostPerMember;
			fboost *= ScoreManager.Cats.Count;

			GameState.Fame.Value += (fameRate + fboost) / 100;
			Timer delay = TimeManager.GetNewTimer(0.25f, () =>
			{
				if (GameState.Fame < 1)
					BeginFameSequence();
				else
					FameSequenceComplete();
			});
			delay.Run();
		};

		CurrentSequence.OnTimeout = () =>
		{
			SequenceDebugger.TimeOutAnim();

			CurrentSequence.Fail();

			if(GameState.Fame == 0)
			{
				GameState.EndFameEncounter();
			}
			else
			{
				GameState.Fame.FloorDecrement(3);

				TimeManager.Delay(BeginFameSequence, 0.5f);
			}
		};

		SequenceDebugger.Setup(CurrentSequence);
	}
	public void Impress()
	{
		Interest.Value += Impressed ? 
			progressOnSuccessInPosse :
			progressOnSuccess;
	}
	public void Bore()
	{
		Interest.Value -= Impressed ? 
			penaltyOnFailureInPosse :
			penaltyOnFailure;
	}

	void BeginSequence()
	{
		if(pissedOffTimer.Running)
		{
			return;
		}

		Interest.OnEmpty = (v) => StormOff();

		Progress.Visibility = 1;

		if (!Blackout.IgnoreList.Contains(gameObject))
		{
			Blackout.IgnoreList.Add(gameObject);
		}
		GameState.BeginEncounter(gameObject);

		if (CurrentSequence != null)
		{
			CurrentSequence.Dispose();
			CurrentSequence = null;
		}


		float tboost = timeBoostPerMember;
		tboost *= ScoreManager.Cats.Count;

		CurrentSequence =
			SequenceManager.GetNewSequence
				(sequenceLengths.AccessByMagnitude(Progress.Magnitude),
				timeLimit: perSequenceTimeLimit + (isHuman? tboost :0),
				types: inputPool);

		CurrentSequence.OnSuccess = () =>
		{
			previousSequenceFailed = false;
			Impress();
			FindObjectOfType<SwarmMovement>().ImpressMembers();
			SequenceDebugger.VictoryAnim();

			Timer delay = TimeManager.GetNewTimer(0.25f,() =>
			{
				if (Progress.Magnitude < 1)
					BeginSequence();
				else if (isHuman)
					BeginFameSequence();
				else
					SequenceComplete();
			});
				delay.Run();
		};

		CurrentSequence.OnBadInput = () =>
		{
			SequenceDebugger.FailAnim();
			CurrentSequence.DelayedRetry();
		};

		CurrentSequence.OnTimeout = () =>
		{
			SequenceDebugger.TimeOutAnim();
			Bore();
			FindObjectOfType<SwarmMovement>().BoreMembers();
			if (CurrentSequence == null)
				return;

			CurrentSequence.Fail();
			TimeManager.Delay(time: 0.5f,
				onComplete: () =>
				{
					if (previousSequenceFailed)
					{
						previousSequenceFailed = false;
						PresentBailout();
						SequenceDebugger.End();
					}
					else
					{
						previousSequenceFailed = true;
						BeginSequence();
					}
				});

		};

		if (!GameState.InEncounter)
			GameState.BeginEncounter();

		SequenceDebugger.Setup(CurrentSequence);
	}

	void FameSequenceComplete()
	{
		GameState.EndFameEncounter();
	}

	void Join()
	{
		Impressed = true;
		GetComponent<Swarmer>().JoinCrowd();
		Interest.OnEmpty = (v) => Leave();
	}

	void StormOff()
	{
		if (!Impressed)
		{
			SequenceEnd();
		}
		else
		{
			Blackout.IgnoreList.Remove(gameObject);
			GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		}
		Player.AngrySound();

		Progress.Visibility = 0;
		Interest.OnEmpty = null;
		Interest.Value = startingInterest;
		Impressed = false;
		pissedOffTimer.End = 1;
		pissedOffTimer.Run();
	}

	void Leave()
	{
		StormOff();
		GetComponent<Swarmer>().LeaveCrowd();
		Blackout.IgnoreList.Remove(gameObject);
	}

	void PresentBailout()
	{
		GameState.BailoutPromptBegin();

		Timer tim = TimeManager.GetNewTimer(bailoutPromptWindow);

		tim.OnTick = (dt) =>
		{
			if (Input.GetButtonDown("Meow"))
			{
				GameState.BailoutPromptEnd();
				SequenceEnd();
				tim.Dispose();
			}
		};
		tim.OnComplete = () =>
		{
			GameState.BailoutPromptEnd();
			BeginSequence();
			tim.Dispose();
		};
		tim.Run();
	}

	void SequenceComplete()
	{
		SequenceEnd();
		Join();
	}

	void SequenceEnd()
	{
		pissedOffTimer.Run();
		Progress.Visibility = 0;
		GameState.EndEncounter();
		CurrentSequence.Dispose();
		CurrentSequence = null;
		SequenceDebugger.End();
	}

	void Start()
	{
		pissedOffTimer = TimeManager.GetNewTimer(angryDuration);

		PassiveDrain =
			TimeManager.GetNewTimer(
				loops: true,
				onTick: (dt) =>
				{
					if (Impressed && !GameState.InEncounter)
						Interest.Value -= interestBleedRate * dt;
				});
		PassiveDrain.Run();
	}

	void Update()
	{
		if (Debug.isDebugBuild)
			Progress.Visibility = 1;

		if (GameState.Paused)
			return;

		if (!Impressed &&
			!pissedOffTimer.Running &&
			CurrentSequence == null &&
			detectionArea.Contains(player) &&
			!GameState.InEncounter &&
			Input.GetButtonDown("Meow")
			)
		{
			BeginSequence();
		}
		this.ClampToBoundary();
	}

	#endregion
}