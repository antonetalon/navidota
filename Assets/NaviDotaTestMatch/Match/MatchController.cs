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
	public void StartMatch(bool multiplayer) {		
		var systems = new List<EntitySystem>() {
			new MovingSystem()
		};
		if (multiplayer) {
			systems.Add(new NetworkMatchSystem());
			systems.Add(new NetworkInputSystem());
		} else {
			systems.Add(new SinglePlayerMatchSystem());
			systems.Add(new InputControlSystem());
		}
		LagController.OnMatchStarted ();
		TimeMachiene.OnStartMatch ();
		Entities.Init(systems);
		SyncChangesController.OnStartMatch();
		_view.OnStartMatch();
		_isPlaying = true;
	}

	public void EndMatch() {
		LagController.OnMatchFinished ();
		SyncChangesController.OnEndMatch();
		_view.OnEndMatch();
		_isPlaying = false;
	}

	public void UpdateWithDelta (float delta) {
		Timer.Update (delta);
		if (!_isPlaying)
			return;
		Entities.Update();
		LagController.Update ();
	}
	void Update () {
		UpdateWithDelta (Time.deltaTime);
	}
}
