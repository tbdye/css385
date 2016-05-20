using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosseFormation : MonoBehaviour, DestinationProvider
{
	#region Private Fields
	
	Vector3 lastAdded = new Vector3(0,-7);
	List<Transform> positions;
	SwarmMovement swarm;
	List<Swarmer> units;
	GameObject Target { get; set; }

	#endregion

	#region Public Properties

	public static PosseFormation Instance { get; private set; }

	public bool InPosition{ get; private set; }

	#endregion

	#region Public Methods

	public void GetIntoPosition(GameObject target)
	{
		Target = target;
		foreach (var s in swarm.listings)
		{
			var unit = s.unit.GetComponent<Swarmer>();
			if (unit != null)
				units.Add(unit);
		}

		foreach (var u in units)
		{
			this.JoinDestinationHandler(u.TargetPosition);
		}
	}

	public void ReleasePosition()
	{
		foreach (var u in units)
		{
			this.LeaveDestinationHandler(u.TargetPosition);
		}
		units.Clear();
	}

	#endregion

	#region Private Methods

	Transform GetNewPosition()
	{
		var go = new GameObject();
		go.transform.SetParent(transform);

		lastAdded = new Vector3(lastAdded.x * -1, lastAdded.y);
		if (positions.Count % 2 == 0)
		{
			lastAdded += new Vector3(3, 3);
		}

		go.transform.localPosition = lastAdded;

		return go.transform;
	}

	void Awake()
	{
		swarm = GameObject.Find("PosseCrowd").GetComponent<SwarmMovement>();
		units = new List<Swarmer>();
		positions = new List<Transform>();
		Instance = this;
	}

	void Update()
	{
		if (!Target)
			return;

		transform.rotation = Quaternion.LookRotation(transform.forward,this.DistanceTo(Target));

		int c = units.Count;
		while (c > positions.Count)
		{
			positions.Add(GetNewPosition());
		}

		int i = 0;
		foreach (var u in units)
		{
			var result = positions[i++].position;
			result.z = 0;
			this.SetDestination(u.TargetPosition, result);
		}
	}

	#endregion
}