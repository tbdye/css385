﻿using System;
using System.Collections.Generic;
using UnityEngine;

public interface ReadOnlyTimer
{
	#region Public Properties

	bool Complete { get; }
	float Completion { get; }
	float Current { get; }
	float End { get; }
	bool Loop { get; }
	float Remaining { get; }
	float Remaining01 { get; }
	bool Running { get; }

	#endregion
}

public interface Timer : ReadOnlyTimer
{
	#region Public Properties

	new float Current { get; set; }
	new float End { get; set; }
	new bool Loop { get; set; }

	/// <summary>
	/// Method called when the timer reaches its end. 
	/// </summary>
	Action OnComplete { get; set; }

	/// <summary>
	/// Method called when the timer advances. 
	/// <para/>
	/// Passes delta time as a parameter. 
	/// </summary>
	Action<float> OnTick { get; set; }

	/// <summary>
	/// A set of methods which are associated with points in time. 
	/// <para/>
	/// A method is called when the timer advances past the associated point in time. 
	/// </summary>
	Dictionary<float, Action> OnTime { get; }

	#endregion

	#region Public Methods

	void Dispose();

	void Pause();

	void Resume();

	void Run();

	void Stop();

	#endregion
}

public class TimeManager : MonoBehaviour
{
	#region Private Fields

	static LinkedList<TimerClass> adding;
	static LinkedList<TimerClass> removing;
	static LinkedList<TimerClass> timers;

	#endregion

	#region Public Methods

	/// <summary>
	/// Creates a new timer.
	/// <para/>
	/// Do not call this method in Awake(), as there is no guarantee that the manager has been instantiated yet.
	/// </summary>
	/// <param name="onComplete"> Method called on completion of timer. </param>
	/// <param name="onTick"> 
	/// Method called on update tick. 
	/// <para/>
	/// Delta time is passed as a parameter. 
	/// </param>
	/// <param name="onTime"> 
	/// A Dictionary that maps methods to points in time.
	/// <para/>
	/// Each method is called as the timer passes the associated time.
	/// </param>
	/// <returns></returns>
	public static Timer GetNewTimer
		(float span = 1,
		Action onComplete = null,
		Action<float> onTick = null,
		Dictionary<float, Action> onTime = null,
		bool runWhilePaused = false,
		bool loops = false)
	{
		var result = new TimerClass(span, onComplete, onTick, onTime, runWhilePaused, loops);
		adding.AddLast(result);
		return result;
	}

	#endregion

	#region Private Methods

	void Awake()
	{
		timers = new LinkedList<TimerClass>();
		adding = new LinkedList<TimerClass>();
		removing = new LinkedList<TimerClass>();
	}

	void Update()
	{
		foreach (var t in adding)
			timers.AddLast(t);
		adding.Clear();

		foreach (var t in timers)
			t.Advance();

		foreach (var t in removing)
			timers.Remove(t);
		removing.Clear();
	}

	#endregion

	#region Private Classes

	class TimerClass : Timer
	{
		#region Public Properties

		public bool Complete { get { return Current >= End; } }
		public float Completion { get { return Mathf.Clamp01(Current / End); } }
		public float Current { get; set; }
		public float End { get; set; }
		public bool Loop { get; set; }
		public Action OnComplete { get; set; }
		public Action<float> OnTick { get; set; }
		public float Remaining { get { return End - Current; } }
		public float Remaining01 { get { return Remaining / End; } }
		public bool Running { get; private set; }
		public bool RunWhilePaused { get; set; }

		public Dictionary<float, Action> OnTime { get; private set; }

		#endregion
		#region Public Constructors

		public TimerClass
			(float span,
			Action onComplete,
			Action<float> onTick,
			Dictionary<float, Action> onTime,
			bool runWhilePaused,
			bool loops)
		{
			End = span;
			OnComplete = onComplete;
			OnTick = onTick;
			RunWhilePaused = runWhilePaused;
			Loop = loops;
			OnTime = onTime ?? new Dictionary<float, Action>();
		}

		#endregion
		#region Public Methods

		public void Advance()
		{
			if (!GameState.Paused || RunWhilePaused)
				if (Running && !Complete)
				{
					Current += Time.deltaTime;

					if (OnTick != null)
						OnTick(Time.deltaTime);

					foreach (var t in OnTime.Keys)
					{
						if (Current > t && Current - Time.deltaTime <= t)
						{
							if(OnTime[t] != null)
								OnTime[t]();
						}
					}

					if (Complete)
					{
						if (OnComplete != null)
							OnComplete();
						if (Loop)
							Current -= End;
						else
							Running = false;
					}
				}
		}

		public void Dispose()
		{
			Pause();
			removing.AddLast(this);
		}

		public void Pause() { Running = false; }

		public void Resume() { Running = true; }

		public void Run()
		{
			Current = 0;
			Running = true;
		}

		public void Stop()
		{
			Current = 0;
			Running = false;
		}

		#endregion
	}

	#endregion
}