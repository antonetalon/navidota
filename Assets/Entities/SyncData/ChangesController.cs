using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChangesController {
	private static List<Entity> _prevEntities = new List<Entity>();
	private static List<ComponentChange> _changes = new List<ComponentChange>();
	public static void SavePrevComponents(List<Entity> entities) {
		_prevEntities.Clear ();
		foreach (Entity entity in entities)
			_prevEntities.Add (entity.Clone ());
	}
	public static void CalcComponentsChange(List<Entity> currEntities) {
		_changes.Clear ();
		for (int i=0;i<currEntities.Count;i++) {
			int prevInd = FindInd(_prevEntities, currEntities[i].Id);
			if (prevInd==-1) {
				// Add all components.
				foreach (var component in currEntities[i].GetComponents()) {
					var change = new ComponentChange(currEntities[i].Id, false, null, component);
					_changes.Add(change);
				}
			} else {
				CalcComponentsChangeInOneEntity(currEntities[i], _prevEntities[prevInd]);
			}
		}
		for (int i=0;i<_prevEntities.Count;i++) {
			int currInd = FindInd(currEntities, _prevEntities[i].Id);
			if (currInd!=-1)
				continue;
			// Entity deleted, all components removed.
			foreach (var component in _prevEntities[i].GetComponents()) {
				var change = new ComponentChange(_prevEntities[i].Id, true, component, null);
				_changes.Add(change);
			}
		}
		TimeMachiene.SaveCalculatedChanges (_changes, Timer.DeltaTime);
	}

	private static int FindInd(List<Entity> array, long entityId) {
		for (var i=0;i<array.Count;i++) {
			if (array[i].Id == entityId)
				return i;
		}
		return -1;
	}


	private static void CalcComponentsChangeInOneEntity(Entity currEntity, Entity prevEntity) {
		var compNames = currEntity.GetComponents();
		foreach (var currComponent in currEntity.GetComponents()) {
			var prevComponent = prevEntity.GetComponent (currComponent.GetType ());
			if (prevComponent==null && currComponent!=null) {
				// Add component.
				var change = new ComponentChange(currEntity.Id, false, prevComponent, currComponent);
				_changes.Add(change);
			} else {
				// Change component.
				var change = new ComponentChange(currEntity.Id, false, prevComponent, currComponent);
				if (!prevComponent.Equals(currComponent))
					_changes.Add(change);
			}
		}
		foreach (var prevComponent in prevEntity.GetComponents()) {
			if (currEntity.GetComponent(prevComponent.GetType())!=null)
				continue;
			// Remove components.
			var change = new ComponentChange(currEntity.Id, true, prevComponent, null);
			_changes.Add(change);
		}
	}
}
