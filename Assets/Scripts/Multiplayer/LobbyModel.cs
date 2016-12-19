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
		bool IsSearchingGame { get; }
		float EndTime { get; }
		bool IsPlaying { get; }
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

	public class LobbyModel : Observable, IMatchLobbyReadonly {
		public bool IsSearchingGame { get; private set; }
		public bool IsPlaying { get; private set; }
		private float _startSearchTime;
		const float SearchingDuration = 18;
		public float EndTime { get { return _startSearchTime + SearchingDuration; } }
		public LobbyModel() {}
		public void StartSearching() {
			_startSearchTime = Time.time;
			Debug.Log("start search time = " + _startSearchTime.ToString());
			IsSearchingGame = true;
			IsPlaying = false;
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
			IsSearchingGame = false;
			_matchFound = true;
			this.Host = host;
			this.Port = port;
			this.AccessToken = accessToken;
			this.MatchId = matchId;
			NotifyObservers();
		}
		public void OnRTSessionConnected() {
			IsPlaying = true;
			NotifyObservers();
		}
		public void OnRTSessionDisconnected() {
			IsPlaying = false;
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
			IsSearchingGame = false;
			_matchNotFound = true;
			NotifyObservers();
		}

	}

}