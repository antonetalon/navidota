using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : EntityComponent {
	public Vector2 Position { get { return GetVector2(0); } set { SetVector2(0, value); }}
	public PositionComponent():base(0,0,1,0,0) { }
}
