using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentChange {
	public readonly long EntityId;
	public readonly bool IsRemoved;
	public readonly EntityComponent Change;
	public ComponentChange(long entityId, bool isRemoved, EntityComponent change) {
		this.EntityId = entityId;
		this.IsRemoved = isRemoved;
		this.Change = change;
	}
}
