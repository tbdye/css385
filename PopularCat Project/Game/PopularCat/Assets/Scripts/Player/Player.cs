using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    #region Class Public Instance Variables
    public AudioClip[] happySounds;
    public AudioClip[] angrySounds;
    public float mMoveSpeed;
	#endregion

	/// <summary>
	/// Start
	/// Use this for initialization
	/// </summary>
	void Start ()
	{
		Blackout.IgnoreList.Add(gameObject);
	}

	/// <summary>
	/// Update
	/// Update is called once per frame
	/// </summary>
	void Update ()
	{
		var aud = GetComponent<AudioSource>();

		if(Input.GetButtonDown("Meow") && !aud.isPlaying)
		{
			aud.pitch = Random.Range(0.9f, 1.1f);
			if(GameState.InBailoutPrompt)
				aud.PlayOneShot(angrySounds.RandomItem());
			else
				aud.PlayOneShot(happySounds.RandomItem());
		}

		MoveAround();
		Boundary.Clamp(gameObject);
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
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");

		float moveX = inputX * mMoveSpeed;
		float moveY = inputY * mMoveSpeed;

		// set walk speed for keyboard users
		if (Input.GetKey(KeyCode.RightControl))
		{
			moveX /= 2;
			moveY /= 2;
		}

        var position = new Vector3(moveX, moveY, 0f);
        GetComponent<Rigidbody2D>().velocity = position;
	}
}