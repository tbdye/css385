using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	#region Class Public Instance Variables
	public AudioClip[] happySounds;
	public AudioClip[] angrySounds;
	public float mMoveSpeed;
	#endregion

	AudioSource meow;
	static Player instance;
	/// <summary>
	/// Start
	/// Use this for initialization
	/// </summary>
	void Start ()
	{
		instance = this;
		meow = GetComponent<AudioSource>();
		Blackout.IgnoreList.Add(gameObject);
	}

	/// <summary>
	/// Update
	/// Update is called once per frame
	/// </summary>
	void Update ()
	{

		if (Input.GetButtonDown("Meow") && !meow.isPlaying)
		{
			meow.pitch = Random.Range(0.9f, 1.1f);
			if(GameState.InBailoutPrompt)
				meow.PlayOneShot(angrySounds.RandomItem());
			else
				meow.PlayOneShot(happySounds.RandomItem());
		}

		MoveAround();
		Boundary.Clamp(gameObject);
	}


	static public void AngrySound()
	{
		if (!instance.meow.isPlaying)
		{
		instance.meow.pitch = Random.Range(0.9f, 1.1f);
		instance.meow.PlayOneShot(instance.angrySounds.RandomItem());
		}
	}

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
}