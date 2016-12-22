module.exports.CreateComponent = function() {
   return new Component();
}

var Vector2 = require("Vector2Module");
function Component() {
    this.Position = Vector2.Vector2(0, 0);
    this.AddToEntity = function(entity) {
        entity.Position = this;
    }
}