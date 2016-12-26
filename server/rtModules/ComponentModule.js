var nameByInd = []
var indByName = {};

function AddComponentDesc(name, typeInd) {
    nameByInd[typeInd] = name;
    indByName[name] = typeInd;
}
AddComponentDesc("Match", 1);
AddComponentDesc("Position", 2);
AddComponentDesc("Moving", 3);
AddComponentDesc("InputControl", 4);

module.exports.GetName = function(typeInd) {
    var name = nameByInd[typeInd];
    if (name==undefined)
        RTSession.getLogger().debug("Component not registered, ind = "+typeInd);
   return name;
}
module.exports.GetInd = function(name) {
    var ind = indByName[name];
    if (ind==-1)
        RTSession.getLogger().debug("Component not registered, name = "+name);
    return ind;
}