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

function System() {
    this.OnStart = function() {
        //RTSession.getLogger().debug("InputControlSystem.OnStart");
    }
}