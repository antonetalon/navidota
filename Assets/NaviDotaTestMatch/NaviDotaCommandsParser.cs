using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class NaviDotaCommandsParser : DefaultCommandsParser {
	public NaviDotaCommandsParser() : base() {
		Debug.Log ("Navidota commands parser created");
		AddCommandParser(3, (data)=>{ return new CommandMove(Vector2.zero); });
		AddCommandParser(4, (data)=>{ return new CommandSyncData(data); });
		AddCommandParser (5, (data) => { return new SyncRequestCommand (-1); });
		AddCommandParser (6, (data) => { return new SyncResponseCommand (data); });
	}
}
