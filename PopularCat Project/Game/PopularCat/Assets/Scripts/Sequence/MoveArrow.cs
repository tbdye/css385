using System.Collections;
using UnityEngine;

public class MoveArrow : MonoBehaviour
{
	#region Private Fields

	Transform arrow;

	#endregion

	#region Private Methods

	void Start()
	{
		arrow = transform.FindChild("Arrow");
	}

	void Update()
	{
		float distance = 192;

		float pos = 
			(((arrow.localPosition.y + distance/2) + 
			(350 * Time.deltaTime)) % distance) - 
			distance/2;
		arrow.localPosition = new Vector3(0, pos, 0);
	}

	#endregion
}