using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages global game variables and states. 
/// </summary>
public class GameState : MonoBehaviour
{
	#region Public Fields

	public bool postponeTimeScore;
	public bool postponePosseScore;
	public Text scoreText;
	public Text timeText;
	public Text posseText;
	public Text endOfLevelText;
	public Button nextLevelButton;

	#endregion

	#region Private Fields
	static GameState instance;
	static Delayed<bool> inBailoutPrompt;
	static Timer levelTimer;

	#endregion

	#region Public Properties

	public static bool EndOfLevelPassed { get { return !levelTimer.Complete && Fame != 0; } }
	public static bool EndOfLevel { get; private set; }
	public static MonitoredValue Fame { get; set; }
	public static bool InBailoutPrompt { get { return inBailoutPrompt; } }
	public static bool InEncounter { get; private set; }
	public static bool InFameEncounter { get; private set; }
	public static ReadOnlyTimer LevelTimer { get { return levelTimer; } }
	public static bool Paused { get; private set; }
	public static StarSet[] Stars { get; private set; }
	public static GameObject Target { get; private set; }

	#endregion

	#region Public Methods

	public static void BailoutPromptBegin()
	{
		inBailoutPrompt.Value = true;
		Text tex = GameObject.Find("BailoutPrompt").GetComponent<Text>();
		var color = tex.color;
		color.a = 1;
		tex.color = color;
	}

	public static void BailoutPromptEnd()
	{
		inBailoutPrompt.Value = false;
		Text tex = GameObject.Find("BailoutPrompt").GetComponent<Text>();
		var color = tex.color;
		color.a = 0;
		tex.color = color;
	}

	public static void BeginEncounter(GameObject target = null)
	{
		if (InEncounter)
			return;

		Target = target;
		if (target != null)
		{
			PosseFormation.Instance.GetIntoPosition(target);
		}

		Blackout.Activate();
		InEncounter = true;
	}

	public static void BeginFameEncounter()
	{
		if (InFameEncounter)
			return;

		foreach (var s in Stars)
			s.Visible = true;
		InFameEncounter = true;
	}

	public static void BeginPause() { Paused = true; }

	public static void EndEncounter()
	{
		if (Target != null)
		{
			PosseFormation.Instance.ReleasePosition();
		}
		Target = null;

		foreach (var s in Stars)
			s.Visible = false;

		Blackout.Deactivate();
		InEncounter = false;
	}

	public static void EndFameEncounter()
	{
		EndLevel();
	}

	public static void EndLevel()
	{
		SequenceDebugger.End();

		levelTimer.Pause();

		foreach (var s in Stars)
		{
			s.Visible = false;
		}
		Fame.Floor(3);
		Fame.Lock();

		if (EndOfLevelPassed)
		{
			instance.Success();
		}
		else
		{
			instance.Failed();
		}

		FindObjectOfType<EndOfLevelPanel>().Visible = true;
		EndOfLevel = true;
	}

	void Failed()
	{
		endOfLevelText.text = "Failure";
		nextLevelButton.interactable = false;
		nextLevelButton.GetComponent<Image>().color = Color.grey;
	}
	void Success()
	{
		endOfLevelText.text = "Success";
	}

	public static void EndPause() { Paused = false; }

	#endregion

	#region Private Methods

	void Awake()
	{
		instance = this;
		inBailoutPrompt = new Delayed<bool>();
		ScoreManager.postponeTimeBonus =
			postponeTimeScore;
	}

	void FinalizeStateChange()
	{
		inBailoutPrompt.Confirm();
	}

	void Initialize()
	{
		Blackout.Deactivate();
		EndOfLevel = false;
		InEncounter = false;
		InFameEncounter = false;
		Paused = false;
	}

	void LateUpdate()
	{
		scoreText.text = string.Format("{0:000000}", ScoreManager.Score);
		posseText.text = 
			string.Format("{0:00}/{1:00}", 
				ScoreManager.Cats.Count, ScoreManager.Cats.CountUnfiltered);
		FinalizeStateChange();
	}

	void Start()
	{
		Initialize();
		ScoreManager.Initialize();

		levelTimer = TimeManager.GetNewTimer(5 * 60, EndLevel);

		levelTimer.OnTick = (dt) =>
		{
			timeText.text = levelTimer.F("{7}:{6:00}/{4}:{3:00}");
		};

		Stars = FindObjectsOfType<StarSet>();
		Fame = new MonitoredValue(
		onModified: (v) =>
		{
			foreach (var s in Stars)
			{
				s.Magnitude = v;
			}
		},
		onFull: (v) => EndLevel()
		);

		levelTimer.Run();
	}

	void Update()
	{
		if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F3))
		{
			levelTimer.Current = levelTimer.End - 5;
		}
	}

	#endregion
}