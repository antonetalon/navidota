using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EntityComponent {

	public static EntityComponent Create(Type t) {
		if (t==typeof(MovingComponent))
			return new MovingComponent();
		if (t==typeof(MatchComponent))
			return new MatchComponent();
		if (t==typeof(PositionComponent))
			return new PositionComponent();
		if (t==typeof(InputControlComponent))
			return new InputControlComponent();
		Debug.LogError("Component ctor not added for " + t.ToString());
		return null;
	}
}
