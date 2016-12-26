var Component = require("ComponentModule");
var Entities = require("EntitiesModule");

module.exports.CreateEntity = function(id) {
    return new Entity(id);
}

function Entity(id) {
    this.Id = id;
    this.HasComponents = function(componentNames) {
        for (var i=0;i<componentNames.length;i++) {
            if (!this.hasOwnProperty(componentNames[i]))
                return false;
        }
        return true;
    }
    this.ComponentNames = function() {
        var names = [];
        for (var name in this) {
            if (name=="Id" || name=="HasComponents" || name=="ComponentNames")
                continue;
            names.push(name);
        }
        return names;
    }
}