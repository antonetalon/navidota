using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Entities {
	private static long _maxId;
	private static List<Entity> _entities = new List<Entity>();
	private static List<EntitySystem> _systems = new List<EntitySystem>();
	public static void Init(List<EntitySystem> systems) {
		EntityComponent.Init();
		_maxId = 0;
		_entities.Clear();
		_systems.Clear();
		foreach (var system in systems)
			_systems.Add(system);		
		foreach (EntitySystem system in _systems)
			system.OnStart();
	}
	public static bool Exists(params Type[] types) {
		foreach (var item in _entities) {
			if (item.HasComponents(types))
				return true;
		}
		return false;
	}
	public static IEnumerable<Entity> GetEntities(Type[] requiredComponents) {
		foreach (var item in _entities) {
			if (item.HasComponents(requiredComponents)) {
				yield return item;
			}
		}
	}
	public static List<Entity> Find(params Type[] types) {
		List<Entity> res = new List<Entity>();
		foreach (var item in _entities) {
			if (item.HasComponents(types))
				res.Add(item);
		}
		return res;
	}
	public static Entity Find(long id) {
		foreach (var item in _entities) {
			if (item.Id == id)
				return item;
		}
		return null;
	}

	public static Entity AddEntity(params Type[] types) {
		_maxId++;
		Entity entity = new Entity(_maxId, types);
		AddEntity(entity);
		return entity;
	}
	private static void AddEntity(Entity entity) {
		_entities.Add(entity);
		foreach (EntitySystem system in _systems) {
			if (entity.HasComponents(system.RequiredComponents))
				system.OnAdded(entity);
		}
	}
	public static void OnComponentAdded(Entity entity, Type addedComponent) {
		if (!_entities.Contains(entity) || !addedComponent.IsSubclassOf(typeof(EntityComponent))) {
			Debug.LogError("Wrong OnComponentAdded call");
			return;
		}
		foreach (EntitySystem system in _systems) {
			if (entity.HasComponents(system.RequiredComponents) && system.RequiresComponent(addedComponent))
				system.OnAdded(entity);
		}
	}
	public static void Update() {
		foreach (EntitySystem system in _systems) {
			foreach (Entity entity in _entities) {
				if (entity.HasComponents(system.RequiredComponents))
					system.Update(entity);
			}
		}
	}
}
