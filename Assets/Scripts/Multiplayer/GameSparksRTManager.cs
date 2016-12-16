using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Responses;
using System;
using GameSparks.RT;

public class GameSparksRTManager : MonoBehaviour {
	public static GameSparksRTManager Instance { get; private set; }
	[SerializeField] GameSparksRTUnity _RT;
	public event Action OnRTDisconnected;

	void Awake() {
		if (Instance!=null) {
			DestroyImmediate(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
	}
	bool _isStartingSession;
	Action<bool> _onResponse;
	public void StartNewRTSession(string host, int port, string accessToken, Action<bool> onResponse) {
		if (_isStartingSession) {
			if (onResponse!=null)
				onResponse(false);
			return;
		}
		_isStartingSession = true;
		_onResponse = onResponse;
		GSRequestData mockedResponse = new GSRequestData()
			.AddNumber("port", (double)port)
			.AddString("host", host)
			.AddString("accessToken", accessToken);
		FindMatchResponse response = new FindMatchResponse(mockedResponse);
		_RT.Configure(response, 
			(peerId) =>  {    OnPlayerConnectedToGame(peerId);  },
			(peerId) => {    OnPlayerDisconnected(peerId);    },
			(ready) => {    OnRTReady(ready);    },
			(packet) => {    OnPacketReceived(packet);    });
		_RT.Connect();
	}

	public void RTSessionDisconnect() {
		_RT.Disconnect();
	}

	private void OnPlayerConnectedToGame(int _peerId){
		//Debug.Log ("Player Connected, "+_peerId);
	}

	private void OnPlayerDisconnected(int _peerId){
		//Debug.Log ("Player Disconnected, "+_peerId);
	}
	private void OnRTReady(bool isReady){		
		if (isReady) {
			Debug.Log ("RT Session Connected...");
		} else {
			if (OnRTDisconnected!=null)
				OnRTDisconnected();
		}
		if (!_isStartingSession)
			return;
		if (_onResponse!=null) {
			_onResponse(isReady);
			_onResponse = null;
		}
		_isStartingSession = false;
	}
	private void OnPacketReceived(RTPacket _packet){
	}
}
