using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class SyncResponseCommand : MatchCommand {
	public readonly long SendingTimestamp;
	public readonly long ServerProcessingTimestamp;
	public SyncResponseCommand(RTData data):base(data) { 
		if (data == null)
			return;
		this.SendingTimestamp = data.GetLong (1).Value;
		this.ServerProcessingTimestamp = data.GetLong (2).Value;
	}
}
