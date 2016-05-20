using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
	#region Private Fields

	static float CIRCLE_SEGMENTS = 50;

	#endregion

	#region Public Methods

	public static float MoveToward(this float f, float other, float speed)
	{
		if (Mathf.Abs(other - f) < speed)
			return other;

		if (f > other)
			speed *= -1;

		return f + speed;
	}

	public static IEnumerable<Transform> Children(this Transform t)
	{
		for(int i = 0; i < t.childCount; i++)
		{
			yield return t.GetChild(i);
		}

	}

	/// <summary>
	/// Picks an item from the list by evaluating a 0-1 floating point variable. 
	/// <para/>
	/// Clamps value before evaluation 
	/// </summary>
	/// <returns></returns>
	public static T AccessByMagnitude<T>(this IList<T> list, float magnitude)
	{
		magnitude = Mathf.Clamp01(magnitude);
		int index = (int)Mathf.Floor(magnitude * list.Count);
		if (index == list.Count)
			index--;
		return list[index];
	}

	public static Vector3 DistanceTo(this MonoBehaviour mb, GameObject dest)
	{
		return DistanceTo(mb.transform.position, dest.transform.position);
	}

	public static Vector3 DistanceTo(this MonoBehaviour mb, Vector3 dest)
	{
		return DistanceTo(mb.transform.position, dest);
	}

	public static Vector3 DistanceTo(this Vector3 v, Vector3 dest)
	{
		var result = dest - v;
		result.z = 0;
		return result;
	}

	public static void DrawCircle(Vector3 position, float radius, Color c)
	{
		for (int i = 0; i < CIRCLE_SEGMENTS; i++)
		{
			float
				start = (i / CIRCLE_SEGMENTS) * (2 * Mathf.PI),
				end = ((i + 1) / CIRCLE_SEGMENTS) * (2 * Mathf.PI);
			Vector3
				a = new Vector3(radius * Mathf.Sin(start), radius * Mathf.Cos(start)),
				b = new Vector3(radius * Mathf.Sin(end), radius * Mathf.Cos(end));

			a += position;
			b += position;
			Debug.DrawLine(a, b, c);
		}
	}

	/// <summary>
	/// Ensures that the maximum value of a MinMax pair is not less than the minimum 
	/// </summary>
	/// <param name="v"> The Vector2 representing a range </param>
	public static void MinMaxValidate(ref Vector2 v)
	{
		if (v.x > v.y)
			v.y = v.x;
	}

	/// <summary>
	/// Returns a random item from the list 
	/// </summary>
	/// <returns></returns>
	public static T RandomItem<T>(this IList<T> collection)
	{
		return collection[Random.Range(0, collection.Count)];
	}

	/// <summary>
	/// Picks a random float out of a MinMax pair 
	/// </summary>
	/// <returns></returns>
	public static float RandomRange(Vector2 minMax)
	{
		return Random.Range(minMax.x, minMax.y);
	}

	#endregion
}