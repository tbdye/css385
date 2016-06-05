using System;
using System.Collections.Generic;
using UnityEngine;

public class Swarmer : MonoBehaviour, DestinationProvider
{
	#region Public Fields

	public Waypoint destination;

	public float slackDistance;
	public float speed;

	[Tooltip("X: Minimum time in seconds\nY: Maximum time")]
	public Vector2 updateFrequency;

	public float wanderDistance;

	#endregion

	#region Private Fields

	float updateCounter;

	#endregion

	#region Public Properties

	public bool InSwarm { get; private set; }
	public SwarmMovement Swarm { get; private set; }
	public CircularArea SwarmArea { get; private set; }
	public DestinationHandler TargetPosition { get; private set; }

	#endregion

	#region Public Methods

	public Vector3 Destination()
	{

		if (InSwarm)
		{
			if (GameState.EndOfLevel)
				return transform.position;
			return SwarmArea.RandomPoint;
		}
			

		var rand = UnityEngine.Random.insideUnitCircle * wanderDistance;
		var newPos = transform.position + new Vector3(rand.x, rand.y);

		return newPos;
	}

	public void JoinCrowd()
	{
		InSwarm = true;
		Swarm.Add(gameObject);
	}

	public void LeaveCrowd()
	{
		InSwarm = false;
		Swarm.Remove(gameObject);
	}

	#endregion

	#region Private Methods

	void DrawDebugJunk()
	{
		Debug.DrawLine(transform.position, TargetPosition, Color.green);
	}

	void OnValidate()
	{
		Utils.MinMaxValidate(ref updateFrequency);
	}

	void Start()
	{
		var go = GameObject.Find("PosseCrowd");
		SwarmArea = go.GetComponent<CircularArea>();
		Swarm = SwarmMovement.Instance;
		TargetPosition = new DestinationHandler();
		this.JoinDestinationHandler(TargetPosition);
	}

	void Update()
	{
		if (!GameState.InEncounter)
			updateCounter -= Time.deltaTime;

		if (updateCounter < 0 ||
			(InSwarm && !SwarmArea.Contains(gameObject, slackDistance)))
		{
			updateCounter = Utils.RandomRange(updateFrequency);
			this.SetDestination(TargetPosition, Destination());
		}

		Vector3 movement = this.DistanceTo(TargetPosition);
		if (movement.magnitude > movement.normalized.magnitude)
		{
			movement = movement.normalized * speed;
		}
		if (movement.magnitude > 0.7f)
			transform.position += movement * Time.deltaTime;
		DrawDebugJunk();
	}

	#endregion
}