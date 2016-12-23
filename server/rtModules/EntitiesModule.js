var Entity = require("EntityModule");
var systems;
var entities;
var maxId;
module.exports.GetEntities = function() {
    return entities;
}
module.exports.Init = function(systemsList) {
    entities = [];
    systems = [];
    maxId = 0;
    // Copy systmes list.
    for (var i=0;i<systemsList.length;i++)
        systems.push(systemsList[i]);
    // Start all systems.
    for (var i=0;i<systems.length;i++)
        systems[i].OnStart();
    RTSession.getLogger().debug("systems started");
    // Log them all.
    var log = "systems = \n " + systems.length + "\n entities = \n " + entities.length;
    RTSession.getLogger().debug(log);
}

module.exports.AddEntity = function(componentsList) {
    maxId++;
    var newEntity = Entity.CreateEntity(maxId);
    for (var i=0;i<componentsList.length;i++)
        componentsList[i].AddToEntity(newEntity);
    entities.push(newEntity);
}