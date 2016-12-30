using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

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
		/*StringBuilder sb = new StringBuilder ("sync received at ");
		sb.Append (Timer.Time);
		sb.Append (", lag = ");
		sb.Append (command.Lag);
		foreach (var change in command.Changes) {
			sb.Append ("; (type=");
			sb.Append (change.After.GetType ());
			sb.Append (", data=");
			sb.Append (change.After.ToString ());
			sb.Append (")");
		}
		Debug.Log (sb.ToString ());*/

		//LogCharPos ("Before sync");
		float commandDelay = (command.Lag + LagController.Lag) * 0.001f;
		TimeMachiene.GoToPast (commandDelay);
		//LogCharPos ("In past");
		foreach (var change in command.Changes)
			SaveBeforeState (change);
		foreach (var change in command.Changes)
			ApplyChange (change);
		//LogCharPos ("after sync in past");
		TimeMachiene.SaveReceivedCommands (command.Changes);
		TimeMachiene.GoForward (commandDelay);
		//LogCharPos ("after sync in curr time");
	}
	public static void LogCharPos(string comment) {
		var chars = Entities.GetEntities (new Type[] { typeof(PositionComponent), typeof(MovingComponent) });
		Entity character = null;
		foreach (var curr in chars) {
			character = curr;
			break;
		}
		if (character == null)
			Debug.Log (comment + " - char not exists");
		else
			Debug.LogFormat ("{0} - pos=[{1:##.000};{2:##.000}]", comment, character.GetComponent<PositionComponent> ().Position.x, character.GetComponent<PositionComponent> ().Position.y);
	}
	private static List<ComponentChange> _currUpdateChanges;
	private static void SaveBeforeState(ComponentChange change) {
		Entity entity = Entities.Find (change.EntityId);
		if (entity == null)
			return; // Prev component is null.
		EntityComponent prevComponent = entity.GetComponent(change.After.GetType());
		if (prevComponent == null)
			return;
		change.SetPrevState (prevComponent);
	}
	public static void ApplyChange(ComponentChange change) {
		Entity entity = Entities.Find (change.EntityId);
		Type componentType = change.IsRemoved?change.Before.GetType():change.After.GetType ();
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
					entity = Entities.AddEntityFromSync(change.EntityId, change.After);
				else // Add new component.
					entity.AddComponent (change.After);
			} else {
				// Apply changes to existing component.
				existingComponent.AddChange(change.After);
			}
		}
	}
	public static void UndoChange(ComponentChange change) {
		Entity entity = Entities.Find (change.EntityId);
		Type componentType = change.IsRemoved?change.Before.GetType():change.After.GetType ();
		if (change.IsRemoved) {
			// Add new component.
			if (entity == null) // Add new entity.
				entity = Entities.AddEntityFromSync(change.EntityId, change.Before);
			else // Add new component.
				entity.AddComponent (change.Before);
		} else {
			if (change.Before == null) {
				// Remove component.
				entity.RemoveComponent (componentType);
			} else {
				// Undo changes in existing component.
				EntityComponent existingComponent = entity.GetComponent (componentType);
				existingComponent.AddChange(change.Before);
			}
		}
	}
}
