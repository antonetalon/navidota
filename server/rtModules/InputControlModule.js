var Commands = require("CommandModule");
var Vector2 = require("Vector2Module");
var Entities = require("EntitiesModule");

var prevEntities;

module.exports.CreateComponent = function() {
   return new Component();
}

var instance;
module.exports.CreateSystem = function() {
    instance = new System();Component
   return instance;
}

function Component() {
    this.Type = 4;
}

module.exports.ProcessMoveCommand = function(/*CommandModule.Command*/command) {
    //RTSession.getLogger().debug("move command processing started");
    var rtData = command.RtData;
    var i = 1;
    var lag = rtData.getNumber(i); i++;
    //RTSession.getLogger().debug("got data = "+rtData);
    var x = rtData.getFloat(i); i++;
    //RTSession.getLogger().debug("got float x = "+x);
    var y = rtData.getFloat(i); i++;
    //RTSession.getLogger().debug("got float y = "+y);
    var targetVec = Vector2.Create(x, y);
    //RTSession.getLogger().debug("got target vec = "+targetVec);
    var entities = instance.GetProcessedEntities();
    //RTSession.getLogger().debug("processed entities count = "+entities.length);
    for (var i=0;i<entities.length;i++) {
        var entity = entities[i];
        entity.Moving.Target = targetVec;
        entity.Moving.IsMoving = true;
        RTSession.getLogger().debug("moving set successfully ");
    }
}

function System() {
    this.OnStart = function() {
        //RTSession.getLogger().debug("InputControlSystem.OnStart");
    };
    this.EntityFits = function(/*EntityModule.Entity*/entity) {
        return entity.HasComponents(["InputControl", "Moving"]);
    };
    this.GetProcessedEntities = function() {
        var allEntities = Entities.GetEntities();
        var processedEntities = [];
        for (var i=0;i<allEntities.length;i++) {
            if (this.EntityFits(allEntities[i]))
                processedEntities.push(allEntities[i]);
        }
        return processedEntities;
    }
    this.Update = function(entity) {
    }
}