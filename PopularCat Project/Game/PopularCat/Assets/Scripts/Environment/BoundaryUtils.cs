using UnityEngine;
using System.Collections;

public static class BoundaryUtils
{

	public static bool IsOutside(this GameObject obj, float lenience = 0)
	{
		var pos = obj.transform.position;
		return
			pos.x + lenience < Boundary.Left ||
			pos.x - lenience > Boundary.Right ||
			pos.y + lenience < Boundary.Bottom ||
			pos.y - lenience > Boundary.Top;
	}

	public static bool IsInside(this GameObject obj, float lenience = 0)
	{
		return !IsOutside(obj, -lenience);
	}

	public static void ClampToBoundary(this MonoBehaviour obj)
	{
		Boundary.Clamp(obj.gameObject);
	}
}
