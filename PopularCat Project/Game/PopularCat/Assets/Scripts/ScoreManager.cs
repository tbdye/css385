using System;
using System.Collections;
using UnityEngine;

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

public interface ObjectCounter
{
	int Count { get; }
	int CountUnfiltered { get; }
}

public static class ScoreManager
{
	static ObjectCounter<Swarmer> othercats;
	#region Public Properties
	public static ObjectCounter Cats { get { return othercats; } }
	public static ScoreDetails Inputs { get; private set; }
	public static ScoreDetails Sequences { get; private set; }

	public static int Score
	{
		get
		{
			return ScoreCalculation();
		}
	}

	#endregion

	#region Public Methods

	public static void Initialize()
	{
		Inputs = new ScoreDetailsClass();
		Sequences = new ScoreDetailsClass();
		othercats = new ObjectCounter<Swarmer> { filter = (c) => c.InSwarm };
	}

	public static string ScoreString
	{
		get
		{
			string result = string.Format("Inputs: {0}/{1}\n", Inputs.Hits, Inputs.Total);
			result += string.Format("Longest streak: {0}\n", Inputs.HighestConsecutive);
			result += string.Format("Accuracy: {0:P}\n", Inputs.Accuracy);
			result += string.Format("Sequences: {0}/{1}\n",Sequences.Hits, Sequences.Total);
			result += string.Format("Cats: {0}/{1}", Cats.Count, Cats.CountUnfiltered);
			return result;
		}
	}

	public static int ScoreCalculation()
	{
		int inputValue = 10;
		int inputScore = inputValue * Inputs.Hits;
		int sequenceValue = 40;
		int sequenceScore = sequenceValue * Sequences.Hits;

		int timeScore = (int)GameState.LevelTimer.Remaining * 5;

		int inputStreakScore = Inputs.HighestConsecutive * inputValue;
		int SequenceStreakScore = Sequences.HighestConsecutive * inputValue;

		return inputScore + sequenceScore + timeScore + inputStreakScore + SequenceStreakScore;
	}

	#endregion

	#region Private Classes

	class ObjectCounter<T> : ObjectCounter where T:MonoBehaviour
	{
		public Func<T,bool> filter;
		public int Count
		{
			get
			{
				var pool = UnityEngine.Object.FindObjectsOfType<T>();
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
				var pool = UnityEngine.Object.FindObjectsOfType<T>();
				return pool.Length;
			}
		}
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

		public int Total { get { return Hits + Misses; } }
		public int CurrentConsecutive { get; set; }
		public int HighestConsecutive { get; set; }
		public int Hits { get; set; }
		public int Misses { get; set; }

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