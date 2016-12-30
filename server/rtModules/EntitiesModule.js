var Entity = require("EntityModule");
var Component = require("ComponentModule");
var Changes = require("ChangesModule");
var systems;
var entities;
var maxId;
module.exports.GetEntities = function() {
    return entities;
}
module.exports.Init = function(systemsList) {
    Changes.Init();
    entities = [];
    systems = [];
    maxId = 0;
    // Copy systmes list.
    for (var i=0;i<systemsList.length;i++)
        systems.push(systemsList[i]);
    Changes.SavePrevComponents(entities);
    // Start all systems.
    for (var i=0;i<systems.length;i++)
        systems[i].OnStart();
    RTSession.getLogger().debug("systems started");
    // Initial change calc to send initial state to clients.
    Changes.CalcComponentsChange(entities);
    // Log them all.
    var log = "systems = \n " + systems.length + "\n entities = \n " + entities.length;
    RTSession.getLogger().debug(log);
}

module.exports.AddEntity = function(componentsList) {
   // RTSession.getLogger().debug("start adding entity");
    maxId++;
    var newEntity = Entity.CreateEntity(maxId);
   // RTSession.getLogger().debug("entity obj created");
    entities.push(newEntity);
   // RTSession.getLogger().debug("entity obj pushed to array");
    for (var i=0;i<componentsList.length;i++) {
        //RTSession.getLogger().debug("before adding component");
        AddComponent(newEntity, componentsList[i]);
        //RTSession.getLogger().debug("after adding component");
    }
    return maxId;
}

module.exports.AddEntityFromSync = function(entityId, component) {
    if (entityId>maxId)
        maxId = entityId;
    var newEntity = Entity.CreateEntity(entityId);
    AddComponent(newEntity, component);
    entities.push(newEntity);
}

module.exports.AddComponent = function(entityId, component) {
    var entity = module.exports.GetEntity(entityId);
    AddComponent(entity, component);
}

function AddComponent(entity, component) {
    //RTSession.getLogger().debug("start adding component");
    var name = Component.GetName(component.Type);
    //RTSession.getLogger().debug("compoent name = " + name);
    if (entity[name]!=undefined) {
        RTSession.getLogger().debug("Cant add component named " + name + " - it already exists");
        return;
    }
    entity[name] = component;
    // TODO: Call OnAdded in all required systems.
    //RTSession.getLogger().debug("component added");
}

module.exports.RemoveEntity = function(id) {
    //RTSession.getLogger().debug("remove entity called");
    var removedEntity = module.exports.GetEntity(id);
    if (removedEntity == null) {
        RTSession.getLogger().debug("Cant remove entity with id = "+id+" - it does not exist");
        return;
    }
    //RTSession.getLogger().debug("removed entity found, prev length = " + entities.length);
    var componentNames = removedEntity.ComponentNames();
    for (var i=0;i<componentNames.length;i++)
        RemoveComponent(removedEntity, componentNames[i]);
    entities.splice(entities.indexOf(removedEntity), 1);
    //RTSession.getLogger().debug("entity removed from array, new length = " + entities.length);
}

module.exports.RemoveComponent = function(entityId, componentName) {
    var entity = module.exports.GetEntity(entityId);
    RemoveComponent(entity, componentName);
}

function RemoveComponent(entity, componentName) {
    if (entity == null) {
        RTSession.getLogger().debug("Cant remove component "+componentName + " from entity id = " + entityId +" - entity does not exist");
        return;
    }
    // TODO: Call OnDeleted in all required syustems.
    delete entity[componentName];
}


module.exports.GetEntity = function(id) {
    for (var i=0;i<entities.length;i++) {
        if (entities[i].Id == id)
            return entities[i];
    }
    return null;
}

module.exports.Update = function() {
    Changes.SavePrevComponents(entities);
    // Calling Update for all systems.
    for (var i=0;i<systems.length;i++) {
        for (var j=0;j<entities.length;j++) {
            if (systems[i].EntityFits(entities[j]))
                systems[i].Update(entities[j]);
        }
    }
    Changes.CalcComponentsChange(entities);
}