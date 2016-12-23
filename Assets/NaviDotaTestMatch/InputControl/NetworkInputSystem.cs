using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkInputSystem : InputControlSystem {
	public override void Send (Vector2 target) {
		base.Send (target);
		GameSparksRTManager.Instance.SendDataReliable(new CommandMove(target));
	}
}
