using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface Sequence
{
	#region Public Properties

	bool Complete { get; }
	List<SequenceItemDetails> Details { get; }
	bool Failed { get; }
	int Index { get; }
	int Length { get; }
	int Level { get; set; }

	/// <summary>
	/// The method called when a bad input is entered, defaults to a delayed reset 
	/// </summary>
	Action OnBadInput { get; set; }

	/// <summary>
	/// The method called when the sequence enters a failure state 
	/// </summary>
	Action OnFail { get; set; }

	/// <summary>
	/// The method called when the sequence enters a success state 
	/// </summary>
	Action OnSuccess { get; set; }

	/// <summary>
	/// The method called when the sequence times out 
	/// </summary>
	Action OnTimeout { get; set; }

	ReadOnlyTimer TimeLimit { get; }

	#endregion

	#region Public Methods

	void BoostTime(float amount);

	void DelayedRetry();

	void Dispose();

	void Fail();

	void Retry();

	void Succeed();

	#endregion
}

public interface SequenceItemDetails
{
	#region Public Properties

	bool Failed { get; }
	string Identifier { get; }
	Sprite Sprite { get; }
	float SpriteRotation { get; }
	bool Passed { get; }

	#endregion
}

public class SequenceManager : MonoBehaviour
{
	#region Public Fields

	public InputType[] axisInputs;

	public AudioClip badInputSound;

	public AudioClip goodInputSound;

	public AudioClip goodSequenceSound;

	public InputType[] keyInputs;

	#endregion

	#region Private Fields

	static AudioSource      audioSource;

	//public Button[] uiButtonInputs;
	static SequenceManager  instance;

	List<SequenceInput>     inputs;

	SafeList<SequenceClass> sequences;

	#endregion

	#region Public Methods

	public static Sequence GetNewSequence
		(int length = 4,
		Action onFail = null,
		Action onSuccess = null,
		Action onTimeout = null,
		Action onGoodInput = null,
		Action onBadInput = null,
		float timeLimit = 0,
		float timeBoost = 0.0f,
		float resetDelay = 0.25f,
		int level = 0)
	{
		var result = new SequenceClass(
			length,
			onFail,
			onSuccess,
			onTimeout,
			onGoodInput,
			onBadInput,
			timeLimit,
			timeBoost,
			resetDelay,
			level);

		instance.sequences.Add(result);
		return result;
	}

	#endregion

	#region Private Methods

	static SequenceInput RandomItem(int level)
	{
		SequenceInput result;
		do
		{
			result = instance.inputs.RandomItem();
		} while (result.Level > level);
		return result;
	}

