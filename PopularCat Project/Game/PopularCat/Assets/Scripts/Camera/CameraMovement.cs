using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	#region Public Fields

	public float deadzoneRadius;
	public bool fineTracking;

	[Range(0, 10)]
	public float lerpStrength;

	public Vector3 offset;
	public GameObject trackingTarget;

	#endregion

	#region Private Fields

	Camera cam;

	Vector3 targetPosition;
	GameObject upBound;
	Rect viewArea;

	#endregion

	#region Private Properties

	float Bottom { get { return viewArea.yMin; } set { viewArea.y = value; } }

	Vector2 Center { get { return viewArea.center; } }

	float CenterX { get { return viewArea.center.x; } set { viewArea.x = value - HalfWidth; } }

	float CenterY { get { return viewArea.center.y; } set { viewArea.y = value - HalfHeight; } }

	float HalfHeight { get { return Height / 2; } }

	float HalfWidth { get { return Width / 2; } }

	float Height { get { return viewArea.height; } }

	bool HigherThanPlayArea { get { return viewArea.height > Boundary.area.height + UpperBuffer; } }

	float Left { get { return viewArea.xMin; } set { viewArea.x = value; } }

	float Right { get { return viewArea.xMax; } set { viewArea.x = value - Width; } }

	float Top { get { return viewArea.yMax; } set { viewArea.y = value - Height; } }

	float UpperBuffer
	{
		get
		{
			float result = 0;

			if (upBound != null)
				result += upBound.transform.position.y - Boundary.area.yMax;

			return result;
		}
	}

	bool WiderThanPlayArea { get { return viewArea.width > Boundary.area.width; } }

	float Width { get { return viewArea.width; } }

	#endregion

	#region Private Methods

	void Awake()
	{
		cam = GetComponentInChildren<Camera>();
		upBound = GameObject.Find("CameraUpBound");

		viewArea.height = cam.orthographicSize * 2;
		viewArea.width = cam.aspect * viewArea.height;

		if (trackingTarget != null)
		{
			targetPosition = trackingTarget.transform.position;
		}
	}

	void Clamp()
	{
		if (WiderThanPlayArea)
		{
			CenterX = Boundary.area.center.x;
		}
		else
		{
			if (Left < Boundary.Left)
			{
				Left = Boundary.Left;
			}
			else if (Right > Boundary.Right)
			{
				Right = Boundary.Right;
			}
		}

		if (HigherThanPlayArea)
		{
			CenterY = Boundary.area.center.y + UpperBuffer / 2;
		}
		else
		{
			if (Bottom < Boundary.Bottom)
			{
				Bottom = Boundary.Bottom;
			}
			else if (Top > Boundary.Top + UpperBuffer)
			{
				Top = Boundary.Top + UpperBuffer;
			}
		}
	}

	//Temporary debug crap
	void DrawDebugCrud()
	{
		Debug.DrawLine(Center, targetPosition, Color.green);
		Utils.DrawCircle(Center, deadzoneRadius, Color.red);
	}

	void Move()
	{
		Vector3 difference = this.DistanceTo(targetPosition + offset);

		Vector3 nextPos =
			transform.position +
			difference * lerpStrength * Time.deltaTime;

		viewArea.center = nextPos;
		Clamp();
		transform.position = new Vector3(viewArea.center.x, viewArea.center.y, offset.z);
	}

	void OnDrawGizmos()
	{
	}

	void Update()
	{
		if (GameState.Paused)
			return;

		if (trackingTarget != null)
		{
			if (this.DistanceTo(trackingTarget.transform.position).magnitude > deadzoneRadius)
			{
				targetPosition = trackingTarget.transform.position;
			}
			else if (fineTracking)
			{
				var direction = (trackingTarget.transform.position - targetPosition) * 0.005f;
				targetPosition += direction;
			}

			Move();
		}
		DrawDebugCrud();
	}

	#endregion
}