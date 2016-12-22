using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingSystem : EntitySystem {
	public static MovingSystem Instance { get; private set; }
	public MovingSystem() : base(new Type[] { typeof(MovingComponent), typeof(PositionComponent) }) {
		Instance = this;
	}
	public override void Update (Entity entity) {
		MovingComponent moving = entity.GetComponent<MovingComponent>();
		if (!moving.IsMoving)
			return;
		PositionComponent position = entity.GetComponent<PositionComponent>();
		position.Position = Vector2.MoveTowards(position.Position, moving.Target, moving.Speed*Time.deltaTime);
		const float eps = 0.001f;
		if (Vector2.Distance(position.Position, moving.Target)<eps)
			moving.IsMoving = false;
	}
	// TODO: Move this to sending by player command system.
	public void Send(Vector2 target) {
		foreach (Entity entity in ProcessedEntities()) {
			MovingComponent moving = entity.GetComponent<MovingComponent>();
			moving.IsMoving = true;
			moving.Target = target;
		}
	}
}
