var Utils = require("UtilsModule");
var prevEntities;
var changes;

module.exports.Init = function() {
    prevEntities = [];
    changes = [];
}

module.exports.SavePrevComponents = function(currEntities) {
    prevEntities = Utils.Clone(currEntities);
}

module.exports.CalcComponentsChange = function(currEntities) {
    changes.length = 0;
    for (var i=0;i<currEntities.length;i++) {
        var prevInd = FindPrevInd(currEntities[i].Id);
        if (prevInd==-1) {
            // Add all components.
            for (var componentName in currEntities[i].ComponentNames()) {
                var change = new ComponentChange(currEntities[i].Id, false, null, currEntities[i][componentName]);
                changes.push(change);
            }
        } else {
            // TODO: Find changed components in existing entity.
        }
    }
    // TODO: Find deleted entities.
}

function FindPrevInd(entityId) {
    // TODO: implement.
}

function ComponentChange(entityId, isRemoved, prevComponent, currComponent) {
    this.EntityId = entityId;
    this.IsRemoved = isRemoved;
    var change = null;
    if (isRemoved)
        change = Utils.Clone(prevComponent); // Removing.
    else if (prevComponent==null)
        change = Utils.Clone(currComponent); // Adding.
    else {                                   // Modifying.
        // TODO: calc difference.
    }
    currComponent.constructor();
    change.AddToEntity(this);
}