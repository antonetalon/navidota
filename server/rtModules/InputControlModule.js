var Commands = require("CommandModule");
var Vector2 = require("Vector2Module");
var Entities = require("EntitiesModule");

module.exports.CreateComponent = function() {
   return new Component();
}

module.exports.CreateSystem = function() {
   return new System();
}

function Component() {
    this.AddToEntity = function(entity) {
        entity.InputControl = this;
    }
}

module.exports.EntityFits = function(/*EntityModule.Entity*/entity) {
    return entity.HasComponents(["InputControl", "Moving"]);
}

module.exports.GetProcessedEntities = function() {
    var allEntities = Entities.GetEntities();
    var processedEntities = [];
    for (var i=0;i<allEntities.length;i++) {
        if (module.exports.EntityFits(allEntities[i]))
            processedEntities.push(allEntities[i]);
    }
    return processedEntities;
}

module.exports.ProcessMoveCommand = function(/*CommandModule.Command*/command) {
    //RTSession.getLogger().debug("move command processing started");
    var rtData = command.RtData;
    //RTSession.getLogger().debug("got data = "+rtData);
    var x = rtData.getFloat(1);
    //RTSession.getLogger().debug("got float x = "+x);
    var y = rtData.getFloat(2);
    //RTSession.getLogger().debug("got float y = "+y);
    var targetVec = Vector2.Create(x, y);
    //RTSession.getLogger().debug("got target vec = "+targetVec);
    var entities = module.exports.GetProcessedEntities();
    //RTSession.getLogger().debug("processed entities count = "+entities.length);
    for (var i=0;i<entities.length;i++) {
        var entity = entities[i];
        entity.Moving.Target = targetVec;
        entity.Moving.IsMoving = true;
        //RTSession.getLogger().debug("moving set successfully ");
    }
}

function System() {
    this.OnStart = function() {
        //RTSession.getLogger().debug("InputControlSystem.OnStart");
    }
}