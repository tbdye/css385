using UnityEngine;
using System.Collections;

public class CircularArea : MonoBehaviour
{
	public float radius;

	private void OnDrawGizmos()
	{
		Utils.DrawCircle(gameObject.transform.position, radius, Color.blue);
	}

	public bool Contains(GameObject obj, float lenience = 0)
	{
		Vector3 test = obj.transform.position - transform.position;
		test.z = 0;
		return test.magnitude < radius + lenience;
	}

	public Vector3 RandomPoint
	{
		get
		{
			var r = Random.insideUnitCircle * radius;
			var result = transform.position + new Vector3(r.x, r.y);
			return result;
		}

	}

}
