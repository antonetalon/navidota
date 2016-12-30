using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;

public class CommandMove : MatchCommand {
	public CommandMove(Vector2 target):base(null) {
		Data = new RTData();
		uint i = 0;
		Data.SetLong (i, LagController.Lag); i++;
		Data.SetFloat(i, target.x); i++;
		Data.SetFloat(i, target.y); i++;
	}
}
