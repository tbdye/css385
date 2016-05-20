using System.Collections;
using UnityEngine;

public class CameraUpBoundary : MonoBehaviour
{
	#region Private Methods

	void OnDrawGizmos()
	{
		Vector3 left = transform.position;
		left.x = -500;
		Vector3 right = left;
		right.x = 500;
		Debug.DrawLine(left, right, Color.yellow);
	}

	#endregion
}