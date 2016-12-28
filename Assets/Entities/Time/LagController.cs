using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LagController {
	private static long GetTimeStamp() { return (long)(DateTime.UtcNow - new DateTime (1970, 1, 1, 0, 0, 0)).TotalMilliseconds; } // Curr client system time.
	public static long Lag { get; private set; }
	public static bool Synced { get; private set; }
	const float SyncInterval = 2;
	static float _remainingTime;
	public static void OnMatchStarted() {
		Lag = 0;
		Synced = false;
		GameSparksRTManager.Instance.OnCommandReceived += OnCommandReceived;
		SendSync ();
	}
	public static void OnMatchFinished() {
		GameSparksRTManager.Instance.OnCommandReceived -= OnCommandReceived;
	}
	private static void SendSync() {
		long timeStamp = GetTimeStamp ();
		GameSparksRTManager.Instance.SendDataReliable (new SyncRequestCommand (timeStamp));
		_remainingTime = SyncInterval;
	}

	private static void OnCommandReceived(MatchCommand currCommand) {
		SyncResponseCommand sync = currCommand as SyncResponseCommand;
		if (sync == null)
			return;
		Synced = true;
		long currTime = GetTimeStamp();
		long roundLag = currTime - sync.SendingTimestamp;
		Lag = roundLag / 2;
	}
	public static void Update() {
		_remainingTime -= Time.deltaTime;
		if (_remainingTime < 0)
			SendSync ();
	}
}
