using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Entity {
	public readonly long Id;
	public Entity(long id, params Type[] types) {
		this.Id = id;
		_components = new List<EntityComponent>();
		foreach (Type component in types)
			AddComponentPrivate(component);
	}
	List<EntityComponent> _components;
	public void AddComponent(Type t) {
		AddComponentPrivate(t);
		Entities.OnComponentAdded(this, t);
	}
	private void AddComponentPrivate(Type t) {
		if (!t.IsSubclassOf(typeof(EntityComponent))) {
			Debug.LogError("Should add only EntityComponents to entities");
			return;
		}
		EntityComponent component = EntityComponent.Create(t);
		if (component!=null)
			_components.Add(component);
	}
	public T GetComponent<T>() where T:EntityComponent {
		var component = GetComponent(typeof(T));
		return component as T;
	}
	public EntityComponent GetComponent(Type t) {
		foreach (var item in _components) {
			if (item.GetType() == t)
				return item;
		}
		return null;
	}
	public bool HasComponents(Type[] types) {
		foreach (Type type in types) {
			if (GetComponent(type)==null)
				return false;
		}
		return true;
	}
}
