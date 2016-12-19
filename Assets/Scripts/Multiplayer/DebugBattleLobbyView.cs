using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System.Collections.Generic;

namespace Multiplayer {
	public static class DebugBattleLobbyView {
		public static GUIStyle style = new GUIStyle();
		public const int HorizontalSpacing = 10;
		public static void Update() {
			if (GameController.Instance.Lobby.State == StateEnum.GettingReady)
				_remainingGettingReadyTime -= Time.deltaTime;
		}

//		private byte _debugData;
//		private void OnDebugDataReceived(int commandId, int senderId, byte[] data) {
//			_debugData = data[0];
//		}
		private static bool _waitingServerResponse;
		private static StateEnum _prevState;
		private static float _remainingGettingReadyTime;
		public static void OnGUI() {
			if (!GameController.Inited)
				return;
			if (_waitingServerResponse) {
				GUILayout.Label("waiting server response...", DebugOnGUIAuth.ProperHeight);
				return;
			}
			GUILayout.Label("State = " + GameController.Instance.Lobby.State.ToString());
			switch (GameController.Instance.Lobby.State) {
				case StateEnum.Idle:
					// Lobby match can be searched.
					if (GUILayout.Button("Find match", DebugOnGUIAuth.ProperHeight)) {
						_waitingServerResponse = true;
						LobbyController.StartSearchMatch((success)=>{
							_waitingServerResponse = false;
							Debug.Log("Start search success = " + success.ToString());
						});
					}
					break;
				case StateEnum.SearchingMatch:
					// Lobby match search.
					GUILayout.Label("Searching match..." + Mathf.RoundToInt(GameController.Instance.Lobby.EndTime-Time.time).ToString(), DebugOnGUIAuth.ProperHeight);
					if (GUILayout.Button("Cancel", DebugOnGUIAuth.ProperHeight)) {
						_waitingServerResponse = true;
						LobbyController.CancelSearchMatch((success)=>{
							_waitingServerResponse = false;
						});
					}
					break;
				case StateEnum.ConnectingMatch:
					// Connecting game.
					GUILayout.Label("connecting game...", DebugOnGUIAuth.ProperHeight);
					break;
				case StateEnum.GettingReady:
					// Getting ready.
					GUILayout.Label("preparing resources, getting ready...", DebugOnGUIAuth.ProperHeight);
					if (_prevState != StateEnum.GettingReady)
						_remainingGettingReadyTime = Random.value*3+2;			
					if (_remainingGettingReadyTime<0)
						LobbyController.BecomeReadyForGame();
					GUILayout.Label(Mathf.RoundToInt(_remainingGettingReadyTime).ToString(), DebugOnGUIAuth.ProperHeight);
					break;
				case StateEnum.WaitingOtherPlayers:
					GUILayout.Label("Waiting other players...", DebugOnGUIAuth.ProperHeight);
					break;
				case StateEnum.IsPlaying:
					// Match is played.
					GUILayout.Label("Match is currently playing..." , DebugOnGUIAuth.ProperHeight);
					foreach (var player in GameController.Instance.Lobby.Players)
						GUILayout.Label("player name = " + player.Name);
					if (GUILayout.Button("Leave match", DebugOnGUIAuth.ProperHeight))
						LobbyController.LeaveMatch();
					break;
			}

			_prevState = GameController.Instance.Lobby.State;
		}
	}

}