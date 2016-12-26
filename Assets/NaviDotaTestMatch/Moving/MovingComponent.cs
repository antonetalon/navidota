using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingComponent : EntityComponent {
	public Vector2 Target { get { return GetVector2(0); } set { SetVector2(0, value); } }
	public float Speed  { get { return GetNumber(0); } set { SetNumber(0, value); } }
	public bool IsMoving  { get { return GetBool(0); } set { SetBool(0, value); } }
	public MovingComponent():base(0,1,1,1,0) { }
}
