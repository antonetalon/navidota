using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class JSONData {
	public abstract int Count { get; }
	protected string ParseString(object val, string defaultValue) {
		string str = val as string;
		if (str!=null)
			return str;
		else
			return defaultValue;
	}
	protected int ParseInt(object val, int defaultValue) {
		int number;
		try {
			number = Convert.ToInt32(val);
			return number;
		} catch {
			return defaultValue;
		}
	}
	protected float ParseFloat(object val, float defaultValue) {
		float number;
		try {
			number = Convert.ToSingle(val);
			return number;
		} catch {
			return defaultValue;
		}	
	}
	protected JSONListData WrapList(object val, JSONListData defaultValue) {
		List<object> list = val as List<object>;
		if (list!=null)
			return new JSONListData(list);
		else
			return defaultValue;
	}
	protected JSONDictData WrapDict(object val, JSONDictData defaultValue) {
		Dictionary<string, object> dict = val as Dictionary<string, object>;
		if (dict!=null)
			return new JSONDictData(dict);
		else
			return defaultValue;
	}
	protected object UnWrapList(JSONListData val) {
		List<object> list = new List<object>();
		for (int i=0;i<val.Count;i++) {
			object curr = val.GetValue(i);
			curr = UnWrapValue(curr);
			list.Add(curr);
		}
		return list;
	}
	protected object UnWrapDict(JSONDictData val) {
		Dictionary<string, object> dict = new Dictionary<string, object>();
		foreach (string key in val.GetKeys()) {
			object curr = val.GetValue(key);
			curr = UnWrapValue(curr);
			dict.Add(key, curr);
		}
		return dict;
	}
	private object WrapValue(object val) {
		var dict = WrapDict(val, null);
		if (dict!=null)
			return dict;
		var list = WrapList(val, null);
		if (list!=null)
			return list;
		return val;
	}
	private object UnWrapValue(object val) {
		var dict = val as JSONDictData; 
		if (dict!=null)
			return UnWrapDict(dict);
		var list = val as JSONListData;
		if (list!=null)
			return UnWrapList(list);
		return val;
	}
	protected object WrapChildValues(object val) {
		Dictionary<string, object> dict = val as Dictionary<string, object>;
		if (dict!=null) {
			List<string> keys = new List<string>();
			foreach (string key in dict.Keys) 
				keys.Add(key);
			foreach (string key in keys)
				dict[key] = WrapValue(dict[key]);
			return dict;
		}

		List<object> list = val as List<object>;
		if (list!=null) {
			for (int i=0;i<list.Count;i++)
				list[i] = WrapValue(list[i]);
			return list;
		}

		return val;
	}
	protected object UnWrapChildValues(object val) {
		Dictionary<string, object> dict = val as Dictionary<string, object>;
		if (dict!=null) {
			List<string> keys = new List<string>();
			foreach (string key in dict.Keys) 
				keys.Add(key);
			foreach (string key in keys)
				dict[key] = UnWrapValue(dict[key]);
			return dict;
		}

		List<object> list = val as List<object>;
		if (list!=null) {
			for (int i=0;i<list.Count;i++)
				list[i] = UnWrapValue(list[i]);
			return list;
		}

		return val;
	}
}
public class JSONDictData : JSONData {
	Dictionary<string, object> _rawDataDict;
	public JSONDictData(Dictionary<string, object> rawData) {
		_rawDataDict = rawData;
		_rawDataDict = WrapChildValues(_rawDataDict) as Dictionary<string, object>;
	}
	public JSONDictData(string jsonText) {
		_rawDataDict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
		_rawDataDict = WrapChildValues(_rawDataDict) as Dictionary<string, object>;
	}
	public string ToJson() {
		JSONDictData clone = this.Clone();
		Dictionary<string, object> _dataDict = UnWrapChildValues(clone._rawDataDict) as Dictionary<string, object>;
		return MiniJSON.Json.Serialize(_dataDict);
	}
	public JSONDictData Clone() {
		JSONDictData clone = new JSONDictData(new Dictionary<string, object>());
		foreach (string key in _rawDataDict.Keys) {
			object val = _rawDataDict[key];
			JSONDictData valDict = val as JSONDictData;
			if (valDict!=null)
				val = valDict.Clone();
			JSONListData valList = val as JSONListData ;
			if (valList!=null)
				val = valList.Clone();
			clone.SetValue(key, val);
		}
		return clone;
	}
	public override int Count {	get { return _rawDataDict.Count; } }
	public void SetValue(string key, object val) {
		_rawDataDict[key] = val;
	}
	public void RemoveValue(string key) {
		_rawDataDict.Remove(key);
	}
	public object GetValue(string key, object defaultValue = null) {
		object val;
		if (_rawDataDict==null || !_rawDataDict.TryGetValue(key, out val))
			return defaultValue;
		return val;
	}
	public JSONDictData GetJSONDict(string key) {
		object val;
		if (_rawDataDict==null || !_rawDataDict.TryGetValue(key, out val))
			return null;
		return val as JSONDictData;
	}
	public JSONListData GetJSONList(string key) {
		object val;
		if (_rawDataDict==null || !_rawDataDict.TryGetValue(key, out val))
			return null;
		return val as JSONListData;
	}
	public string GetString(string key, string defaultValue = null) {
		object val;
		if (_rawDataDict==null || !_rawDataDict.TryGetValue(key, out val))
			return defaultValue;
		return ParseString(val, defaultValue);
	}
	public float GetFloat(string key, float defaultValue = -1) {
		object val;
		if (_rawDataDict==null || !_rawDataDict.TryGetValue(key, out val))
			return defaultValue;
		return ParseFloat(val, defaultValue);
	}
	public int GetInt(string key, int defaultValue = -1) {
		object val;
		if (_rawDataDict==null || !_rawDataDict.TryGetValue(key, out val))
			return defaultValue;
		return ParseInt(val, defaultValue);		
	}
	public IEnumerable<string> GetKeys() { return _rawDataDict.Keys; }
}
public class JSONListData : JSONData, IEnumerable {
	List<object> _rawDataList;
	public override int Count {	get { return _rawDataList.Count; } }
	public JSONListData(List<object> rawDataList) {
		_rawDataList = rawDataList;
		_rawDataList = WrapChildValues(_rawDataList) as List<object>;
	}
	public JSONListData Clone() {
		JSONListData clone = new JSONListData(new List<object>());
		foreach (object obj in _rawDataList) {
			object val = obj;
			JSONDictData valDict = val as JSONDictData;
			if (valDict!=null)
				val = valDict.Clone();
			JSONListData valList = val as JSONListData ;
			if (valList!=null)
				val = valList.Clone();
			clone._rawDataList.Add(val);
		}
		return clone;
	}
	public void SetValue(int ind, object val) {
		_rawDataList[ind] = val;
	}
	public void AddValue(object val) {
		_rawDataList.Add(val);
	}
	public void RemoveValue(int ind) {
		_rawDataList.RemoveAt(ind);
	}
	public object GetValue(int ind, object defaultValue = null) {
		if (_rawDataList==null || ind<0 || ind>=_rawDataList.Count)
			return defaultValue;
		return _rawDataList[ind];
	}
	public JSONDictData GetDict(int ind) {
		if (_rawDataList==null)
			return null;
		object val = _rawDataList[ind];
		return val as JSONDictData;
	}
	public JSONListData GetList(int ind) {
		if (_rawDataList==null)
			return null;
		object val = _rawDataList[ind];
		return val as JSONListData;
	}
	public string GetString(int ind, string defaultValue = null) {
		if (_rawDataList==null)
			return null;
		object val = _rawDataList[ind];
		return ParseString(val, defaultValue);
	}
	public int GetInt(int ind, int defaultValue = -1) {
		if (_rawDataList==null)
			return defaultValue;
		object val = _rawDataList[ind];
		return ParseInt(val, defaultValue);
	}
	public float GetFloat(int ind, float defaultValue = -1) {
		if (_rawDataList==null)
			return defaultValue;
		object val = _rawDataList[ind];
		return ParseFloat(val, defaultValue);
	}

	#region IEnumerable implementation

	public IEnumerator GetEnumerator() // Temporary solution. Should be replaced by typed IEnumerator properties like .Floats or .Strings
	{
		return _rawDataList.GetEnumerator();
	}

	#endregion
}
