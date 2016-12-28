using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SinglePlayerMatchSystem : MatchSystem {

	public SinglePlayerMatchSystem() : base() { }
	public override void OnStart () {
		base.OnStart ();
		Entities.AddEntity(typeof(MatchComponent));
		Entity character = Entities.AddEntity(typeof(PositionComponent), typeof(MovingComponent), typeof(InputControlComponent));
		character.GetComponent<PositionComponent>().Position = Vector2.zero;
		character.GetComponent<MovingComponent>().IsMoving = false;
		character.GetComponent<MovingComponent>().Speed = 1;
	}

}
