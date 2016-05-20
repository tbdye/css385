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

public static class ScoreManager
{
	#region Private Fields

	static ScoreDetailsClass inputs;
	static ScoreDetailsClass sequences;

	#endregion

	#region Public Properties

	public static ScoreDetails Inputs { get { return inputs; } }
	public static ScoreDetails Sequences { get { return sequences; } }

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
		inputs = new ScoreDetailsClass();
		sequences = new ScoreDetailsClass();
	}

	public static string ScoreString
	{
		get
		{
			string result = string.Format("Inputs: {0}/{1}\n", Inputs.Hits, Inputs.Total);
			result += string.Format("Accuracy: {0:P}\n", Inputs.Accuracy);
			result += string.Format("Sequences: {0}/{1}",Sequences.Hits, Sequences.Total);

			return result;
		}
	}

	public static int ScoreCalculation()
	{
		//TODO: implement score calculations

		return 0;
	}

	#endregion

	#region Private Classes

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