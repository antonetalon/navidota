using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public sealed class ReadOnlyDictionary < TKey, TValue > : IEnumerable
{
	IDictionary<TKey , TValue> _items;
	public ReadOnlyDictionary(IDictionary<TKey , TValue> items) {
		_items = items;
	}

	public TValue this[TKey key]
	{
		get
		{
			return _items[key];
		}
	}
	
	public bool ContainsKey(TKey key)
	{
		return _items.ContainsKey(key);
	}

	public ICollection<TKey> Keys { get { return _items.Keys; } }
	public ICollection<TValue> Values { get { return _items.Values; } }
	#region IEnumerable implementation
	IEnumerator IEnumerable.GetEnumerator () {
		return _items.GetEnumerator();
	}
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator () {
		return _items.GetEnumerator();
	}
	#endregion
}
