var CommandCodes = require("CommandCodesModule");
var Commands = require("CommandModule");
var SyncStartSystem = require("SyncStartSystemModule");
var InputControl = require("InputControlModule");
var Changes = require("ChangesModule");
var TimeMachiene = require("TimeMachieneModule");

module.exports.Send = function(opCode, rtData) {
    if (rtData==null)
        rtData = RTSession.newData();
    RTSession.newPacket().setOpCode(opCode).setData(rtData).send();
    RTSession.getLogger().debug("sent command " + opCode);
}

module.exports.RegisterCommands = function() {
    AddCommand(CommandCodes.CommandCodes.ReadyForMatch);
    AddCommand(CommandCodes.CommandCodes.Move);
}

module.exports.ReceivedCommands = [];


function AddCommand(opCode) {
    RTSession.onPacket(opCode, function(packet){
        RTSession.getLogger().debug("received command " + opCode);
        var time = new Date().getTime();
        var sender = packet.getSender().getPeerId();
        var command = new Commands.Command(opCode, time, sender, packet.getData());
        module.exports.ReceivedCommands.push(command);
        //RTSession.getLogger().debug("saved command with opCode = " + opCode + ", commands count = " + module.exports.ReceivedCommands.length);
    });
}

module.exports.ProcessCommands = function() {
    while (module.exports.ReceivedCommands.length>0) {
        var command = module.exports.ReceivedCommands.shift();
        var lag = command.RtData.getNumber(1);
        lag = TimeMachiene.GoToPast (lag);
        Changes.SavePrevComponents(Entities.GetEntities());
        module.exports.ProcessCommand(command);
        TimeMachiene.SaveReceivedCommand(command);
        Changes.CalcComponentsChange(Entities.GetEntities());
        TimeMachiene.GoForward (lag);
    }
}

module.exports.ProcessCommand = function(/*CommandModule.Command*/command) {
    RTSession.getLogger().debug("Processing command " + command.OpCode + " from player " + command.SendersPeer);
    switch (command.OpCode) {
        case CommandCodes.CommandCodes.ReadyForMatch:SyncStartSystem.ProcessReadyForMatch(command); break;
        case CommandCodes.CommandCodes.Move:InputControl.ProcessMoveCommand(command); break;
    }
}