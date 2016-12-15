/*using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System.Collections.Generic;

namespace Multiplayer {
	public class BattleLobbyController {
		LobbyModel _model;
		public BattleLobbyController(LobbyModel model) {
			this._model = model;
			GameSparksRTManager.Instance.OnRTMatchSearchFinishes += OnMatchFound;
			GameSparksRTManager.Instance.OnRTMatchConnected += OnRTMatchConnected;
		}

		public void Destroy() {
			GameSparksRTManager.Instance.OnRTMatchSearchFinishes -= OnMatchFound;
			GameSparksRTManager.Instance.OnRTMatchConnected -= OnRTMatchConnected;
		}
		public void StartSearchMatch(System.Action<bool> onResponse) {
			//_model.OnStartSearch();
			GameSparksRTManager.Instance.StartSearchMatch((success)=>{ 
				if (success)
					_model.OnStartSearch();
				if (onResponse!=null)
					onResponse(success);
			});
		}
		public void CancelSearchMatch(System.Action<bool> onResponse) {
			GameSparksRTManager.Instance.CancelSearchMatch((success)=>{ 
				if (success)
					_model.OnSearchCancelled();
				if (onResponse!=null)
					onResponse(success);
			});
		}
		MatchLobby _lobbyToConnect;
		private void OnMatchFound(GSMatchLobbyInfo match) {			
			if (match!=null)
				_lobbyToConnect = new MatchLobby( match );
			else
				_lobbyToConnect = null;

			// Find oppo player.
			PlayerDesc oppo = _lobbyToConnect.OppoPlayer;
			// Find oppo hero.
			PlayerController.SendGetOppoHero(oppo, (hero)=>{
				if (hero==null) {
					// If failed to get oppo.
					_lobbyToConnect = null;
					_model.OnSearchEnded(null);
				} else {
					_model.OnOppoHeroDownloaded(hero);
					// Start match.
					GSMatchLobbyInfo gsLobby = _lobbyToConnect.GetGSMatch();
					GameSparksRTManager.Instance.ConnectRT(gsLobby.hostURL, gsLobby.portID, gsLobby.myAcccessToken);
				}
			});
		}
		void OnRTMatchConnected (bool connected)
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
		}
	}

}*/