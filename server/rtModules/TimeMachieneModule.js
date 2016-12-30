var Timer = require("TimerModule");
var MatchController = require("MatchControllerModule");
var Entities = require("EntitiesModule");
var Component = require("ComponentModule");
var Utils = require("UtilsModule");
var MaxTimeShift = 1; // Saving history length. Can travel in past on that distance.
var MaxStep = 0.05; // Max fast forward time step.
var frames;
var futureFramesWithCommands;

function FrameStamp(changes, command, deltaTime) {
    this.Changes = changes;
    this.Command = command;
    this.DeltaTime = deltaTime;
}

module.exports.OnStartMatch = function() {
    frames = [];
    futureFramesWithCommands = [];
}
module.exports.SaveCalculatedChanges = function(changes) {
    var frame = new FrameStamp(changes, null, Timer.GetDeltaTime());
    frames.push(frame);
    ClampFrames();
}
module.exports.SaveReceivedCommand = function(command) {
    var frame = new FrameStamp(null, command, 0);
    frames.push(frame);
}
module.exports.GoToPast = function(timeShiftParam) {
    timeShift = Math.min(timeShiftParam, MaxTimeShift);
    //RTSession.getLogger().debug("going to past on " + timeShift + ", frames.length=" + frames.length);
    futureFramesWithCommands.length = 0;
    var currShift = 0;
    while (currShift<timeShift && frames.length>0) {
        var frame = frames.pop();
        if (frame.Command!=null) {
            futureFramesWithCommands.unshift(frame);
            frame.DeltaTime = timeShift - currShift;
        }
        if (frame.Changes!=null) {
            //RTSession.getLogger().debug("undoing " + frame.Changes.length + " changes with dt = " + frame.DeltaTime);
            for (var i=0;i<frame.Changes.length;i++)
                UndoChange(frame.Changes[i]);
        }
        currShift += frame.DeltaTime;
        Timer.GoToPast(frame.DeltaTime);
        //RTSession.getLogger().debug("going to past, currShift=" + currShift + ", entities = " + JSON.stringify(Entities.GetEntities()));
    }
    module.exports.GoForward(currShift-timeShift);
    return timeShift;
}
module.exports.GoForward = function(timeShift) {
    //RTSession.getLogger().debug("go forward on "+timeShift);
    var currShift = 0;
    while (currShift<timeShift) {
        var delta = Math.min(MaxStep, timeShift - currShift);
        var deltaFromCommands = Number.POSITIVE_INFINITY;
        if (futureFramesWithCommands.length>0)
            deltaFromCommands = futureFramesWithCommands[0].DeltaTime;
        var applyCommand = delta > deltaFromCommands;
        delta = Math.min(delta, deltaFromCommands);
        if (delta>0) {
            currShift += delta;
            MatchController.Update(delta);
        }
        if (applyCommand) {
            var command = futureFramesWithCommands.shift();
            CommandsController.ProcessCommand(command);
        }
        for (var i=0;i<futureFramesWithCommands.length;i++)
            futureFramesWithCommands[i].DeltaTime -= delta;
    }
}

function ClampFrames() {
    var framesSum = 0;
    for (var i=frames.length-1;i>=0;i--) {
        if (framesSum<MaxTimeShift)
            framesSum += frames[i].DeltaTime;
        else {
            frames.splice(0, i+1)
            break;
        }
    }
}

function UndoChange(change) {
    var entity = Entities.GetEntity(change.EntityId);
    var typeInd = change.Type;
    var typeName = Component.GetName(typeInd);
    if (change.After==null) {
        // Component removed in change.
        // So add component back.
        if (entity==null) // Add new entity with component on it.
            entity = Entities.AddEntityFromSync(change.EntityId, change.Before);
        else
            Entities.AddComponent(change.Before);
    } else {
        if (change.Before==null) {
            // Component was added in change.
            // So remove component back.
            Entities.RemoveComponent(change.EntityId, typeName);
        } else {
            // Undo changes in existing component.
            var existingComponent = entity[typeName];
            for (var key in change.Before)
                existingComponent[key] = Utils.Clone(change.Before[key]);
        }
    }
}