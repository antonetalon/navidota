using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System.Collections.Generic;

namespace Multiplayer {
	public class DebugBattleLobbyView : MonoBehaviour {
		public static GUIStyle style = new GUIStyle();
		public const int HorizontalSpacing = 10;
		// Use this for initialization
		void Start () {
			//GameSparksRTManager.Instance.OnDataReceived += OnDebugDataReceived;
		}
		void OnDestroy() {
			//GameSparksRTManager.Instance.OnDataReceived -= OnDebugDataReceived;
		}

		private byte _debugData;
		private void OnDebugDataReceived(int commandId, int senderId, byte[] data) {
			_debugData = data[0];
		}
		private static bool _waitingServerResponse;
		public static void OnGUI() {
			if (!GameController.Inited)
				return;
			if (_waitingServerResponse) {
				GUILayout.Label("waiting server response...", DebugOnGUIAuth.ProperHeight);
				return;
			}
			if (GameController.Instance.Lobby.IsSearchingGame) {
				// Lobby match search.
				GUILayout.Label("Searching match..." + Mathf.RoundToInt(GameController.Instance.Lobby.EndTime-Time.time).ToString(), DebugOnGUIAuth.ProperHeight);
				if (GUILayout.Button("Cancel", DebugOnGUIAuth.ProperHeight)) {
					_waitingServerResponse = true;
					LobbyController.CancelSearchMatch((success)=>{
						_waitingServerResponse = false;
					});
				}
			} else if (GameController.Instance.Lobby.IsPlaying) {
				// Match is played.
				GUILayout.Label("Match is currently playing..." , DebugOnGUIAuth.ProperHeight);
				//if (GUILayout.Button("Send debug data to opponent", DebugOnGUIAuth.ProperHeight)) {
				//	_debugData = (byte)Random.Range(0, 255);
				//	GameSparksRTManager.Instance.SendDataReliable(1, new byte[1] { _debugData });
				//}
				//GUILayout.Label("debug data = "+_debugData.ToString(), DebugOnGUIAuth.ProperHeight);
			} else {
				// Lobby match can be searched.
				if (GUILayout.Button("Find match", DebugOnGUIAuth.ProperHeight)) {
					_waitingServerResponse = true;
					LobbyController.StartSearchMatch((success)=>{
						_waitingServerResponse = false;
						Debug.Log("Start search success = " + success.ToString());
					});
				}
			}
		}
	}

}