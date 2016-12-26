using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class NaviDotaCommandsParser : DefaultCommandsParser {
	public NaviDotaCommandsParser() : base() {
		AddCommandParser(3, (data)=>{ return new CommandMove(Vector2.zero); });
		AddCommandParser(4, (data)=>{ return new CommandSyncData(data); });
	}
}
