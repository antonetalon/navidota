using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;
using System;

public class GameController : MonoBehaviour {

	public static bool Inited { get { return Instance!=null && Instance.CurrPlayer!=null; } }
	public static bool Authed { get { return Instance!=null && Instance.Auth!=null; } }
	public static GameController Instance { get; private set; }
	public PlayerModel CurrPlayer { get; private set; }
	public AuthModel Auth { get; private set; }
	public LobbyModel Lobby { get; private set; }

	void Awake() {
		if (Instance!=null) {
			Destroy(this);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
	}	
	void Start() {
		PlayerController.Init();
		LobbyController.Init();
	}	

	public void SetCurrPlayer(PlayerModel player) {
		CurrPlayer = player;
	}
	public void SetCurrAuth(AuthModel auth) {
		Auth = auth;
	}
	public void SetLobby(LobbyModel lobby) {
		Lobby = lobby;
	}

	void Update () {
		AuthController.Update();
		LobbyController.Update();
	}

	public void ExecuteAtNextFrame(Action action) {
		StartCoroutine (ExecutingAtNextFrame (action));
	}
	IEnumerator ExecutingAtNextFrame(Action action) {
		yield return new WaitForEndOfFrame ();
		action ();
	}
}
