using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
	#region Private Fields

	private List<SpriteRenderer> characters, obstacles;

	#endregion

	#region Public Methods

	public int AssignLayer(float posVertical)
	{
		return (int)Mathf.Floor(posVertical);
	}

	#endregion

	#region Private Methods

	void Start()
	{
		// Find character and obstacle objects.
		GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Character");
		characters = new List<SpriteRenderer>();

		foreach (GameObject n in characterObjects)
		{
			var sr = n.GetComponent<SpriteRenderer>();
			if (sr != null)
				characters.Add(sr);
		}

		GameObject[] obstacleObjects = GameObject.FindGameObjectsWithTag("Obstacle");
		obstacles = new List<SpriteRenderer>();

		foreach (GameObject n in obstacleObjects)
		{
			var obstacle = n.GetComponent<SpriteRenderer>();
			var collide = n.GetComponent<Collider2D>();
			obstacles.Add(obstacle);
			obstacle.sortingOrder = AssignLayer(collide.bounds.center.y * -3);
		}
	}

	void Update()
	{
		foreach (SpriteRenderer sr in characters)
		{
			var coll = sr.GetComponent<Collider2D>();
			if (coll != null)
				sr.sortingOrder =
					AssignLayer(coll.bounds.center.y * -3);
			else
			{
				coll = sr.GetComponentInParent<Collider2D>();
				if (coll != null)
					sr.sortingOrder =
						AssignLayer(coll.bounds.center.y * -3);
			}
		}
	}

	#endregion

	/*
	void RandomizeObstacles()
	{
		if (Input.GetButtonUp("Fire1"))
		{
			foreach (Collider2D obstacle in mObstacles)
			{
				Vector3 relocate = new Vector3(Random.Range(-6f, 8f), Random.Range(-3f, 2f), 0f);
				obstacle.transform.position = relocate;
				obstacle.GetComponent<SpriteRenderer>().sortingOrder = AssignLayer(obstacle.bounds.center.y);
			}
		}
	}
	*/
}