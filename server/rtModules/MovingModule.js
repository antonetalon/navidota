module.exports.CreateComponent = function() {
   return new Component();
}

module.exports.CreateSystem = function() {
   return new System();
}

var Vector2 = require("Vector2Module");
var Entity = require("EntityModule");

function Component() {
    this.Target = Vector2.Vector2(0, 0);
    this.Speed = 0;
    this.IsMoving = false;
    this.AddToEntity = function(entity) {
        entity.Moving = this;
    }
}

module.exports.EntityFits = function(/*EntityModule.Entity*/entity) {
    return entity.HasComponents(["Moving", "Position"]);
}

function System() {
    this.OnStart = function() {
       // RTSession.getLogger().debug("MovingSystem.OnStart");
    };
    this.Update = function(entity) {
        // entity.Position - move towards entity.Moving.Target on entity.Moving.Speed* delta time. Stop when dist to pos is less eps=0.01
    }
}