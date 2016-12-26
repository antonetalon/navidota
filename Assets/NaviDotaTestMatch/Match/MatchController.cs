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
		GameSparksRTManager.Instance.SetCommandsFactory(new NaviDotaCommandsParser());
		var systems = new List<EntitySystem>() {
			new MatchSystem(),
			new MovingSystem()
		};
		if (multiplayer) {
			systems.Add(new NetworkInputSystem());
		} else {
			systems.Add(new InputControlSystem());
		}
		Entities.Init(systems);
		SyncChangesController.OnStartMatch();
		_view.OnStartMatch();
		_isPlaying = true;
	}

	public void EndMatch() {
		SyncChangesController.OnEndMatch();
		_view.OnEndMatch();
		_isPlaying = false;
	}

	void Update () {
		if (!_isPlaying)
			return;
		Entities.Update();
	}
}
