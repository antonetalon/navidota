var CommandCodes = require("CommandCodesModule");
var Commands = require("CommandModule");
var SyncStartSystem = require("SyncStartSystemModule");
var InputControl = require("InputControlModule");
var Changes = require("ChangesModule");
var TimeMachiene = require("TimeMachieneModule");
var MatchController = require("MatchControllerModule");
var Entities = require("EntitiesModule");

var ReceivedCommands;

module.exports.Send = function(opCode, rtData) {
    if (rtData==null)
        rtData = RTSession.newData();
    RTSession.newPacket().setOpCode(opCode).setData(rtData).send();
    //RTSession.getLogger().debug("sent command " + opCode);
}

module.exports.RegisterCommands = function() {
    ReceivedCommands = [];
    AddCommand(CommandCodes.CommandCodes.Move);
}

function AddCommand(opCode) {
    RTSession.onPacket(opCode, function(packet){
        RTSession.getLogger().debug("received command " + opCode);
        var time = new Date().getTime();
        var sender = packet.getSender().getPeerId();
        var command = new Commands.Command(opCode, time, sender, packet.getData());
        //RTSession.getLogger().debug("command obj created");
        ReceivedCommands.push(command);
        //RTSession.getLogger().debug("saved command with opCode = " + opCode + ", commands count = " + ReceivedCommands.length);
    });
}

module.exports.ProcessCommands = function() {
    while (ReceivedCommands.length>0) {
        //RTSession.getLogger().debug("start processing command");
        //RTSession.getLogger().debug("entities in present before command = " + JSON.stringify(Entities.GetEntities()));
        var command = ReceivedCommands.shift();
        if (command==null)
            continue;
        var lag = command.RtData.getNumber(1)*0.001;
        //RTSession.getLogger().debug("command lag received, it = " + lag);
        lag = TimeMachiene.GoToPast (lag);
         //RTSession.getLogger().debug("went to past");
         //RTSession.getLogger().debug("entities in past before command = " + JSON.stringify(Entities.GetEntities()));
        Changes.SavePrevComponents(Entities.GetEntities());
        // RTSession.getLogger().debug("prev comps saved");
        module.exports.ProcessCommand(command);
        // RTSession.getLogger().debug("command processed in past");
        TimeMachiene.SaveReceivedCommand(command);
        // RTSession.getLogger().debug("command saved to time machiene");
        // RTSession.getLogger().debug("entities in past after command = " + JSON.stringify(Entities.GetEntities()));
        Changes.CalcComponentsChange(Entities.GetEntities());
        // RTSession.getLogger().debug("changes in past calced");
        TimeMachiene.GoForward (lag);
        // RTSession.getLogger().debug("went forward in time");
        // RTSession.getLogger().debug("entities in present after command = " + JSON.stringify(Entities.GetEntities()));
        //RTSession.getLogger().debug("end processing command");
    }
}

module.exports.ProcessCommand = function(/*CommandModule.Command*/command) {
    RTSession.getLogger().debug("Processing command " + command.OpCode + " from player " + command.SendersPeer);
    switch (command.OpCode) {
        case CommandCodes.CommandCodes.Move:InputControl.ProcessMoveCommand(command); break;
    }
}