using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;

public abstract class MatchCommand {
	public RTData Data { get; protected set; }
	public MatchCommand(RTData data) {
		this.Data = data;
	}
}
