using System;
using System.Collections;
using UnityEngine;

public interface ObjectCounter
{
	#region Public Properties

	int Count { get; }
	int CountUnfiltered { get; }

	#endregion
}

public interface ScoreDetails
{
	#region Public Properties

	float Accuracy { get; }
	int CurrentConsecutive { get; }
	int HighestConsecutive { get; }
	int Hits { get; }
	int Misses { get; }
	int Total { get; }

	#endregion

	#region Public Methods

	void Hit();

	void Miss();

	#endregion
}

public class ScoreManager : MonoBehaviour
{
	#region Public Fields

	public static bool postponePosseBonus;
	public static bool postponeTimeBonus;
	public int catValue = 20;
	public int inputValue = 10;
	public int sequenceValue = 40;
	public int streakMultiplier = 1;
	public int timeMultiplier = 5;

	#endregion

	#region Private Fields

	static ScoreManager instance;
	static ObjectCounter<Swarmer> othercats;

	#endregion

	#region Public Properties

	public static ObjectCounter Cats { get { return othercats; } }
	public static ScoreDetails Inputs { get; private set; }
	public static int Score { get { return ScoreCalculation(); } }

	public static string ScoreString
	{
		get
		{
			string result = string.Format("Inputs: {0}/{1}\n", Inputs.Hits, Inputs.Total);
			result += string.Format("Longest input streak: {0}\n", Inputs.HighestConsecutive);
			result += string.Format("Input Accuracy: {0:P}\n", Inputs.Accuracy);
			result += string.Format("Sequences: {0}/{1}\n", Sequences.Hits, Sequences.Total);
			result += string.Format("Sequence Completion: {0:P}\n", Sequences.Accuracy);
			result += string.Format("Longest sequence streak: {0}\n", Sequences.HighestConsecutive);
			result += string.Format("Cats: {0}/{1}", Cats.Count, Cats.CountUnfiltered);
			return result;
		}
	}

	public static ScoreDetails Sequences { get; private set; }

	#endregion

	#region Public Methods

	public static void Initialize()
	{
		Inputs = new ScoreDetailsClass();
		Sequences = new ScoreDetailsClass();
		othercats = new ObjectCounter<Swarmer> { filter = (c) => c.InSwarm };
	}

	public static int ScoreCalculation()
	{
		int inputScore = instance.inputValue * Inputs.Hits;
		int sequenceScore = instance.sequenceValue * Sequences.Hits;

		int timeScore = 0;
		if (!postponeTimeBonus || GameState.EndOfLevel)
			timeScore = (int)GameState.LevelTimer.Remaining * instance.timeMultiplier;

		int posseScore = 0;
		if (!postponePosseBonus || GameState.EndOfLevel)
			posseScore = Cats.Count * instance.catValue;

		int inputStreakScore = Inputs.HighestConsecutive * instance.inputValue * instance.streakMultiplier;
		int SequenceStreakScore = Sequences.HighestConsecutive * instance.inputValue * instance.streakMultiplier;

		return
			inputScore + sequenceScore + timeScore +
			inputStreakScore + SequenceStreakScore + posseScore;
	}

	#endregion

	#region Private Methods

	void Awake()
	{
		instance = this;
	}

	#endregion

	#region Private Classes

	class ObjectCounter<T> : ObjectCounter where T : MonoBehaviour
	{
		#region Public Fields

		public Func<T,bool> filter;

		#endregion

		#region Public Properties

		public int Count
		{
			get
			{
				var pool = FindObjectsOfType<T>();
				int i = 0;
				if (filter != null)
				{
					foreach (var t in pool)
					{
						if (filter(t))
							i++;
					}
				}
				else
					i = pool.Length;

				return i;
			}
		}

		public int CountUnfiltered
		{
			get
			{
				var pool = FindObjectsOfType<T>();
				return pool.Length;
			}
		}

		#endregion
	}

	class ScoreDetailsClass : ScoreDetails
	{
		#region Public Properties

		public float Accuracy
		{
			get
			{
				if (Hits + Misses == 0)
				{
					return 0;
				}
				return (float)Hits / (Hits + Misses);
			}
		}

		public int CurrentConsecutive { get; set; }
		public int HighestConsecutive { get; set; }
		public int Hits { get; set; }
		public int Misses { get; set; }
		public int Total { get { return Hits + Misses; } }

		#endregion

		#region Public Methods

		public void Hit()
		{
			CurrentConsecutive++;
			if (CurrentConsecutive > HighestConsecutive)
				HighestConsecutive = CurrentConsecutive;
			Hits++;
		}

		public void Miss()
		{
			CurrentConsecutive = 0;
			Misses++;
		}

		#endregion
	}

	#endregion
}