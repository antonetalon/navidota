/*using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System.Collections.Generic;

namespace Multiplayer {
	public class DebugBattleLobbyView : MonoBehaviour {
		public static GUIStyle style = new GUIStyle();
		[SerializeField] Texture2D _backTexture;
		public const int HorizontalSpacing = 10;
		bool _forcedSignedIn;
		// Use this for initialization
		void Start () {
			style.normal.background = _backTexture;
			GameSparksRTManager.Instance.OnDataReceived += OnDebugDataReceived;
		}
		void OnDestroy() {
			GameSparksRTManager.Instance.OnDataReceived -= OnDebugDataReceived;
		}

		void OnGUI() {
			GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)));
			GUILayout.BeginVertical(style);
			DebugOnGUIAuth.OnGUI();
			if (!GameController.PlayerDataLoaded) {
				// Authenticating.
			} else {
				if (string.IsNullOrEmpty( GameController.Instance.PlayerModel.Name ))
					AccountController.SignOut();
				// Showing battle lobby.
				OnBattleLobbyGUI();
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		private byte _debugData;
		private void OnDebugDataReceived(int commandId, int senderId, byte[] data) {
			_debugData = data[0];
		}
		private bool _waitingServerResponse;
		void OnBattleLobbyGUI() {
			if (_waitingServerResponse) {
				GUILayout.Label("waiting server response...", DebugOnGUIAuth.ProperHeight);
				return;
			}
			if (GameController.Instance.Lobby.IsSearchingGame) {
				// Lobby match search.
				GUILayout.Label("Searching match...", DebugOnGUIAuth.ProperHeight);
			} else if (GameController.Instance.Lobby.IsPlaying) {
				// Match is played.
				if (GUILayout.Button("Send debug data to opponent", DebugOnGUIAuth.ProperHeight)) {
					_debugData = (byte)Random.Range(0, 255);
					GameSparksRTManager.Instance.SendDataReliable(1, new byte[1] { _debugData });
				}
				GUILayout.Label("debug data = "+_debugData.ToString(), DebugOnGUIAuth.ProperHeight);
			} else if (GameController.Instance.Lobby.Match==null) {
				// Lobby match can be searched.
				if (GUILayout.Button("Find match", DebugOnGUIAuth.ProperHeight)) {
					_waitingServerResponse = true;
					GameController.Instance.LobbyController.StartSearchMatch((success)=>{
						_waitingServerResponse = false;
						Debug.Log("Start search success = " + success.ToString());
					});
				}
			} else {
				// Lobby match exists.
				if (GameController.Instance.Lobby.Match.Players.Count>0) {
					GUILayout.Label("Players list:", DebugOnGUIAuth.ProperHeight);
					foreach (var player in GameController.Instance.Lobby.Match.Players)
						GUILayout.Label(string.Format("name={0}, id={1}", player.Name, player.Id), DebugOnGUIAuth.ProperHeight);
				} else
					GUILayout.Label("Players list empty", DebugOnGUIAuth.ProperHeight);
				if (GameController.Instance.Lobby.IsReady) {
					GUILayout.Label("Waiting opponent...", DebugOnGUIAuth.ProperHeight);
					//if (GUILayout.Button("Send IM READY"))
					//	GameController.Instance.LobbyController.SendReadyToStart();
				} else { //if (GUILayout.Button("Start match")) {
					_waitingServerResponse = true;
					GameController.Instance.LobbyController.Ready((success)=>{
						_waitingServerResponse = false;
						Debug.Log("Getting ready game success = " + success.ToString());
					});
				}
			}
		}
	}

}*/