/*using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System.Collections.Generic;

namespace Multiplayer {
	
	public interface IMatchLobbyReadonly: IObservable {
		ReadonlyList<HumanPlayerDesc> Players { get; }
		HumanPlayerDesc OppoPlayer { get; }
	}
	public class MatchLobby:Observable, IMatchLobbyReadonly {
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


	public interface ILobbyReadonly: IObservable {
		bool IsSearchingGame { get; }
		IMatchLobbyReadonly Match { get; }
		bool IsReady { get; }
		bool IsPlaying { get; }
		float RemainingSearchTime { get; }
		HeroModel OppoHero { get; }
		event System.Action OnMatchFound;
	}
	public class LobbyModel : Observable, ILobbyReadonly {
		public bool IsSearchingGame { get; private set; }
		private float _startSearchTime;
		const float SearchingDuration = 15;
		public float RemainingSearchTime { get { return _startSearchTime + SearchingDuration - Time.time; } }
		public void OnStartSearch() {
			_startSearchTime = Time.time;
			IsSearchingGame = true;
			_match = null;
			NotifyObservers();
		}
		private MatchLobby _match;
		public MatchLobby MatchEditable { get { return _match; } }
		public IMatchLobbyReadonly Match { get { return _match; } }
		public HeroModel OppoHero { get; private set; }
		public event System.Action OnMatchFound;
		public void OnSearchEnded(MatchLobby foundMatch) {
			IsSearchingGame = false;
			_match = foundMatch;
			NotifyObservers();
			if (OnMatchFound != null)
				OnMatchFound ();
		}
		public void OnSearchCancelled() {
			OnSearchEnded (null);
		}
		public bool IsReady { get; private set; }
		public bool IsPlaying { get; private set; }
		public void OnOppoHeroDownloaded(HeroModel hero) {
			OppoHero = hero;
		}
		//public float MatchStartTime { get; private set; }
		public void OnMatchReady() {
			//_match = null;
			IsReady = true;
		//	MatchStartTime = Time.realtimeSinceStartup;
			NotifyObservers();
		}
		public void OnMatchUnReady() {
			IsReady = false;
			IsPlaying = false;
			NotifyObservers();
		}
		public void OnMatchStarted() {
			IsPlaying = true;
			NotifyObservers();
		}
		public void OnMatchFinished() {
			IsPlaying = false;
			IsReady = false;
			_match = null;
			NotifyObservers();
		}
	}

}*/