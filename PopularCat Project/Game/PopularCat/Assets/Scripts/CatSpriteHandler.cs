using System.Collections;
using UnityEngine;

public class CatSpriteHandler : MonoBehaviour
{
    #region Public Fields

    public Vector2 timeInSitPosition;
    public Vector2 timeUntilCatSits;
    public Sprite[] CatSit;
	public Sprite   CatStand;
	public Sprite[] CatWalk;
    public float    switchTime;
    public bool     isPlayer;

	#endregion

	#region Private Fields

	Vector3        lastPos;
	SpriteRenderer render;
	Timer          sitLookTimer;
	Timer          sitTimer;
    Sprite         currentSprite;
    int            counter = 0;

	#endregion

	#region Private Methods

	void OnValidate()
	{
		Utils.MinMaxValidate(ref timeUntilCatSits);
		Utils.MinMaxValidate(ref timeInSitPosition);
	}

	void SitLook()
	{
		sitLookTimer.End = Utils.RandomRange(timeInSitPosition);

		Sprite last = render.sprite;
		do
		{
			render.sprite = CatSit.RandomItem();
		} while (render.sprite == last);
	}

	void SpriteDirection()
	{
		float currentX = transform.position.x;
		if (Mathf.Abs(currentX - lastPos.x) < 0.001f)
			return;
		GetComponent<SpriteRenderer>().flipX = lastPos.x > currentX;
	}

	void Start()
	{
		sitLookTimer =
			TimeManager.GetNewTimer(
				loops: true,
				onComplete: SitLook);

		sitTimer =
			TimeManager.GetNewTimer(
				onComplete: sitLookTimer.Resume);

		lastPos = transform.position;
		render = GetComponent<SpriteRenderer>();

        counter = 0;
        StartCoroutine("SwitchSprite");
	}

    IEnumerator SwitchSprite()
    {
        currentSprite = CatWalk[counter];

        if (counter < CatWalk.Length - 1)
        {
            counter++;
        }
        else
        {
            counter = 0;
        }

        yield return new WaitForSeconds(switchTime);
        StartCoroutine("SwitchSprite");
    }

	void Update()
	{
        // get controller/arrow input for movement
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (!isPlayer)
        {
            if ((lastPos - transform.position).magnitude < 0.01f && !isPlayer)
            {
                if (!sitTimer.Running && !sitLookTimer.Running)
                {
                    render.sprite = CatStand;
                    sitTimer.End = Utils.RandomRange(timeUntilCatSits);
                    sitTimer.Run();
                }
            }
            else
            {
                render.sprite = currentSprite;
                sitLookTimer.Stop();
                sitTimer.Stop();
                SpriteDirection();
            }
        }
        else
        {
            if (!GameState.InEncounter && (Mathf.Abs(inputX) > 0 || Mathf.Abs(inputY) > 0))
            {
                render.sprite = currentSprite;
                sitLookTimer.Stop();
                sitTimer.Stop();
                SpriteDirection();
            }
            else
            {
                if (!sitTimer.Running && !sitLookTimer.Running)
                {
                    render.sprite = CatStand;
                    sitTimer.End = Utils.RandomRange(timeUntilCatSits);
                    sitTimer.Run();
                }
            }
        }
        lastPos = transform.position;
    }

	#endregion
}