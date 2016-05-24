using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Maintains a float clamped to a range.
/// <para/>
/// Offers delegate function properties.
/// </summary>
public class MonitoredValue
{
	public bool Locked { get; private set; }
	float val;
	public float Value
	{
		get
		{
			return val;
		}
		set
		{
			if (Locked)
				return;

			var previous = val;
			val = Mathf.Clamp(value, Min, Max);

			if (val == previous)
				return;

			if (OnModified != null)
				OnModified(val);

			if (val == Max && OnFull != null)
				OnFull(val);
			else if (val == Min && OnEmpty != null)
				OnEmpty(val);
		}
	}

	public float Min { get; set; }
	public float Max { get; set; }

	/// <summary>
	/// The method called whenever the value is changed.
	/// <para/>
	/// Only triggers if the value changed.
	/// <para/>
	/// Passes the new value as a parameter.
	/// </summary>
	public Action<float> OnModified { get; set; }

	/// <summary>
	/// The method called whenever the value reaches Max.
	/// <para/>
	/// Only triggers if the value changed.
	/// <para/>
	/// Passes the new value as a parameter.
	/// </summary>
	public Action<float> OnFull { get; set; }

	/// <summary>
	/// The method called whenever the value reaches Min.
	/// <para/>
	/// Only triggers if the value changed.
	/// <para/>
	/// Passes the new value as a parameter.
	/// </summary>
	public Action<float> OnEmpty { get; set; }

	public static implicit operator float(MonitoredValue monitor)
	{
		return monitor.val;
	}

	public MonitoredValue
		(float startingValue = 0,
		float min = 0,
		float max = 1,
		Action<float> onModified = null,
		Action<float> onFull = null,
		Action<float> onEmpty = null)
	{
		val = startingValue;
		Min = min;
		Max = max;
		OnModified = onModified;
		OnFull = onFull;
		OnEmpty = onEmpty;
	}

	public void Lock() { Locked = true; }
	public void Unlock() { Locked = false; }

	public void FloorDecrement(int quantization)
	{
		float f = Value * quantization;
		f -= 1;
		f = Mathf.Ceil(f) / quantization;


		Value = f;
	}
}
