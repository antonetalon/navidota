using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class SyncRequestCommand : MatchCommand {
	public SyncRequestCommand(long time):base(null) { 
		Data = new RTData ();
		Data.SetLong (1, time);
	}
}
