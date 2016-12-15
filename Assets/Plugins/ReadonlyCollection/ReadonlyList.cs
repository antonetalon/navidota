using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadonlyList<T>:IEnumerable<T> {
	private List<T> _list;
	public int Count { get { return _list.Count; } }
	public T GetValue(int i) {
		return _list[i];
	}
	public ReadonlyList(List<T> list) {
		_list = list;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator () {
		return _list.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator () {
		return _list.GetEnumerator();
	}
	public bool Contains(T item) {
		return _list.Contains(item);
	}
	public T this[int ind] { get { return _list[ind]; } }
}
