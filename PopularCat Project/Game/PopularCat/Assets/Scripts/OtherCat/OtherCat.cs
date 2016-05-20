using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OtherCat : MonoBehaviour
{
	#region Public Fields

	public float bailoutPromptWindow = 3;
	public float interestBleedRate = 2;
	public bool isHuman;
	public float penaltyOnFailure  = 20;
	public int perSequenceTimeLimit = 6;
	public float progressOnSuccess = 34;
	public int[] sequenceLengths;
	public float startingInterest  = 33;
	public float angryDuration = 3;
	public float fameRate = 10;

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

		CurrentSequence =
			SequenceManager.GetNewSequence
				(sequenceLengths.AccessByMagnitude(Progress.Magnitude),
				timeLimit: perSequenceTimeLimit);

		CurrentSequence.OnSuccess = () =>
		{
			GameState.Fame.Value += fameRate / 100;
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
			CurrentSequence.Fail();
			GameState.EndFameEncounter();
		};

		SequenceDebugger.Setup(CurrentSequence);
	}
	public void Impress()
	{
		Interest.Value += progressOnSuccess;
	}
	public void Bore()
	{
		Interest.Value -= penaltyOnFailure;
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

		CurrentSequence =
			SequenceManager.GetNewSequence
				(sequenceLengths.AccessByMagnitude(Progress.Magnitude),
				timeLimit: perSequenceTimeLimit);

		CurrentSequence.OnSuccess = () =>
		{
			previousSequenceFailed = false;
			Impress();
			FindObjectOfType<SwarmMovement>().ImpressMembers();
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

		CurrentSequence.OnTimeout = () =>
		{
			Bore();
			//FindObjectOfType<SwarmMovement>().BoreMembers(); Broken
			if (CurrentSequence == null)
				return;

			CurrentSequence.Fail();
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
			SequenceEnd();

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