using System.Collections.Generic;
using UnityEngine;

public class SwarmMovement : MonoBehaviour
{
	#region Public Fields

	public bool adjustLeaderBias, growOnAddition;

	public float growthFactor = 1.4f;

	[Range(0, 1)]
	public float lerpStrength;
	
	public SafeList<UnitListing> listings;

	#endregion

	#region Private Fields

	CircularArea area;

	#endregion

	#region Public Properties

	public static SwarmMovement Instance    { get; private set; }
	public        Vector3       Destination { get; private set; }

	#endregion

	#region Public Methods

	public void Add(GameObject obj, float bias = 1)
	{
		var listing = new UnitListing { unit = obj, weight = bias };
		listings.Add(listing);
		if (adjustLeaderBias)
		{
			listings[0].weight += bias;
		}
		if (growOnAddition)
		{
			area.radius +=
				Mathf.Pow(listings.Count, 1f / growthFactor) -
				Mathf.Pow(listings.Count - 1, 1f / growthFactor);
		}
	}

	public void ImpressMembers()
	{
		foreach (var l in listings)
		{
			var oc = l.unit.GetComponent<OtherCat>();
			if(oc != null)
				oc.Impress();
		}
	}
	public void BoreMembers()
	{
		foreach (var l in listings)
		{
			var oc = l.unit.GetComponent<OtherCat>();
			if (oc != null)
				oc.Bore();
		}
		
	}

	public void Remove(GameObject obj)
	{
		var listing = listings.Find((x) => x.unit == obj);
		if (listing == null)
			return;

		if (adjustLeaderBias)
		{
			listings[0].weight -= listing.weight;
		}
		if (growOnAddition)
		{
			area.radius -=
				Mathf.Pow(listings.Count, 1f / growthFactor) -
				Mathf.Pow(listings.Count - 1, 1f / growthFactor);
		}

		listings.Remove(listing);
	}

	#endregion

	#region Private Methods

	void Awake()
	{
		listings = new SafeList<UnitListing>();
		area = GetComponent<CircularArea>();
		Instance = this;

		listings.Add(new UnitListing {
			unit = FindObjectOfType<Player>().gameObject,
			weight = 1 });
	}

	void Update()
	{
		if (GameState.Paused)
			return;

		//Update destination
		{
			float count = 0;
			Vector3 newTarget = Vector3.zero;
			foreach (UnitListing ul in listings)
			{
				count += ul.weight;
				newTarget += ul.unit.transform.position * ul.weight;
			}
			newTarget /= count;

			Destination = newTarget;
		}

		var dist = this.DistanceTo(Destination);

		transform.position += dist * lerpStrength;
	}

	#endregion

	#region Public Classes

	[System.Serializable]
	public class UnitListing
	{
		#region Public Fields

		public GameObject unit;
		public float weight = 1;

		#endregion
	}

	#endregion
}