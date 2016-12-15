using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthModel : Observable {

	public enum Type {
		None,
		Device,
		LoginPass
	}
	// Auth type player used to auth for current session.
	public Type ActiveType { get; private set; }
	private List<Type> _existingTypes;
	public ReadonlyList<Type> ExistingTypes;
	public AuthModel(Type type) {
		ActiveType = type;
		_existingTypes = new List<Type>();
		if (ActiveType != Type.None);
			_existingTypes.Add(ActiveType);
	}
	public void AddAuthType(Type type) {
		_existingTypes.Add(type);
		NotifyObservers();
	}
}


