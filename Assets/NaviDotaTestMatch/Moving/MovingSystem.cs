using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingSystem : EntitySystem {
	public MovingSystem() : base(new Type[] { typeof(MovingComponent), typeof(PositionComponent) }) {
	}
	public override void Update (Entity entity) {
		MovingComponent moving = entity.GetComponent<MovingComponent>();
		if (!moving.IsMoving)
			return;
		PositionComponent position = entity.GetComponent<PositionComponent>();
		position.Position = Vector2.MoveTowards(position.Position, moving.Target, moving.Speed*Timer.DeltaTime);
		const float eps = 0.001f;
		if (Vector2.Distance(position.Position, moving.Target)<eps)
			moving.IsMoving = false;
	}
}
