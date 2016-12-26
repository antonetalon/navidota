var Comp = require("ComponentModule");
var Entities = require("EntitiesModule");
var Vector2 = require("Vector2Module");
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
    this.Type = 1;
}

function System() {
     this.OnStart = function() {
       // RTSession.getLogger().debug("MatchSystem.OnStart");
        // Add match.
        var match = module.exports.CreateComponent();
        //RTSession.getLogger().debug("match module obj created");
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
        //RTSession.getLogger().debug("input control created");
        Entities.AddEntity([position, moving, inputControl]);
        //RTSession.getLogger().debug("character entity added");
       TestEntitiesChange();
    }
    this.EntityFits = function(/*EntityModule.Entity*/entity) {
        return false;
    };
    this.Update = function(entity) {
        
    }
}


var testEntityId;
function TestEntitiesChange() {
    RTSession.setTimeout(function(){
        // Add entity.
        var position = Position.CreateComponent();
        position.Position = Vector2.Create(1, 1);
        testEntityId = Entities.AddEntity([position]);
    }, 1000);
    RTSession.setTimeout(function(){
        // Add component.
        var moving = Moving.CreateComponent();
        moving.IsMoving = false;
        moving.Speed = 1;
        Entities.AddComponent(testEntityId, moving);
    }, 2000);
    RTSession.setTimeout(function(){
        // Change component.
        //RTSession.getLogger().debug("before changed component");
        var changedEntity = Entities.GetEntity(testEntityId);
        changedEntity.Position.Position.x = 2;
        //RTSession.getLogger().debug("after changed component");
    }, 3000);
    RTSession.setTimeout(function(){
        // Remove component.
        //RTSession.getLogger().debug("before changed component");
        Entities.RemoveComponent(testEntityId, "Moving");
        //RTSession.getLogger().debug("after changed component");
    }, 4000);
    RTSession.setTimeout(function(){
        // Remove entity.
        //RTSession.getLogger().debug("before remove entity");
        Entities.RemoveEntity(testEntityId);
        //RTSession.getLogger().debug("after remove entity");
    }, 5000);
}