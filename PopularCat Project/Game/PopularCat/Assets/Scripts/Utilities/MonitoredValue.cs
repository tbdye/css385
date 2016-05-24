using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Maintains a float clamped to a range. 
/// <para/>
/// Offers delegate function properties. 
/// </summary>
public class MonitoredValue
{
	#region Private Fields

	float val;

	#endregion

	#region Public Properties

	public bool Locked { get; private set; }
	public float Max { get; set; }

	public float Min { get; set; }

	/// <summary>
	/// The method called whenever the value reaches Min. 
	/// <para/>
	/// Only triggers if the value changed. 
	/// <para/>
	/// Passes the new value as a parameter. 
	/// </summary>
	public Action<float> OnEmpty { get; set; }

	/// <summary>
	/// The method called whenever the value reaches Max. 
	/// <para/>
	/// Only triggers if the value changed. 
	/// <para/>
	/// Passes the new value as a parameter. 
	/// </summary>
	public Action<float> OnFull { get; set; }

	/// <summary>
	/// The method called whenever the value is changed. 
	/// <para/>
	/// Only triggers if the value changed. 
	/// <para/>
	/// Passes the new value as a parameter. 
	/// </summary>
	public Action<float> OnModified { get; set; }

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

	#endregion

	#region Public Constructors

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

	#endregion

	#region Public Methods

	public static implicit operator float(MonitoredValue monitor)
	{
		return monitor.val;
	}

	public void Floor(int quantization)
	{
		float f = Value * quantization;
		f = Mathf.Floor(f) / quantization;

		Value = f;
	}

	public void FloorDecrement(int quantization)
	{
		float f = Value * quantization;
		f -= 1;
		f = Mathf.Ceil(f) / quantization;

		Value = f;
	}

	public void Lock() { Locked = true; }

	public void Unlock() { Locked = false; }

	#endregion
}