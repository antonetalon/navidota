using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSparks.RT;
using System.Linq;
using System.Text;

public abstract class EntityComponent {

	public static EntityComponent Create(Type t) {
		if (t==typeof(MovingComponent))
			return new MovingComponent();
		if (t==typeof(MatchComponent))
			return new MatchComponent();
		if (t==typeof(PositionComponent))
			return new PositionComponent();
		if (t==typeof(InputControlComponent))
			return new InputControlComponent();
		Debug.LogError("Component ctor not added for " + t.ToString());
		return null;
	}
	public static void Init() {
		indByType = new Dictionary<Type, int>();
		typeByInd = new Dictionary<int, Type>();
		AddComponentDesc(typeof(MatchComponent), 1);
		AddComponentDesc(typeof(PositionComponent), 2);
		AddComponentDesc(typeof(MovingComponent), 3);
		AddComponentDesc(typeof(InputControlComponent), 4);
	}
	static Dictionary<Type, int> indByType;
	static Dictionary<int, Type> typeByInd;
	static void AddComponentDesc(Type t, int typeInd) {
		indByType.Add(t, typeInd);
		typeByInd.Add(typeInd, t);
	}
	public static Type GetType(int typeInd) {
		return typeByInd[typeInd];
	}
	public static int GetInd(Type t) {
		return indByType[t];
	}
	public static EntityComponent Create(Type t, RTData data, uint indInData) {
		EntityComponent comp = Create(t);
		uint i = indInData;
		for (int j=0;j<comp._numbers.Count;j++) {
			comp._numbers[j] = data.GetFloat(i).Value; i++;
		}
		for (int j=0;j<comp._vectors.Count;j++) {
			float x = data.GetFloat(i).Value; i++;
			float y = data.GetFloat(i).Value; i++;
			comp._vectors[j] = new Vector2(x, y);
		}
		for (int j=0;j<comp._bools.Count;j++) {
			comp._bools[j] = data.GetInt(i).Value==1; i++;
		}
		for (int j=0;j<comp._strings.Count;j++) {
			comp._strings[j] = data.GetString(i); i++;
		}
		return comp;
	}
	private List<float> _numbers;
	private List<Vector2> _vectors;
	private List<bool> _bools;
	private List<string> _strings;
	protected float GetNumber(int ind) { return _numbers[ind]; }
	protected Vector2 GetVector2(int ind) { return _vectors[ind]; }
	protected bool GetBool(int ind) { return _bools[ind]; }
	protected string GetString(int ind) { return _strings[ind]; }
	protected void SetNumber(int ind, float value) { _numbers[ind] = value; }
	protected void SetVector2(int ind, Vector2 value) { _vectors[ind] = value; }
	protected void SetBool(int ind, bool value) { _bools[ind] = value; }
	protected void SetString(int ind, string value) { _strings[ind] = value; }
	public EntityComponent(int intFiledsCount, int floatFieldsCount, int vector2FieldsCount, int boolFieldsCount, int stringFieldsCount) {
		_numbers = new List<float>();
		for (int i=0;i<intFiledsCount+floatFieldsCount;i++)
			_numbers.Add(0);
		_vectors = new List<Vector2>();
		for (int i=0;i<vector2FieldsCount;i++)
			_vectors.Add(Vector2.zero);
		_bools = new List<bool>();
		for (int i=0;i<boolFieldsCount;i++)
			_bools.Add(false);
		_strings = new List<string>();
		for (int i=0;i<stringFieldsCount;i++)
			_strings.Add(string.Empty);
	}

	public void AddChange(EntityComponent change) {
		bool componentsAddable = _numbers.Count == change._numbers.Count && _vectors.Count == change._vectors.Count && _bools.Count == change._bools.Count && _strings.Count == change._strings.Count;
		if (!componentsAddable) {
			Debug.LogError ("Change to component must be the same type as a component");
			return;
		}
		for (int i = 0; i < _numbers.Count; i++)
			_numbers [i] = change._numbers [i];
		for (int i = 0; i < _vectors.Count; i++)
			_vectors [i] = change._vectors [i];
		for (int i = 0; i < _bools.Count; i++)
			_bools [i] = change._bools [i];
		for (int i = 0; i < _strings.Count; i++)
			_strings [i] = change._strings [i];
	}

	public override string ToString () {
		return string.Format("numbers=[{0}], vecs=[{1}], bools=[{2}], strings=[{3}]", ListJoin<float>(_numbers), ListJoin<Vector2>(_vectors), ListJoin<bool>(_bools), ListJoin<string>(_strings));
	}
	private static string ListJoin<T>(List<T> list) {
		StringBuilder sb = new StringBuilder();
		for (int i=0;i<list.Count;i++) {
			if (i>0)
				sb.Append(",");
			sb.Append(list[i].ToString());
		}
		return sb.ToString();
	}
}
