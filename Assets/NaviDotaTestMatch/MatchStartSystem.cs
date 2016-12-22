using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MatchStartSystem : EntitySystem {

	public MatchStartSystem() : base(new Type[] {} ) { }
	public override void OnStart () {
		Entities.AddEntity(typeof(MatchComponent));
		Entity character = Entities.AddEntity(typeof(PositionComponent), typeof(MovingComponent), typeof(InputControlComponent));
		character.GetComponent<PositionComponent>().Position = Vector2.zero;
		character.GetComponent<MovingComponent>().IsMoving = false;
		character.GetComponent<MovingComponent>().Speed = 1;
	}

}
