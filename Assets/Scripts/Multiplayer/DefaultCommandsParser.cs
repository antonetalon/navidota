using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class DefaultCommandsParser : ReceivedCommandsFactory {
	public DefaultCommandsParser() {
		AddCommandParser(1, (data)=>{ return new CommandReadyForMatch(); });
		AddCommandParser(2, (data)=>{ return new CommandAllPlayersReady(data); });
	}
}
