using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System.Collections.Generic;
using GameSparks.Api.Messages;

namespace Multiplayer {
	public static class LobbyController {
		public static void Init() {
			LobbyModel lobby = new LobbyModel();
			GameController.Instance.SetLobby(lobby);
			if (!_initedOnce) {
				_initedOnce = true;
				GameSparks.Api.Messages.MatchFoundMessage.Listener = OnMatchFound;
				GameSparks.Api.Messages.MatchNotFoundMessage.Listener = OnMatchNotFound;
				GameSparks.Api.Messages.MatchUpdatedMessage.Listener = (mess)=>{
					Debug.Log("Match updated, not implemented");
					// TODO: Implement MatchUpdatedMessage
				};
				GameSparksRTManager.Instance.OnRTDisconnected += OnRTMatchDisConnected;
				GameSparksRTManager.Instance.OnPlayerDisconnected += OnPlayerDisconnected;
				GameSparksRTManager.Instance.OnPlayerConnected += OnPlayerConnected;
				GameSparksRTManager.Instance.OnCommandReceived += OnCommandReceived;
			}
		}
		private static bool _initedOnce;
		const string MatchCode = "TESTMATCH";
		public static void StartSearchMatch(System.Action<bool> onResponse) {
			new GameSparks.Api.Requests.MatchmakingRequest ()
				.SetMatchShortCode (MatchCode)
				.SetSkill (0)
				.Send ((response) => {
					bool success = !response.HasErrors;
					if (success)
						GameController.Instance.Lobby.StartSearching();
					if (onResponse!=null)
						onResponse(success);
				});
		}
		public static void CancelSearchMatch(System.Action<bool> onResponse) {
			new GameSparks.Api.Requests.MatchmakingRequest ()
				.SetMatchShortCode (MatchCode)
				.SetSkill (0)
				.SetAction("cancel")
				.Send ((response) => {
					bool success = !response.HasErrors;
					if (success)
						GameController.Instance.Lobby.OnSearchCancelled();
					if (onResponse!=null)
						onResponse(success);
				});
		}
		public static void LeaveMatch() {
			GameSparksRTManager.Instance.RTSessionDisconnect();
			GameController.Instance.Lobby.OnRTSessionDisconnected();
		}
		private static void OnPlayerDisconnected(int peerId) {
			GameController.Instance.Lobby.OnPlayerDisconnected(peerId);
		}
		private static void OnPlayerConnected(int peerId) {
			GameController.Instance.Lobby.OnPlayerConnected(peerId);
		}
		private static void OnMatchFound(MatchFoundMessage matchFoundMessage) {
			if (matchFoundMessage==null || matchFoundMessage.HasErrors)	{
				Debug.Log("OnMatchFound received no valid match");
				GameController.Instance.Lobby.OnSearchCancelled();
				return;
			}
			List<LobbyPlayerModel> players = new List<LobbyPlayerModel>();
			foreach (var participant in matchFoundMessage.Participants)
				players.Add(new LobbyPlayerModel(participant.DisplayName, participant.Id, (int)participant.PeerId));
			GameController.Instance.Lobby.OnMatchFound(players, matchFoundMessage.Host, (int)matchFoundMessage.Port, matchFoundMessage.AccessToken, matchFoundMessage.MatchId);
			// Connect to found match immediately.
			GameSparksRTManager.Instance.StartNewRTSession(matchFoundMessage.Host, (int)matchFoundMessage.Port, matchFoundMessage.AccessToken, OnRTMatchConnected);
		}
		private static void OnMatchNotFound(MatchNotFoundMessage matchNotFoundMessage) {
			GameController.Instance.Lobby.OnSearchCancelled();
		}
		private static void OnRTMatchConnected(bool success) {
			Debug.Log("Match connected success = " + success.ToString());
			GameController.Instance.Lobby.OnRTSessionConnected();
			Timer.OnMatchConnected ();
		}
		private static void OnRTMatchDisConnected() {
			Debug.Log("Match disconnected");
			GameController.Instance.Lobby.OnRTSessionDisconnected();
			Timer.OnMatchDisconnected();
		}
		public static void BecomeReadyForGame() {
			Debug.Log("Became ready for game");
			GameController.Instance.Lobby.BecomeReadyForGame();
			GameSparksRTManager.Instance.SendDataReliable(new CommandReadyForMatch());
		}
		private static void OnCommandReceived(MatchCommand command) {
			if (command is CommandAllPlayersReady)
				GameController.Instance.Lobby.OnAllPlayersReady();
		}
		public static void Update() {
			UpdateSearchTimeout();
		}
		private static void UpdateSearchTimeout() {
			if (GameController.Instance.Lobby.State != StateEnum.SearchingMatch)
				return;
			if (GameController.Instance.Lobby.EndTime>Time.time)
				return;
			GameController.Instance.Lobby.OnSearchCancelled();
			Debug.Log("cancel search time = " + Time.time.ToString());
		}
		/*void OnRTMatchConnected (bool connected)
		{
			_model.OnSearchEnded(_lobbyToConnect);
			_lobbyToConnect = null;
			if (connected) {
				_model.OnMatchReady();
			} else {
				_model.OnMatchUnReady();
			}
		}

		public void Ready(System.Action<bool> onResponse) {
			if (_model.Match==null || _model.OppoHero==null) {
				// Cant start match if it doesnt exist.
				if (onResponse!=null)
					onResponse(false);
				return;
			}

			if (onResponse!=null)
				onResponse(true);
			StartMatch();
		}

		void StartMatch() {
			// Start multiplayer match.
			HumanPlayerDesc player1 = new HumanPlayerDesc(_model.Match.Players[0].Name, _model.Match.Players[0].Id);
			HumanPlayerDesc player2 = new HumanPlayerDesc(_model.Match.Players[1].Name, _model.Match.Players[1].Id);
			GameController.Instance.StartBattle(player1, player2, _model.OppoHero, BattleType.PVP);
		}
		public void OnMatchFinished() {
			_model.OnMatchFinished();
		}*/
	}

}