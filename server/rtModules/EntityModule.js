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
}