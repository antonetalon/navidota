using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class CommandSyncData : MatchCommand {
	public CommandSyncData(RTData data):base(data) { 
		if (data==null)
			return;
		uint i=1;
		long entityId = data.GetInt(i).Value; i++;
		bool isRemoved = data.GetInt(i)==1; i++;
		int componentTypeInd = data.GetInt(i).Value; i++;

		Type t = EntityComponent.GetType(componentTypeInd);
		EntityComponent change = EntityComponent.Create(t, data, i);
		Change = new ComponentChange(entityId, isRemoved, change);
	}
	public readonly ComponentChange Change;
}
