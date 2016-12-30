var CommandsController = require("CommandsControllerModule");
var CommandCodes = require("CommandCodesModule");
var Timer = require("TimerModule");

module.exports.SendChanges = function(changes) {
     //RTSession.getLogger().debug("start SendChanges");
    var data = RTSession.newData();
    var i=1;
    data.setNumber(i, Timer.GetDepthInPast());i++;// - sending lag, not timestamp.  new Date().getTime()); i++;
    data.setNumber(i, changes.length); i++;
    for (var ind=0;ind<changes.length;ind++) {
        var currChangeData = SendChange(changes[ind]);
        data.setData(i, currChangeData); i++;
    }
    CommandsController.Send(CommandCodes.CommandCodes.SyncData, data);
    //RTSession.getLogger().debug("end SendChanges");
}

function SendChange(change) {
    var data = RTSession.newData();
    var i = 1;
    data.setNumber(i, change.EntityId); i++;
    data.setNumber(i, +change.IsRemoved); i++; // Convert bool to number.
    var component = change.Before == null?change.After:change.Before;
    if (component==null) {
        RTSession.getLogger().debug("cant find changed component, change = "+JSON.stringify(change));
        return;
    }
    data.setNumber(i, component.Type); i++;
    IterateComponentFields(component, function(val){
        if (typeof(val)!="number")
            return;
        data.setFloat(i, val); i++;
    });
    IterateComponentFields(component, function(val){
        if (typeof(val)!="object") // Vector2.
            return;
        data.setFloat(i, val.x); i++;
        data.setFloat(i, val.y); i++;
    });
    IterateComponentFields(component, function(val){
        if (typeof(val)!="boolean")
            return;
        data.setNumber(i, +val); i++;
    });
    IterateComponentFields(component, function(val){
        if (typeof(val)!="string")
            return;
        data.setString(i, val); i++;
    });
    return data;
}

function IterateComponentFields(component, func) {
    for (field in component) {
        if (!component.hasOwnProperty(field) || field == "Type")
            continue;
        func(component[field]);
    }
}