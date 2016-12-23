using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour {
	[SerializeField] MatchView _view;
	bool _isPlaying;
	public bool IsPlaying { get { return _isPlaying; } }
	public static MatchController Instance { get; private set; }
	void Awake() {
		Instance = this;
	}
	public void StartMatch() {
		Entities.Init(new List<EntitySystem>() {
			new MatchSystem(),
			new MovingSystem(),
			new InputControlSystem()
		});
		_view.OnStartMatch();
		_isPlaying = true;
	}

	public void EndMatch() {
		_view.OnEndMatch();
		_isPlaying = false;
	}

	void Update () {
		if (!_isPlaying)
			return;
		Entities.Update();
	}
}
