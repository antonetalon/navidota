var Utils = require("UtilsModule");
var Component = require("ComponentModule");
var Vector2 = require("Vector2Module");
var prevEntities;
var changes;

module.exports.Init = function() {
    prevEntities = [];
    changes = [];
}

module.exports.SavePrevComponents = function(currEntities) {
    //RTSession.getLogger().debug("before save prev components");
    //RTSession.getLogger().debug("currEntities.length = " + currEntities.length);
    prevEntities = Utils.Clone(currEntities);
    //RTSession.getLogger().debug("after save prev components");
}

module.exports.CalcComponentsChange = function(currEntities) {
   // var log = currEntities.length != prevEntities.length;
    //if (log) RTSession.getLogger().debug("count changed");
    var componentName;
    var compNames;
    //RTSession.getLogger().debug("before calc change");
    changes.length = 0;
    for (var i=0;i<currEntities.length;i++) {
        var prevInd = FindInd(prevEntities, currEntities[i].Id);
        if (prevInd==-1) {
            // Add all components.
            //RTSession.getLogger().debug("add all components from new entity");
            //RTSession.getLogger().debug("entity = " + JSON.stringify(currEntities[i]));
            compNames = currEntities[i].ComponentNames();
            for (var j = 0;j<compNames.length;j++) {
                componentName = compNames[j];
                //RTSession.getLogger().debug("adding component with name = " + componentName);
                var change = new ComponentChange(currEntities[i].Id, false, null, currEntities[i][componentName]);
                changes.push(change);
            }
        } else {
            //RTSession.getLogger().debug("CalcComponentsChangeInOneEntity");
            CalcComponentsChangeInOneEntity(currEntities[i], prevEntities[prevInd]);
        }
    }
    //if (log) RTSession.getLogger().debug("before iterating old arry, langth = " + prevEntities.length);
    for (i=0;i<prevEntities.length;i++) {
        var currInd = FindInd(currEntities, prevEntities[i].Id);
        //if (log) RTSession.getLogger().debug("currInd = " + currInd);
        if (currInd!=-1)
            continue;
        // Entity deleted, all components removed.
        //RTSession.getLogger().debug("remove all components from removed entity");
        compNames = prevEntities[i].ComponentNames();
        for (var j = 0;j<compNames.length;j++) {
            componentName = compNames[j];
            change = new ComponentChange(prevEntities[i].Id, true, prevEntities[i][componentName], null);
            changes.push(change);
        }
    }
    LogChanges();
    //RTSession.getLogger().debug("after calc change");
}

function LogChanges() {
    if (changes.length==0)
        return;
    RTSession.getLogger().debug("Changes detected = " + JSON.stringify(changes));
}

function CalcComponentsChangeInOneEntity(currEntity, prevEntity) {
    var compNames = currEntity.ComponentNames();
    var componentName;
    var prevComponent;
    var currComponent;
    var change;
    for (var j = 0;j<compNames.length;j++) {
        componentName = compNames[j];
        prevComponent = prevEntity[componentName];
        currComponent = currEntity[componentName];
        if (prevComponent==null && currComponent!=null) {
            // Add component.
            change = new ComponentChange(currEntity.Id, false, null, currComponent);
            changes.push(change);
        } else {
            // Change component.
            change = new ComponentChange(currEntity.Id, false, prevComponent, currComponent);
            if (!change.EmptyChange)
                changes.push(change);
        }
    }
    var prevNames = prevEntity.ComponentNames();
    for (var i=0;i<prevNames.length;i++) {
        if (compNames.indexOf(prevNames[i])!=-1)
            continue;
        // Remove components.
        componentName = prevNames[i];
        prevComponent = prevEntity[componentName];
        change = new ComponentChange(currEntity.Id, true, prevComponent, null);
        changes.push(change);
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
    //var log = false;
    //if (isRemoved)
    //    log = true;
    //RTSession.getLogger().debug("before ComponentChange creation");
    if (isRemoved)
    //RTSession.getLogger().debug("ComponentChange params entityId="+entityId+" isRemoved = "+isRemoved+
    //    " prev="+JSON.stringify(prevComponent)+ " curr="+JSON.stringify(currComponent));
    this.EntityId = entityId;
    this.IsRemoved = isRemoved;
    var change = null;
    this.EmptyChange = false;
    if (isRemoved) { 
        //RTSession.getLogger().debug("change = removing component " + JSON.stringify(prevComponent));
        change = Utils.Clone(prevComponent); // Removing.
    } else if (prevComponent==null)
        change = Utils.Clone(currComponent); // Adding.
    else {                                   // Modifying.
        // Calc difference.
        this.EmptyChange = true;
        change = {};
        for (var key in currComponent) {
            if (key=="Type")
                change[key] = currComponent[key];
            else if (typeof(currComponent[key]) == "number") {
                change[key] = currComponent[key] - prevComponent[key];
                if (currComponent[key]!=prevComponent[key]) {
                    this.EmptyChange = false;
                    //RTSession.getLogger().debug("number changed");
                }
            } else if (typeof(currComponent[key]) == "object") {
                // Vector2.
                change[key] = Vector2.Subtract(currComponent[key], prevComponent[key]);
                if (!Vector2.Equals(currComponent[key], prevComponent[key])) {
                    this.EmptyChange = false;
                    //RTSession.getLogger().debug("vector changed");
                }
            } else if (typeof(currComponent[key]) == "boolean") {
                change[key] = !!(currComponent[key]^prevComponent[key]); // Logical xor mimic.
                if (currComponent[key]!=prevComponent[key]) {
                    this.EmptyChange = false;
                    //RTSession.getLogger().debug("bool changed");
                }
            } else if (typeof(currComponent[key]) == "string") {
                change[key] = currComponent[key];
                if (currComponent[key]!=prevComponent[key]) {
                    this.EmptyChange = false;
                    //RTSession.getLogger().debug("string changed");
                }
            }
        }
    }
    //RTSession.getLogger().debug("nothing changed = " + this.EmptyChange);
    var compName = Component.GetName(change.Type);
    this[compName] = change;
    //RTSession.getLogger().debug("after ComponentChange creation");
}