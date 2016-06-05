using System.Collections;
using UnityEngine;

public class HumanSpriteHandler : MonoBehaviour
{
	#region Public Fields

	public Sprite mAttention;
	public Sprite mFilming;
	public Sprite mIdle;

	#endregion

	#region Private Fields

	SpriteRenderer render;

	#endregion

	#region Private Methods

	void Start()
	{
		render = GetComponent<SpriteRenderer>();
		render.sprite = mIdle;
	}

	void Update()
	{
		bool eol = GameState.EndOfLevel && !GameState.EndOfLevelPassed;

		if (GameState.InEncounter && GameState.Target == GetComponentInParent<OtherCat>().gameObject)
		{
			render.flipX = Player.Instance.transform.position.x > transform.position.x;

			if (GameState.InFameEncounter)
			{
				render.sprite = mFilming;
			}
			else
			{
				render.sprite = mAttention;
			}
			if(!eol)
			return;
		}
		
			render.sprite = mIdle;
	}

	#endregion
}