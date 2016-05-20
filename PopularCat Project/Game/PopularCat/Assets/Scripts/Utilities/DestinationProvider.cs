using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface DestinationProvider
{
}

public static class DestinationUtils
{
	#region Public Methods

	public static void AssertControl(
		this DestinationProvider dp,
		DestinationHandler handler)
	{
		if (handler.providers.Contains(dp))
		{
			while (handler.providers.Last.Value != dp)
			{
				handler.providers.RemoveLast();
			}
		}
	}

	public static void JoinDestinationHandler
		(this DestinationProvider dp,
		DestinationHandler handler)
	{
		handler.providers.AddLast(dp);
	}

	public static void LeaveDestinationHandler
		(this DestinationProvider dp,
		DestinationHandler handler)
	{
		if (handler.providers.Contains(dp))
			handler.providers.Remove(dp);
	}

	public static void SetDestination
		(this DestinationProvider dp,
		DestinationHandler handler,
		Vector3 value)
	{
		if (dp == handler.providers.Last.Value)
			handler.destination = value;
	}

	#endregion
}

public class DestinationHandler
{
	#region Public Fields

	public Vector3 destination;
	public LinkedList<DestinationProvider> providers = new LinkedList<DestinationProvider>();

	#endregion

	#region Public Methods

	public static implicit operator Vector3(DestinationHandler dh)
	{
		return dh.destination;
	}

	#endregion
}
