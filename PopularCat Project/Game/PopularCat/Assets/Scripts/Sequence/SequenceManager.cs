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
	void Succeed();
	void Retry();

	#endregion
}

public interface SequenceItemDetails
{
	#region Public Properties

	bool Failed { get; }
	string Identifier { get; }
	bool Passed { get; }

	#endregion
}

public class SequenceManager : MonoBehaviour
{
	#region Public Fields

	public AudioClip badInputSound;
	public AudioClip goodInputSound;
	public AudioClip goodSequenceSound;
	public string[] axisInputs;
	public string[] keyInputs;
	public Button[] uiButtonInputs;

	#endregion

	#region Private Fields

	static AudioSource audioSource;
	static SequenceManager    instance;
	LinkedList<SequenceClass> AddBuffer = new LinkedList<SequenceClass>();
	List<SequenceInput>       Inputs    = new List<SequenceInput>();
	LinkedList<SequenceClass> Removing  = new LinkedList<SequenceClass>();
	LinkedList<SequenceClass> Sequences = new LinkedList<SequenceClass>();

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
		float timeBoost = 0.0f)
	{
		var result = new SequenceClass(
			length, 
			onFail,
			onSuccess,
			onTimeout,
			onGoodInput,
			onBadInput,
			timeLimit,
			timeBoost);
		instance.AddBuffer.AddLast(result);
		return result;
	}

	#endregion

	#region Private Methods

	static SequenceInput RandomItem()
	{
		return instance.Inputs.RandomItem();
	}

	void Awake()
	{
		instance = this;
		foreach (var s in keyInputs)
		{
			string t = s;
			Inputs.Add(new SequenceInput(() => Input.GetButton(t), t));
		}
		foreach (var s in axisInputs)
		{
			string t = s;
			Inputs.Add(new SequenceInput(() => Input.GetAxisRaw(t) > 0.3f, "+" + t));
			Inputs.Add(new SequenceInput(() => Input.GetAxisRaw(t) < -0.3f, "-" + t));
		}
		foreach (var button in uiButtonInputs)
		{
			var bb = button.GetComponent<ButtonBool>();
			if (bb == null)
				bb = button.gameObject.AddComponent<ButtonBool>();
			Func<bool> f = () => bb.IsPressed;
			Inputs.Add(new SequenceInput(f, button.GetComponentInChildren<Text>().text));
		}

		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (GameState.Paused || GameState.EndOfLevel)
			return;

		// Add new sequences in the buffer
		foreach (var i in AddBuffer)
		{
			Sequences.AddLast(i);
		}
		AddBuffer.Clear();

		if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F2))
			foreach (var s in Sequences)
			{
				s.Succeed();
			}
		else
			// Check inputs against running sequences
			foreach (var i in Inputs)
			{
				bool test = i.Check();

				if (test)
				{
					foreach (var s in Sequences)
						s.EnterInput(i);
				}
			}

		// Remove disposed sequences
		foreach (var s in Removing)
		{
			Sequences.Remove(s);
		}
		Removing.Clear();
	}

	#endregion

	#region Private Classes

	class SequenceClass : Sequence
	{
		#region Private Fields

		Timer resetDelay;
		Timer time;
		#endregion

		#region Public Properties

		public float TimeBoost { get; set; }
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
			float timeBoost)
		{
			TimeBoost = timeBoost;
			Items = new List<SequenceItem>(length);
			Details = new List<SequenceItemDetails>(length);
			for (int i = 0; i < length; i++)
			{
				var toAdd = new SequenceItem { input = RandomItem() };
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

			resetDelay = TimeManager.GetNewTimer(1, Retry);
		}

		#endregion

		#region Public Methods

		public void BoostTime(float amount)
		{
			if(time != null)
				time.Current -= amount;
		}

		public void DelayedRetry()
		{
			resetDelay.Run();
		}

		public void Dispose()
		{
			instance.Removing.AddLast(this);
			if (time != null)
				time.Dispose();

			if (resetDelay != null)
				resetDelay.Dispose();
		}

		public void EnterInput(SequenceInput si)
		{
			if (Complete || Failed || resetDelay.Running)
			{
				return;
			}
			if (si == Items[Index].input)
			{
				GoodInput();
				return;
			}
			BadInput();
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
				audioSource.pitch = UnityEngine.Random.Range(0.9f,1.1f);
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

			#endregion
		}

		#endregion
	}

	class SequenceInput
	{
		#region Public Properties

		public string Identifier { get; private set; }
		public bool PreviousState { get; private set; }

		#endregion

		#region Private Properties

		Func<bool> Trigger { get; set; }

		#endregion

		#region Public Constructors

		public SequenceInput(Func<bool> trigger, string id)
		{
			Trigger = trigger;
			Identifier = id;
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