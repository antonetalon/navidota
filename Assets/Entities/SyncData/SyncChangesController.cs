using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
		//Debug.LogFormat("Sync received, type = {0}, id={1}, del={2}, change={3}", command.Change.Change.GetType().ToString(),
		//	command.Change.EntityId, command.Change.IsRemoved, command.Change.Change.ToString());
		foreach (var change in command.Changes)
			ApplyChange(change);
	}
	static void ApplyChange(ComponentChange change) {
		Entity entity = Entities.Find (change.EntityId);
		Type componentType = change.Change.GetType ();
		if (change.IsRemoved) {
			if (entity == null) {
				Debug.Log ("Received intent to remove component from non-existing entity");
				return;
			} else {
				// Remove component.
				entity.RemoveComponent (componentType);
			}
		} else {
			EntityComponent existingComponent = entity == null ? null : entity.GetComponent (componentType);
			if (existingComponent == null) {				
				if (entity == null) // Add new entity.
					entity = Entities.AddEntityFromSync(change.EntityId, change.Change);
				else // Add new component.
					entity.AddComponent (change.Change);
			} else {
				// Apply changes to existing component.
				existingComponent.AddChange(change.Change);
			}
		}
	}
}
