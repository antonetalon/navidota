using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;

public class CommandMove : MatchCommand {
	public CommandMove(Vector2 target):base(null) {
		Data = new RTData();
		Data.SetFloat(1, target.x);
		Data.SetFloat(2, target.y);
	}
}
