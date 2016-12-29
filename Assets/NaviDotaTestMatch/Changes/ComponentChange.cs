using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentChange {
	public readonly long EntityId;
	public readonly bool IsRemoved;
	public EntityComponent Before { get; private set; }
	public readonly EntityComponent After;
	public ComponentChange(long entityId, bool isRemoved, EntityComponent before, EntityComponent after) {
		this.EntityId = entityId;
		this.IsRemoved = isRemoved;
		this.Before = before == null ? null : before.Clone ();
		this.After = after == null ? null : after.Clone ();
	}
	public void SetPrevState(EntityComponent before) {
		this.Before = before == null ? null : before.Clone ();
	}
}
