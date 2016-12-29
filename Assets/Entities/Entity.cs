using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Entity {
	public readonly long Id;
	private readonly bool IsInEntities; 
	public Entity(long id, params Type[] types):this(id, CreateDefaultComponents(types)) { }
	private static IEnumerable<EntityComponent> CreateDefaultComponents(Type[] types) {
		List<EntityComponent> components = new List<EntityComponent> ();
		for (int i = 0; i < types.Length; i++)
			components.Add (EntityComponent.Create (types [i]));
		return components;
	}
	public Entity(long id, IEnumerable<EntityComponent> components) {
		this.IsInEntities = true;
		this.Id = id;
		_components = new List<EntityComponent>();
		foreach (EntityComponent component in components)
			AddComponentPrivate(component);
	}
	List<EntityComponent> _components;
	public void AddComponent(Type t) {
		var component = EntityComponent.Create (t);
		AddComponent (t);
	}
	public void AddComponent(EntityComponent component) {
		AddComponentPrivate(component);
		if (IsInEntities)
			Entities.OnComponentAdded(this, component.GetType());
	}
	public bool RemoveComponent(Type t) {
		EntityComponent removedComponent = null;
		foreach (var currComp in _components) {
			if (currComp.GetType () == t) {
				removedComponent = currComp;
				break;
			}
		}
		if (removedComponent == null)
			return false;
		_components.Remove (removedComponent);
		if (IsInEntities)
			Entities.OnComponentRemoved(this, t);
		return true;
	}
	private void AddComponentPrivate(EntityComponent component) {
		//if (!t.IsSubclassOf(typeof(EntityComponent))) {
		//	Debug.LogError("Should add only EntityComponents to entities");
		//	return;
		//}
		//EntityComponent component = EntityComponent.Create(t);
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
	public IEnumerable<EntityComponent> GetComponents() { return _components; }
	public bool HasComponents(Type[] types) {
		foreach (Type type in types) {
			if (GetComponent(type)==null)
				return false;
		}
		return true;
	}
	public bool Empty { get { return _components.Count == 0; } }
	public Entity Clone() {
		return new Entity (this);
	}
	private Entity(Entity origin) {
		IsInEntities = false;
		Id = origin.Id;
		_components = new List<EntityComponent> ();
		foreach (EntityComponent component in origin._components)
			_components.Add (component.Clone ());
	}
}
