using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
	#region Private Fields

	private List<SpriteRenderer> characters, obstacles;
	int drawLayers;

	#endregion

	#region Public Properties

	public float LayerIncrement { get; private set; }

	#endregion

	#region Public Methods

	public int AssignLayer(float posVertical)
	{
		return (int)Mathf.Floor(drawLayers * (1 - (posVertical - Boundary.Bottom) / (Boundary.Height)));
	}

	#endregion

	#region Private Methods

	void SliceCamera()
	{
		Camera worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		LayerIncrement = worldCamera.orthographicSize * 2 / drawLayers;
	}

	void Start()
	{
		// Scene camera must be tagged with "MainCamera" in order for this to work.
		drawLayers = 100;
		SliceCamera();

		// Find character and obstacle objects.
		GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Character");
		characters = new List<SpriteRenderer>();

		foreach (GameObject n in characterObjects)
		{
			characters.Add(n.GetComponent<SpriteRenderer>());
		}

		GameObject[] obstacleObjects = GameObject.FindGameObjectsWithTag("Obstacle");
		obstacles = new List<SpriteRenderer>();

		foreach (GameObject n in obstacleObjects)
		{
			var obstacle = n.GetComponent<SpriteRenderer>();
			var collide = n.GetComponent<Collider2D>();
			obstacles.Add(obstacle);
			obstacle.sortingOrder = AssignLayer(collide.bounds.center.y);
		}
	}

	void Update()
	{
		foreach (SpriteRenderer sr in characters)
		{
			sr.sortingOrder =
				AssignLayer(sr.GetComponent<Collider2D>().bounds.center.y);
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