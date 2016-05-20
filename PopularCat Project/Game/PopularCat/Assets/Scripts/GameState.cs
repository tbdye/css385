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

	public Text timeText;

	#endregion

	#region Private Fields

	static Timer levelTimer;

	#endregion

	#region Public Properties

	public static bool EndOfLevel { get; private set; }
	public static MonitoredValue Fame { get; set; }
	public static bool InBailoutPrompt { get; private set; }
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
		InBailoutPrompt = true;
		Text tex = GameObject.Find("BailoutPrompt").GetComponent<Text>();
		var color = tex.color;
		color.a = 1;
		tex.color = color;
	}
	public static void BailoutPromptEnd()
	{
		InBailoutPrompt = false;
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
			s.Floor();
		}

		FindObjectOfType<EndOfLevelPanel>().Visible = true;
		EndOfLevel = true;
	}

	public static void EndPause() { Paused = false; }

	#endregion

	#region Private Methods

	void Initialize()
	{
		Blackout.Deactivate();
		EndOfLevel = false;
		InEncounter = false;
		InFameEncounter = false;
		Paused = false;
	}

	void Start()
	{
		Initialize();
		ScoreManager.Initialize();

		levelTimer = TimeManager.GetNewTimer(5 * 60, EndLevel);

		levelTimer.OnTick = (dt) =>
		{
			var remain = TimeSpan.FromSeconds(levelTimer.Remaining);
			var total = TimeSpan.FromSeconds(levelTimer.End);
			string rString = string.Format("{0}:{1:00}", remain.Minutes, remain.Seconds);
			string tString = string.Format("{0}:{1:00}", total.Minutes, total.Seconds);

			timeText.text = rString + "/" + tString;
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
		if(Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F3))
		{
			levelTimer.Current = levelTimer.End - 5;
		}
	}

	#endregion
}