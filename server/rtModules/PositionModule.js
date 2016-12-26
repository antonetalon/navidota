module.exports.CreateComponent = function() {
   return new Component();
}

var Vector2 = require("Vector2Module");
function Component() {
   // RTSession.getLogger().debug("position component creating started");
    this.Position = Vector2.Create(0, 0);
    //RTSession.getLogger().debug("vec2 created for pos");
    this.Type = 2;
    //RTSession.getLogger().debug("addtoentity createds");
}