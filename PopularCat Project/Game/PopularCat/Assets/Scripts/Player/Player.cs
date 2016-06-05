using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region Public Fields

	public AudioClip[] angrySounds;
	public AudioClip[] happySounds;
	public float mMoveSpeed;

	#endregion

	#region Private Fields

	AudioSource meow;

	#endregion

	#region Public Properties

	public static Player Instance { get; private set; }

	#endregion

	#region Public Methods

	static public void AngrySound()
	{
		if (!Instance.meow.isPlaying)
		{
			Instance.meow.pitch = Random.Range(0.9f, 1.1f);
			Instance.meow.PlayOneShot(Instance.angrySounds.RandomItem());
		}
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// MoveAround 
	/// </summary>
	void MoveAround()
	{
		if (GameState.Paused || GameState.InEncounter || GameState.EndOfLevel)
		{
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			return;
		}

		// get controller/arrow input for movement
		var input = Utils.InputVector;

		input *= mMoveSpeed;

		// set walk speed for keyboard users
		if (Input.GetKey(KeyCode.RightControl))
			input /= 2;

		var position = new Vector3(input.x, input.y, 0f);
		GetComponent<Rigidbody2D>().velocity = position;
	}

	/// <summary>
	/// Start Use this for initialization 
	/// </summary>
	void Start()
	{
		Instance = this;
		meow = GetComponent<AudioSource>();
		Blackout.IgnoreList.Add(gameObject);
	}

	/// <summary>
	/// Update Update is called once per frame 
	/// </summary>
	void Update()
	{
		if (Input.GetButtonDown("Meow") && !meow.isPlaying)
		{
			meow.pitch = Random.Range(0.9f, 1.1f);
			if (GameState.InBailoutPrompt)
				meow.PlayOneShot(angrySounds.RandomItem());
			else
				meow.PlayOneShot(happySounds.RandomItem());
		}

		MoveAround();
		Boundary.Clamp(gameObject);
	}

	#endregion
}