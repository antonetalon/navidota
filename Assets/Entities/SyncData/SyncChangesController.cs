using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncChangesController {
	
	public static void OnStartMatch() {
		GameSparksRTManager.Instance.OnCommandReceived += OnSyncReceived;
	}
	public static void OnEndMatch() {
		GameSparksRTManager.Instance.OnCommandReceived -= OnSyncReceived;
	}
	static void OnSyncReceived(MatchCommand currCommand) {
		CommandSyncData command = currCommand as CommandSyncData;
		if (command==null)
			return;	
		Debug.LogFormat("Sync received, type = {0}, id={1}, del={2}, change={3}", command.Change.Change.GetType().ToString(),
			command.Change.EntityId, command.Change.IsRemoved, command.Change.Change.ToString());
	}

}
