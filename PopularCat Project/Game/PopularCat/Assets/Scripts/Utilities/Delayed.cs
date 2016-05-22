public class Delayed<T>
{
	T currentVal;
	T nextVal;

	public T Value
	{
		get
		{
			return currentVal;
		}
		set
		{
			nextVal = value;
		}
	}

	public void Confirm()
	{
		currentVal = nextVal;
	}

	public static implicit operator T(Delayed<T> container)
	{
		return container.currentVal;
	}

}

