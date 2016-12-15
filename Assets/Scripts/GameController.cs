using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static bool Inited { get { return Instance!=null && Instance.CurrPlayer!=null; } }
	public static bool Authed { get { return Instance!=null && Instance.CurrAuth!=null; } }
	public static GameController Instance { get; private set; }
	public PlayerModel CurrPlayer { get; private set; }
	public AuthModel CurrAuth { get; private set; }

	void Awake() {
		if (Instance!=null) {
			Destroy(this);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
		PlayerController.Init();
	}	

	public void SetCurrPlayer(PlayerModel player) {
		CurrPlayer = player;
	}
	public void SetCurrAuth(AuthModel auth) {
		CurrAuth = auth;
	}

	void Update () {
		AuthController.Update();
	}
}
