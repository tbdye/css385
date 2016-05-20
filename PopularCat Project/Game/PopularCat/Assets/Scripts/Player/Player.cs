using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public AudioClip[] happySounds;
	public AudioClip[] angrySounds;

	enum CatState
	{
		Still,
		Walking,
		Running
	}

	#region Class Public Instance Variables
	public Sprite mCatJumpLeap;
	public Sprite mCatJumpAir;
	public Sprite mCatJumpLand;
	public Sprite mCatSitLookAway;
	public Sprite mCatSitLookForward;
	public Sprite mCatSitLookToward;
	public Sprite mCatStand;
	public Sprite mCatWalk;

	public float mMinTimeUntilCatSits;  // in seconds
	public float mMaxTimeUntilCatSits;  // in seconds
	public float mMinTimeInSitPosition; // in seconds
	public float mMaxTimeInSitPosition; // in seconds

	public float mMoveSpeed;
	public float mDeadzoneX;
	public float mDeadzoneY;
	public float mGroundDeadzone;

	public float mMaxJumpTime;         // in seconds
	public float mJumpImpulseAddRate;
	#endregion

	#region Class Private Instance Variables
	CatState mCatState;
	SpriteRenderer mSprite;
	

	float mTimeUntilCatSits;            // in seconds
	float mTimeInSitPosition;           // in seconds
	float mTimeSinceLastMovement;       // in seconds
	float mTimeSinceLastSitChange;      // in seconds
	bool mCatSitting;
	bool mResetSitTimes;
	#endregion
	

	/// <summary>
	/// Start
	/// Use this for initialization
	/// </summary>
	void Start ()
	{
		Blackout.IgnoreList.Add(gameObject);

		mCatState = CatState.Still;
		mSprite = GetComponent<SpriteRenderer>();
		
		mTimeUntilCatSits = Random.Range(mMinTimeUntilCatSits, mMaxTimeUntilCatSits);
		mTimeInSitPosition = Random.Range(mMinTimeInSitPosition, mMaxTimeInSitPosition);
	}

	/// <summary>
	/// Update
	/// Update is called once per frame
	/// </summary>
	void Update ()
	{
		var aud = GetComponent<AudioSource>();

		if(Input.GetButtonDown("Meow"))
		{
			aud.pitch = Random.Range(0.9f, 1.1f);
			if(GameState.InBailoutPrompt)
				aud.PlayOneShot(angrySounds.RandomItem());
			else
				aud.PlayOneShot(happySounds.RandomItem());
		}


		MoveAround();
		UpdateSprite();
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

		{
			EvaluateState(inputX, inputY);

			if (mCatState != CatState.Still)
			{
				// flip sprite to match player direction
				SpriteDirection(inputX);

				// move the player
				var position = new Vector3(moveX, moveY, 0f);
				GetComponent<Rigidbody2D>().velocity = position;
			}
		}
	}

	/// <summary>
	/// EvaluateState
	/// </summary>
	/// <param name="rawX"></param>
	/// <param name="rawY"></param>
	private void EvaluateState(float rawX, float rawY)
	{
		float inputX = Mathf.Abs(rawX);
		float inputY = Mathf.Abs(rawY);

		if (inputX > mDeadzoneX || inputY > mDeadzoneY)
		{
			mTimeSinceLastMovement = Time.realtimeSinceStartup;
			mCatSitting = false;

			ResetSitTimers();

			if (inputX > 0.6f || inputY > 0.6f)
			{
				mCatState = CatState.Running;
			}
			else
			{
				mCatState = CatState.Walking;
			}
		}
		else
		{
			mCatState = CatState.Still;
		}

	}

	private void ResetSitTimers()
	{
		if (mResetSitTimes)
		{
			// regenerate random values for next time
			mTimeUntilCatSits = Random.Range(mMinTimeUntilCatSits, mMaxTimeUntilCatSits);
			mTimeInSitPosition = Random.Range(mMinTimeInSitPosition, mMaxTimeInSitPosition);

			mResetSitTimes = false;
		}
	}

	/// <summary>
	/// SpriteDirection
	/// </summary>
	/// <param name="moveDir"></param>
	private void SpriteDirection(float moveDir)
	{
		mSprite.flipX &= moveDir <= 0;

		mSprite.flipX |= moveDir < 0;
	}

	/// <summary>
	/// UpdateSprite
	/// </summary>
	private void UpdateSprite()
	{
		switch (mCatState)
		{
			case CatState.Running:
				SetWalkingSprite();
				break;
			case CatState.Still:
				SetStillSprite();
				break;
			case CatState.Walking:
				SetWalkingSprite();
				break;
		}
	}
	
	/// <summary>
	/// SetStillSprite
	/// </summary>
	private void SetStillSprite()
	{
		if ((Time.realtimeSinceStartup - mTimeSinceLastMovement) < mTimeUntilCatSits)
		{
			mSprite.sprite = mCatStand;
			mTimeSinceLastSitChange = Time.realtimeSinceStartup;
		}
		else
		{
			ChooseSitSprite();
		}
	}

	/// <summary>
	/// ChooseSitSprite
	/// </summary>
	private void ChooseSitSprite()
	{
		if (!mCatSitting || (Time.realtimeSinceStartup - mTimeSinceLastSitChange) > mTimeInSitPosition)
		{
			mResetSitTimes = true;
			
			mCatSitting = true;
			mTimeSinceLastSitChange = Time.realtimeSinceStartup;
			mTimeInSitPosition = Random.Range(mMinTimeInSitPosition, mMaxTimeInSitPosition);

			int choice = Random.Range(0, 3); // will get results 0 - 2
			switch (choice)
			{
				case 0:
					mSprite.sprite = mCatSitLookAway;
					break;
				case 1:
					mSprite.sprite = mCatSitLookForward;
					break;
				case 2:
					mSprite.sprite = mCatSitLookToward;
					break;
			}
		}
	}

	/// <summary>
	/// SetWalkingSprite
	/// </summary>
	private void SetWalkingSprite()
	{
		mSprite.sprite = mCatWalk;
	}
}