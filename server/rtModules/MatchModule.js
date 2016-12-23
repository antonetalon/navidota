var Entities = require("EntitiesModule");
var Vector2 = require("Vector2Module");
var Match = require("MatchModule");
var Position = require("PositionModule");
var Moving = require("MovingModule");
var InputControl = require("InputControlModule");

module.exports.CreateComponent = function() {
   return new Component();
}

module.exports.CreateSystem = function() {
   return new System();
}

module.exports.CreateComponent = function() {
   return new Component();
}

function Component() {
    this.AddToEntity = function(entity) {
        entity.Match = this;
    }
}

function System() {
     this.OnStart = function() {
        //RTSession.getLogger().debug("MatchSystem.OnStart");
        // Add match.
        var match = Match.CreateComponent();
        Entities.AddEntity([match]);
       //RTSession.getLogger().debug("match entity added");
        // Add character.
        var position = Position.CreateComponent();
        position.Position = Vector2.Create(0, 0);
        //RTSession.getLogger().debug("position created");
        var moving = Moving.CreateComponent();
        moving.IsMoving = false;
        moving.Speed = 1;
        //RTSession.getLogger().debug("moving created");
        var inputControl = InputControl.CreateComponent();
       // RTSession.getLogger().debug("input control created");
        Entities.AddEntity([position, moving, inputControl]);
       // RTSession.getLogger().debug("character entity added");
    }
}