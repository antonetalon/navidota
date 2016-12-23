module.exports.CreateComponent = function() {
   return new Component();
}

module.exports.CreateSystem = function() {
   return new System();
}

var Vector2 = require("Vector2Module");
var Entity = require("EntityModule");

function Component() {
    this.Target = Vector2.Create(0, 0);
    this.Speed = 0;
    this.IsMoving = false;
    this.AddToEntity = function(entity) {
        entity.Moving = this;
    }
}

var eps = 0.01;
function System() {
    this.OnStart = function() {
       //RTSession.getLogger().debug("MovingSystem.OnStart");
    };
    this.EntityFits = function(/*EntityModule.Entity*/entity) {
        return entity.HasComponents(["Moving", "Position"]);
    };
    this.Update = function(entity) {
        if (!entity.Moving.IsMoving)
            return;
        
        var toTarget = Vector2.Subtract(entity.Moving.Target, entity.Position.Position);
        var distToTarget = toTarget.GetLength();
        var DeltaTime = 50/1000;
        var coveredLength = entity.Moving.Speed*DeltaTime;
        if (coveredLength>distToTarget) {
            // Arrived to target.
            entity.Position.Position = entity.Moving.Target;
            entity.Moving.IsMoving = false;
            RTSession.getLogger().debug("Arrived to destination");
        } else {
            toTarget = Vector2.Multiply(toTarget, coveredLength/distToTarget);
            entity.Position.Position = Vector2.Add(entity.Position.Position, toTarget);
            RTSession.getLogger().debug("Moved to pos x="+entity.Position.Position.x+"; "+entity.Position.Position.y);
        }
    }
}