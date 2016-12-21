using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class EntitySystem {
	public Type[] RequiredComponents { get; private set; }
	public EntitySystem(Type[] requiredComponents) {		
		RequiredComponents = new Type[requiredComponents.Length];
		for (int i=0;i<requiredComponents.Length;i++) {
			RequiredComponents[i] = requiredComponents[i];
			if (!RequiredComponents[i].IsSubclassOf(typeof(EntityComponent)))
				Debug.LogError("System should require only components");
		}
	}
	public bool RequiresComponent(Type t) {
		for (int i=0;i<RequiredComponents.Length;i++) {
			if (RequiredComponents[i] == t)
				return true;
		}
		return false;
	}
	protected IEnumerable<Entity> ProcessedEntities() {
		return Entities.GetEntities(RequiredComponents);
	}
	public virtual void OnStart() { }
	public virtual void Update(Entity entity) {}
	public virtual void OnAdded(Entity entity) {}
}
