using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour {
	[SerializeField] MatchView _view;
	void Awake() {
		Entities.Init(new List<EntitySystem>() {
			new MatchStartSystem(),
			new MovingSystem(),
			new InputControlSystem()
		});
		_view.OnStartMatch();
	}

	void Update () {
		Entities.Update();
	}
}
