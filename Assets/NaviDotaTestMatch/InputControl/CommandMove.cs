﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;

public class CommandMove : MatchCommand {
	public CommandMove(Vector2 target):base(null) {
		Data = new RTData();
		Data.SetVector2(0, target);
	}
}
