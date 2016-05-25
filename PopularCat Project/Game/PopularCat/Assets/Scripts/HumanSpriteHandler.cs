using UnityEngine;
using System.Collections;

public class HumanSpriteHandler : MonoBehaviour {

    #region Public Fields
    public Sprite mIdle;
    public Sprite mAttention;
    public Sprite mFilming;
    #endregion

    #region Private Fields
    SpriteRenderer render;
    #endregion

    void Start ()
    {
        render.sprite = mIdle;
	}
	
	void Update ()
    {
	    if (GameState.InEncounter)
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
}