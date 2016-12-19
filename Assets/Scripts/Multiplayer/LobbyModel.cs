using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System.Collections.Generic;

namespace Multiplayer {
	public class LobbyPlayerModel {
		public readonly string Name;
		public readonly string Id;
		public readonly int IdInMatch;
		public LobbyPlayerModel(string name, string id, int idInMatch) {
			this.Name = name;
			this.Id = id;
			this.IdInMatch = idInMatch;
		}
	}
	public interface IMatchLobbyReadonly: IObservable {
		ReadonlyList<LobbyPlayerModel> Players { get; }
		float EndTime { get; }
		StateEnum State { get; }
	}
	/*public class MatchLobby:Observable, IMatchLobbyReadonly {
		public string MatchId { get { return _GSMatchLobby.matchID; } }
		private GSMatchLobbyInfo _GSMatchLobby;
		public GSMatchLobbyInfo GetGSMatch() { return _GSMatchLobby; }
		public MatchLobby(GSMatchLobbyInfo GSMatchLobby) {
			_GSMatchLobby = GSMatchLobby;
			_players = new List<HumanPlayerDesc>();
			foreach (var gsPlayer in _GSMatchLobby.playerList)
				_players.Add(new HumanPlayerDesc(gsPlayer.displayName, gsPlayer.id));
			Players = new ReadonlyList<HumanPlayerDesc>(_players);
		}
		private List<HumanPlayerDesc> _players;
		public ReadonlyList<HumanPlayerDesc> Players { get; private set; }
		public void UpdatePlayers(List<HumanPlayerDesc> updatedPlayers) {
			// TODO: Update players list.
			NotifyObservers();
		}
		public HumanPlayerDesc OppoPlayer {
			get {
				HumanPlayerDesc oppo = null;
				foreach (var player in _players) {
					if (!player.IsCurrPlayer) {
						oppo = player;
						break;
					}
				}
				return oppo;
			}
		}
	}

*/
	public enum StateEnum { 
		Idle, // Doing nothing, ready to start searching.
		SearchingMatch, // Searching players for match.
		ConnectingMatch, // Connecting session with found players.
		GettingReady, // Loading game resources, preparing scene for start etc.
		WaitingOtherPlayers, // Waiting while all other players got ready.
		IsPlaying // Actually enjoying gameplay.
	}
	public class LobbyModel : Observable, IMatchLobbyReadonly {		
		public StateEnum State { get; private set; }
		private float _startSearchTime;
		const float SearchingDuration = 18;
		public float EndTime { get { return _startSearchTime + SearchingDuration; } }
		public LobbyModel() {
			State = StateEnum.Idle;
		}
		public void StartSearching() {
			_startSearchTime = Time.time;
			Debug.Log("start search time = " + _startSearchTime.ToString());
			State = StateEnum.SearchingMatch;
			_matchFound = false;
			_matchNotFound = false;
			_players = null;
			NotifyObservers();
		}
		private bool _matchFound;
		private bool _matchNotFound;
		List<LobbyPlayerModel> _players;
		public ReadonlyList<LobbyPlayerModel> Players  { get; private set; }
		public string Host { get; private set; }
		public int Port { get; private set; }
		public string AccessToken { get; private set; }
		public string MatchId  { get; private set; }
		public void OnMatchFound(IEnumerable<LobbyPlayerModel> players, string host, int port, string accessToken, string matchId) {
			_players = new List<LobbyPlayerModel>();
			foreach (var player in players)
				_players.Add(player);
			this.Players = new ReadonlyList<LobbyPlayerModel>(_players);
			State = StateEnum.ConnectingMatch;
			_matchFound = true;
			this.Host = host;
			this.Port = port;
			this.AccessToken = accessToken;
			this.MatchId = matchId;
			NotifyObservers();
		}
		public void OnRTSessionConnected() {
			State = StateEnum.GettingReady;
			NotifyObservers();
		}
		public void OnRTSessionDisconnected() {
			State = StateEnum.Idle;
			NotifyObservers();
		}
		public void BecomeReadyForGame() {
			State = StateEnum.WaitingOtherPlayers;
			NotifyObservers();
		}
		public void OnAllPlayersReady() {
			State = StateEnum.IsPlaying;
			NotifyObservers();
		}
		public void OnPlayerDisconnected(int peerId) {
			bool found = false;
			for (int i=0;i<_players.Count;i++) {
				if (_players[i].IdInMatch == peerId) {
					found = true;
					_players.RemoveAt(i);
					break;
				}
			}
			if (!found)
				Debug.LogError("Disconnected player was not in players list");
			NotifyObservers();
		}
		public void OnPlayerConnected(int peerId) {
			// Not implemented. How can player connect if its not 'drop on drop off'
		}
		// Cancelled by player or timeout.
		public void OnSearchCancelled() {
			State = StateEnum.Idle;
			_matchNotFound = true;
			NotifyObservers();
		}

	}

}