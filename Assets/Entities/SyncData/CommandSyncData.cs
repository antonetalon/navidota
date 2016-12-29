using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class CommandSyncData : MatchCommand {
	public CommandSyncData(RTData data):base(data) { 
		if (data==null)
			return;
		uint i = 1;
		Lag = data.GetLong (i).Value; i++;
		int count = data.GetInt (i).Value; i++;
		List<ComponentChange> changes = new List<ComponentChange> ();
		Changes = new ReadonlyList<ComponentChange> (changes);
		for (int ind = 0; ind < count; ind++) {
			ComponentChange currChange = ParseChange (data.GetData (i)); i++;
			changes.Add (currChange);
		}
	}
	private ComponentChange ParseChange(RTData data) {
		uint i=1;
		long entityId = data.GetInt(i).Value; i++;
		bool isRemoved = data.GetInt(i)==1; i++;
		int componentTypeInd = data.GetInt(i).Value; i++;

		Type t = EntityComponent.GetType(componentTypeInd);
		EntityComponent after = EntityComponent.Create(t, data, i);
		return new ComponentChange(entityId, isRemoved, null, after);
	}
	public readonly long Lag;
	public readonly ReadonlyList<ComponentChange> Changes;
	public override string ToString ()
	{
		return base.ToString ();// + " " + Change.Change.GetType().ToString();
	}
}
