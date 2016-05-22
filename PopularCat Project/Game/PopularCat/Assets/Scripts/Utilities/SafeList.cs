using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides safe foreach execution that allows modification of the list during a foreach operation 
/// </summary>
/// <typeparam name="T"></typeparam>

public class SafeList<T> : IList<T>
{
	#region Private Fields
	bool dirty = true;
	List<T> content = new List<T>();
	IEnumerable<T> working;

	#endregion

	#region Public Properties

	public int Count
	{
		get
		{
			return content.Count;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return ((ICollection<T>)content).IsReadOnly;
		}
	}

	#endregion

	#region Public Indexers

	public T this[int index]
	{
		get
		{
			return content[index];
		}

		set
		{
			content[index] = value;
			dirty = true;
		}
	}

	#endregion

	#region Public Methods

	public void Add(T item)
	{
		dirty = true;
		content.Add(item);
	}

	public T Find(Predicate<T> match)
	{
		return content.Find(match);
	}

	public void Clear()
	{
		dirty = true;
		content.Clear();
	}

	public bool Contains(T item)
	{
		return content.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		content.CopyTo(array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (dirty)
		{
			working = new List<T>(content);
			dirty = false;
		}
		return working.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		if (dirty)
		{
			working = new List<T>(content);
			dirty = false;
		}
		working = new List<T>(content);
		return working.GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return content.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		dirty = true;
		content.Insert(index, item);
	}

	public bool Remove(T item)
	{
		dirty = true;
		return content.Remove(item);
	}

	public void RemoveAt(int index)
	{
		dirty = true;
		content.RemoveAt(index);
	}

	#endregion
}