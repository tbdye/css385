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
		if (GameState.InEncounter && GameState.Target == gameObject)
		{
			if (GameState.InFameEncounter)
			{
				render.sprite = mFilming;
			}
			else
			{
				render.sprite = mAttention;
			}
		}
		else
		{
			render.sprite = mIdle;
		}
	}

	#endregion
}