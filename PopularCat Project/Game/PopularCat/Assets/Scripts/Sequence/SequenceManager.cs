using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Flags]
public enum InputTypes
{
	NormalArrows = 1,
	InvertedArrows = 2,
	AlternateArrows = 4,
	InvertedAlternateArrows = 8,
	TutorialArrows = 16,
	TutorialAlternateArrows = 32,
	TutorialInvertedArrows = 64,
	TutorialAlternateInvertedArrows = 128
}

public interface Sequence
{
	#region Public Properties

	bool Complete { get; }
	List<SequenceItemDetails> Details { get; }
	bool Failed { get; }
	int Index { get; }
	int Length { get; }

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

	Timer TimeLimit { get; }
	InputTypes Types { get; set; }

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
	bool Passed { get; }
	Sprite Sprite { get; }
	float SpriteRotation { get; }

	#endregion
}

public class SequenceManager : MonoBehaviour
{
	#region Public Fields

	public InputType[] axisInputs;
	public AudioClip badInputSound;
	public AudioClip goodInputSound;
	public AudioClip goodSequenceSound;

	#endregion

	#region Private Fields

	static AudioSource      audioSource;

	//public InputType[] keyInputs;
	//public Button[] uiButtonInputs;
	static SequenceManager  instance;

	SafeList<SequenceClass> sequences;

	#endregion

	#region Private Properties

	static Dictionary<InputTypes, List<SequenceInput>> InputPools { get; set; }

	#endregion

	#region Public Methods

	//List<SequenceInput>     inputs;
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
		InputTypes types = InputTypes.NormalArrows)
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
			types);

		instance.sequences.Add(result);
		return result;
	}

	public static void StopSequences()
	{
		foreach (var s in instance.sequences)
		{
			s.TimeLimit.Pause();
		}
	}

	#endregion

	#region Private Methods

	static SequenceInput RandomItem(InputTypes types)
	{
		var flags = new List<InputTypes>(types.GetFlags());

		IEnumerable<SequenceInput> combinedPool = InputPools[flags[0]];
		for (int i = 1; i < flags.Count; i++)
			combinedPool = combinedPool.Concat(InputPools[flags[i]]);

		var l = new List<SequenceInput>(combinedPool);

		return l.RandomItem();
	}

	void Awake()
	{
		instance = this;
		sequences = new SafeList<SequenceClass>();
		InputPools = new Dictionary<InputTypes, List<SequenceInput>>();
		foreach (InputTypes e in Enum.GetValues(typeof(InputTypes)))
		{
			InputPools.Add(e, new List<SequenceInput>());
		}

		//foreach (var s in keyInputs)
		//{
		//	string t = s.identifier;
		//	inputs.Add(new SequenceInput(() => Input.GetButton(t), s));
		//}

		foreach (var s in axisInputs)
		{
			var rev = s.Reverse();
			string t = s.identifier;

			InputPools[s.type].Add(new SequenceInput(() => Input.GetAxisRaw(t) > 0.3f, s, "+"));
			InputPools[s.type].Add(new SequenceInput(() => Input.GetAxisRaw(t) < -0.3f, rev, "-"));
		}

		//foreach (var button in uiButtonInputs)
		//{
		//	var bb = button.GetComponent<ButtonBool>();
		//	if (bb == null)
		//		bb = button.gameObject.AddComponent<ButtonBool>();
		//	Func<bool> f = () => bb.IsPressed;
		//	inputs.Add(new SequenceInput(f, button.GetComponentInChildren<Text>().text, 0));
		//}
	}

	void Start()
	{
		var mm = GetComponent<MusicManager>();
		if (mm != null)
			audioSource = GetComponent<MusicManager>().SpareAudioSource;
		else
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	void Update()
	{
		if (GameState.Paused || GameState.EndOfLevel)
			return;

		var dupTrap = new List<string>();

		if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F2))
			foreach (var s in sequences)
			{
				s.Succeed();
			}
		else
			// Check inputs against running sequences
			foreach (var p in InputPools.Values)
			{
				foreach (var i in p)
				{
					if (dupTrap.Contains(i.Identifier))
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
	}

	#endregion

	#region Public Classes

	[Serializable]
	public class InputType
	{
		#region Public Fields

		public string identifier;
		public string label;
		public Sprite sprite;
		public Sprite sprite2;
		public float spriteRotation;
		public InputTypes type = InputTypes.NormalArrows;

		#endregion

		#region Public Methods

		public InputType Reverse()
		{
			InputType result = new InputType();
			result.identifier = identifier;
			result.type = type;
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
		public Action OnBadInput { get; set; }
		public Action OnFail { get; set; }
		public Action OnGoodInput { get; set; }
		public Action OnSuccess { get; set; }
		public Action OnTimeout { get; set; }
		public float TimeBoost { get; set; }
		public Timer TimeLimit { get; private set; }
		public InputTypes Types { get; set; }

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
			InputTypes level)
		{
			TimeBoost = timeBoost;
			Items = new List<SequenceItem>(length);
			Details = new List<SequenceItemDetails>(length);
			Types = level;

			for (int i = 0; i < length; i++)
			{
				var toAdd = new SequenceItem { input = RandomItem(Types) };
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
			if (TimeLimit != null && !TimeLimit.Running)
				return;
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
			public bool Passed { get; set; }
			public Sprite Sprite { get { return input.Sprite; } }
			public float SpriteRotation { get { return input.SpriteRotation; } }

			#endregion
		}

		#endregion
	}

	class SequenceInput
	{
		#region Public Properties

		public string Identifier { get; private set; }
		public bool PreviousState { get; private set; }
		public Sprite Sprite { get; private set; }
		public float SpriteRotation { get; private set; }
		public InputTypes Type { get; private set; }

		#endregion

		#region Private Properties

		Func<bool> Trigger { get; set; }

		#endregion

		#region Public Constructors

		public SequenceInput(Func<bool> trigger, InputType details, string add = "")
		{
			Trigger = trigger;
			Identifier = add + details.identifier;
			Type = details.type;
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

		#endregion
	}

	#endregion
}