	void Awake()
	{
		instance = this;
		sequences = new SafeList<SequenceClass>();
		inputs = new List<SequenceInput>();
		foreach (var s in keyInputs)
		{
			string t = s.identifier;
			inputs.Add(new SequenceInput(() => Input.GetButton(t), s));
		}
		foreach (var s in axisInputs)
		{
			var rev = s.Reverse();
			string t = s.identifier;
			inputs.Add(new SequenceInput(() => Input.GetAxisRaw(t) > 0.3f, s, "+"));
			inputs.Add(new SequenceInput(() => Input.GetAxisRaw(t) < -0.3f, rev, "-"));
		}

		inputs.Sort();
		//foreach (var button in uiButtonInputs)
		//{
		//	var bb = button.GetComponent<ButtonBool>();
		//	if (bb == null)
		//		bb = button.gameObject.AddComponent<ButtonBool>();
		//	Func<bool> f = () => bb.IsPressed;
		//	inputs.Add(new SequenceInput(f, button.GetComponentInChildren<Text>().text, 0));
		//}

		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (GameState.Paused || GameState.EndOfLevel)
			return;

		List<string> dupTrap = new List<string>();

		if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F2))
			foreach (var s in sequences)
			{
				s.Succeed();
			}
		else
			// Check inputs against running sequences
			foreach (var i in inputs)
			{
				if(dupTrap.Contains(i.Identifier))
				{
					continue;
				}
				dupTrap.Add(i.Identifier);
				bool test = i.Check();

				if (test)
				{
					foreach (var s in sequences)
						s.EnterInput(i.Identifier);
				}
			}
	}

	#endregion

	#region Public Classes

	[Serializable]
	public class InputType
	{
		#region Public Fields

		public string identifier;
		public int level;
		public Sprite sprite;
		public Sprite sprite2;
		public float spriteRotation;

		public InputType Reverse()
		{
			InputType result = new InputType();
			result.identifier = identifier;
			result.level = level;
			if (sprite2 != null)
				result.sprite = sprite2;
			else
			{
				result.spriteRotation = spriteRotation + 180;
				result.sprite = sprite;
			}
			return result;
		}

		#endregion
	}

	#endregion

	#region Private Classes

	class SequenceClass : Sequence
	{
		#region Private Fields

		Timer resetTimer;
		Timer time;

		#endregion

		#region Public Properties

		public bool Complete { get { return Index == Items.Count; } }
		public List<SequenceItemDetails> Details { get; private set; }
		public bool Failed { get; set; }
		public int Index { get; set; }
		public int Length { get { return Items.Count; } }
		public int Level { get; set; }
		public Action OnBadInput { get; set; }
		public Action OnFail { get; set; }
		public Action OnGoodInput { get; set; }
		public Action OnSuccess { get; set; }
		public Action OnTimeout { get; set; }
		public float TimeBoost { get; set; }
		public ReadOnlyTimer TimeLimit { get; private set; }

		#endregion

		#region Private Properties

		List<SequenceItem> Items { get; set; }

		#endregion

		#region Public Constructors

		public SequenceClass
			(int length,
			Action onFail,
			Action onSuccess,
			Action onTimeout,
			Action onGoodInput,
			Action onBadInput,
			float timeLimit,
			float timeBoost,
			float resetDelay,
			int level)
		{
			TimeBoost = timeBoost;
			Items = new List<SequenceItem>(length);
			Details = new List<SequenceItemDetails>(length);
			Level = level;

			for (int i = 0; i < length; i++)
			{
				var toAdd = new SequenceItem { input = RandomItem(Level) };
				Items.Add(toAdd);
				Details.Add(toAdd);
			}
			OnFail = onFail;
			OnSuccess = onSuccess;
			OnTimeout = onTimeout;
			OnGoodInput = onGoodInput ?? (() => BoostTime(TimeBoost));
			OnBadInput = onBadInput ?? DelayedRetry;

			if (timeLimit > 0)
			{
				time = TimeManager.GetNewTimer(timeLimit, () =>
				{
					if (OnTimeout != null)
						OnTimeout();
				});
				TimeLimit = time;
				time.Run();
			}

			resetTimer = TimeManager.GetNewTimer(resetDelay, Retry);
		}

		#endregion

		#region Public Methods

		public void BoostTime(float amount)
		{
			if (time != null)
				time.Current -= amount;
		}

		public void DelayedRetry()
		{
			resetTimer.Run();
		}

		public void Dispose()
		{
			instance.sequences.Remove(this);
			if (time != null)
				time.Dispose();

			if (resetTimer != null)
				resetTimer.Dispose();
		}

		public void EnterInput(string si)
		{
			if (Complete || Failed || resetTimer.Running)
			{
				return;
			}
			if (si == Items[Index].input.Identifier)
			{
				GoodInput();
				return;
			}
			BadInput();
		}

		public void Fail()
		{
			Failed = true;

			ScoreManager.Sequences.Miss();

			if (OnFail != null)
				OnFail();
		}

		public void Retry()
		{
			Index = 0;
			Failed = false;
			foreach (var i in Items)
			{
				i.Failed = false;
				i.Passed = false;
			}
		}

		public void Succeed()
		{
			time.Pause();
			if (instance.goodSequenceSound != null)
			{
				audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				audioSource.PlayOneShot(instance.goodSequenceSound);
			}

			ScoreManager.Sequences.Hit();
			if (OnSuccess != null)
				OnSuccess();
		}

		#endregion

		#region Private Methods

		private void BadInput()
		{
			Items[Index].Failed = true;
			Index++;

			if (instance.badInputSound != null)
			{
				audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				audioSource.PlayOneShot(instance.badInputSound);
			}

			ScoreManager.Inputs.Miss();

			if (OnBadInput != null)
				OnBadInput();
		}

		private void GoodInput()
		{
			Items[Index].Passed = true;
			Index++;

			if (instance.goodInputSound != null)
			{
				audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				audioSource.PlayOneShot(instance.goodInputSound);
			}

			if (OnGoodInput != null)
				OnGoodInput();

			ScoreManager.Inputs.Hit();

			if (Complete)
			{
				Succeed();
			}
		}

		#endregion

		#region Private Classes

		class SequenceItem : SequenceItemDetails
		{
			#region Public Fields

			public SequenceInput input;

			#endregion

			#region Public Properties

			public bool Failed { get; set; }
			public string Identifier { get { return input.Identifier; } }
			public Sprite Sprite { get { return input.Sprite; } }
			public float SpriteRotation { get { return input.SpriteRotation; } }
			public bool Passed { get; set; }

			#endregion
		}

		#endregion
	}

	class SequenceInput : IComparable<SequenceInput>
	{
		#region Public Properties

		public string Identifier { get; private set; }
		public int Level { get; private set; }
		public Sprite Sprite { get; private set; }
		public float SpriteRotation { get; private set; }
		public bool PreviousState { get; private set; }

		#endregion

		#region Private Properties

		Func<bool> Trigger { get; set; }

		#endregion

		#region Public Constructors

		public SequenceInput(Func<bool> trigger, InputType details, string add = "")
		{
			Trigger = trigger;
			Identifier = add + details.identifier;
			Level = details.level;
			Sprite = details.sprite;
			SpriteRotation = details.spriteRotation;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Checks if the trigger has been activated. 
		/// <para/>
		/// Subsequent checks will be false until the trigger is recorded as being deactivated. 
		/// </summary>
		/// <returns></returns>
		public bool Check()
		{
			bool sample = Trigger();
			bool test = sample && !PreviousState;
			PreviousState = sample;
			return test;
		}

		public int CompareTo(SequenceInput other)
		{
			return Level - other.Level;
		}

		#endregion
	}

	#endregion
}