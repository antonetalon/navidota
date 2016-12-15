using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Observable {

	private string _name;
	public string Name { 
		get { return _name; }
		set {
			if (_name==value)
				return;
			_name = value;
			NotifyObservers();
		}
	}
	public PlayerModel(string name) {
		_name = name;
	}
}
