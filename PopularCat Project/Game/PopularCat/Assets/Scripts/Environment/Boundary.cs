using System.Collections;
using UnityEngine;

public class Boundary : MonoBehaviour
{
	#region Public Fields

	public static Rect area;

	public float height = 100;
	public float width  = 100;

	#endregion

	#region Public Properties

	public static float   Bottom      { get { return area.yMin; } }
	public static Vector3 BottomLeft  { get { return new Vector3(Left, Bottom); } }
	public static Vector3 BottomRight { get { return new Vector3(Right, Bottom); } }
	public static Vector3 Center      { get { return area.center; } }
	public static float   Height      { get { return area.height; } }
	public static float   Left        { get { return area.xMin; } }
	public static float   Right       { get { return area.xMax; } }
	public static float   Top         { get { return area.yMax; } }
	public static Vector3 TopLeft     { get { return new Vector3(Left, Top); } }
	public static Vector3 TopRight    { get { return new Vector3(Right, Top); } }

	#endregion

	#region Public Methods

	public static void Clamp(GameObject obj)
	{
		var collider = obj.GetComponent<Collider2D>();

		var bounds = collider.bounds;

		var ex = bounds.extents.x;
		var ey = bounds.extents.y;

		var newPosition = bounds.center;

		newPosition.x = Mathf.Clamp(newPosition.x, Left + ex, Right - ex);
		newPosition.y = Mathf.Clamp(newPosition.y, Bottom + ey, Top - ey);

		newPosition.x -= collider.offset.x;
		newPosition.y -= collider.offset.y;

		obj.transform.position = newPosition;
	}

	public static bool IsInside(GameObject obj, float lenience = 0)
	{
		return !IsOutside(obj, -lenience);
	}

	public static bool IsOutside(GameObject obj, float lenience = 0)
	{
		var pos = obj.transform.position;
		return
			pos.x + lenience < area.xMin ||
			pos.x - lenience > area.xMax ||
			pos.y + lenience < area.yMin ||
			pos.y - lenience > area.yMax;
	}

	public static Vector3 RandomPoint(Vector3 reference = new Vector3(), float buffer = 0)
	{
		Vector3 result = reference;
		result.x -= UnityEngine.Random.Range(Left + buffer, Right - buffer);
		result.y -= UnityEngine.Random.Range(Bottom + buffer, Top - buffer);

		return result;
	}

	#endregion

	#region Private Methods

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green * 0.7f;
		//Corner square
		{
			var size = Mathf.Max(Top, Right) * 0.045f;
			var s = new Vector3(size, size);
			Gizmos.DrawCube(TopLeft * 1.05f, s);
		}
		Gizmos.DrawLine(TopLeft, TopRight);
		Gizmos.DrawLine(TopRight, BottomRight);
		Gizmos.DrawLine(BottomRight, BottomLeft);
		Gizmos.DrawLine(BottomLeft, TopLeft);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(TopLeft, TopRight);
		Gizmos.DrawLine(TopRight, BottomRight);
		Gizmos.DrawLine(BottomRight, BottomLeft);
		Gizmos.DrawLine(BottomLeft, TopLeft);
	}

	private void OnValidate()
	{
		area.width = width;
		area.height = height;
		area.center = Vector2.zero;
	}

	private void Reset()
	{
		width = 100;
		height = 100;
	}

	private void Start()
	{
		area.width = width;
		area.height = height;
		area.center = Vector2.zero;
	}

	#endregion
}