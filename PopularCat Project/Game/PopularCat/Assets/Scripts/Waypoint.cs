using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour, DestinationProvider
{
	public Waypoint next;
	public Waypoint Previous { get; set; }
	List<DestinationHandler> handlers;

	void OnDrawGizmos()
	{
		var pos = transform.position;
		var offset = new Vector3(1,1);
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(pos + offset, pos - offset);
		offset.x = -offset.x;
		Gizmos.DrawLine(pos + offset, pos - offset);
		Utils.DrawCircle(pos, 1, Color.green);
	}

	public Waypoint VisitNext(DestinationHandler dh)
	{
		if (next == null)
			return this;

		if (handlers.Contains(dh))
		{
			this.LeaveDestinationHandler(dh);
			handlers.Remove(dh);
		}

		next.Visit(dh);
		return next;
	}
	public Waypoint VisitPrevious(DestinationHandler dh)
	{
		if (Previous == null)
			return this;

		if (handlers.Contains(dh))
		{
			this.LeaveDestinationHandler(dh);
			handlers.Remove(dh);
		}

		Previous.Visit(dh);
		return Previous;
	}
	public void Visit(DestinationHandler dh)
	{
		this.JoinDestinationHandler(dh);
		handlers.Add(dh);
	}

	public void Leave(DestinationHandler dh)
	{
		this.LeaveDestinationHandler(dh);
		handlers.Remove(dh);
	}

	void Awake()
	{
		handlers = new List<DestinationHandler>();
		if (next != null)
			next.Previous = this;
	}

	void Update()
	{
		foreach(var h in handlers)
			this.SetDestination(h, transform.position);
	}
}
