using UnityEngine;
using System.Collections;

public class SlowFloat
{
	float slowValue, destinationValue, moveSpeed;

	Timer updateTimer;

	public float Speed
	{
		get { return moveSpeed; }
		set
		{
			moveSpeed = Mathf.Abs(value);
		}
	}

	public float Value
	{
		get { return slowValue; }
		set
		{
			destinationValue = value;
		}
	}

	public SlowFloat(float speed = 1f)
	{
		Speed = speed;
		updateTimer = TimeManager.GetNewTimer(loops: true);
		updateTimer.OnTick = UpdateValue;
		updateTimer.Run();
	}

	public static implicit operator float(SlowFloat sf)
	{
		return sf.slowValue;
	}

	void UpdateValue(float deltaTime)
	{
		if(slowValue > destinationValue)
		{
			if ((slowValue -= moveSpeed * deltaTime) < destinationValue)
				slowValue = destinationValue;
		}
		else if (slowValue < destinationValue)
		{
			if ((slowValue += moveSpeed * deltaTime) > destinationValue)
				slowValue = destinationValue;
		}
	}
}
