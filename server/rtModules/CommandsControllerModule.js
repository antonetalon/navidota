var CommandCodes = require("CommandCodesModule");
var Commands = require("CommandModule");
var SyncStartSystem = require("SyncStartSystemModule");

module.exports.Send = function(opCode, rtData) {
    if (rtData==null)
        rtData = RTSession.newData();
    RTSession.newPacket().setOpCode(opCode).setData(rtData).send();
}

module.exports.RegisterCommands = function() {
    AddCommand(CommandCodes.CommandCodes.ReadyForMatch);
}

module.exports.ReceivedCommands = [];


function AddCommand(opCode) {
    RTSession.onPacket(opCode, function(packet){
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
        ProcessCommand(command);
    }
}

function ProcessCommand(/*CommandModule.Command*/command) {
    //RTSession.getLogger().debug("Processing command " + command.OpCode + " from player " + command.SendersPeer);
    switch (command.OpCode) {
        case CommandCodes.CommandCodes.ReadyForMatch:SyncStartSystem.ProcessReadyForMatch(command); break;
    }
}