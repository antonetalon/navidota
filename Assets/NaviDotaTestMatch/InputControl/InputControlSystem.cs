using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputControlSystem : EntitySystem {
	public static InputControlSystem Instance { get; private set; }
	public InputControlSystem() : base(new Type[] { typeof(InputControlComponent), typeof(MovingComponent) }) {
		Instance = this;
	}
	public virtual void Send(Vector2 target) {
		foreach (Entity entity in ProcessedEntities()) {
			MovingComponent moving = entity.GetComponent<MovingComponent>();
			moving.IsMoving = true;
			moving.Target = target;
		}
	}
}
