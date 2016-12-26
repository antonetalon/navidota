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
        var prevInd = FindInd(prevEntities, currEntities[i].Id);
        if (prevInd==-1) {
            // Add all components.
            for (var componentName in currEntities[i].ComponentNames()) {
                var change = new ComponentChange(currEntities[i].Id, false, null, currEntities[i][componentName]);
                changes.push(change);
            }
        } else {
            CalcComponentsChangeInOneEntity(currEntities[i], prevEntities[prevInd]);
        }
    }
    for (i=0;i<prevEntities.lengh;i++) {
        var currInd = FindInd(currEntities, prevEntities[i].Id);
        if (currInd!=-1)
            continue;
        // Entity deleted, all components removed.
        for (componentName in prevEntities[i].ComponentNames()) {
            change = new ComponentChange(prevEntities[i].Id, true, null, prevEntities[i][componentName]);
            changes.push(change);
        }
    }
    LogChanges();
}

function LogChanges() {
    if (changes.length==0)
        return;
    RTSession.getLogger().debug(JSON.stringify(changes));
}

function CalcComponentsChangeInOneEntity(currEntity, prevEntity) {
    for (var componentName in currEntity.ComponentNames()) {
        var prevComponent = prevEntity[componentName];
        var currComponent = currEntity[componentName];
        if (prevComponent==null && currComponent!=null) {
            var change = new ComponentChange(currEntity.Id, false, null, currComponent);
            changes.push(change);
        } else if (prevComponent!=null && currComponent==null) {
            var change = new ComponentChange(currEntity.Id, true, prevComponent, null);
            changes.push(change);
        } else {
            var change = new ComponentChange(currEntity.Id, false, prevComponent, currComponent);
            if (!change.EmptyChange)
                changes.push(change);
        }
    }
}

function FindInd(array, entityId) {
    for (var i=0;i<array.length;i++) {
        if (array[i].Id == entityId)
            return i;
    }
    return -1;
}

function ComponentChange(entityId, isRemoved, prevComponent, currComponent) {
    this.EntityId = entityId;
    this.IsRemoved = isRemoved;
    var change = null;
    this.EmptyChange = false;
    if (isRemoved)
        change = Utils.Clone(prevComponent); // Removing.
    else if (prevComponent==null)
        change = Utils.Clone(currComponent); // Adding.
    else {                                   // Modifying.
        // Calc difference.
        this.EmptyChange = true;
        change = {};
        for (var key in currComponent) {
            if (currComponent[key]!=prevComponent[key])
                this.EmptyChange = false;
            if (key=="Type")
                change[key] = currComponent[key];
            else if (typeof(currComponent[key]) == "number")
                change[key] = currComponent[key] - prevComponent[key];
            else if (typeof(currComponent[key]) == "boolean")
                change[key] = !!(currComponent[key]^prevComponent[key]); // Logical xor mimic.
            else if (typeof(currComponent[key]) == "string")
                change[key] = currComponent[key];
        }
    }
    if (!this.EmptyChange)
        change.AddToEntity(this);
}