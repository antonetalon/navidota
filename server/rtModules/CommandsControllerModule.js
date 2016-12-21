var CommandCodes = require("CommandCodesModule");
var Commands = require("CommandModule");

module.exports.Send = function(opCode, rtData) {
    RTSession.newPacket().setOpCode(opCode).setData(rtData).send();
}

module.exports.RegisterCommands = function() {
    RTSession.getLogger().debug("called register commands");
    AddCommand(CommandCodes.CommandCodes.ReadyForMatch);
}


function AddCommand(opCode) {
    RTSession.getLogger().debug("before registering first command, opCode = "+opCode);
    RTSession.onPacket(opCode, function(packet){
        var time = new Date().getTime();
        var sender = packet.getSender().getPeerId();
        var command = new Commands.Command(opCode, time, sender, packet.getData());
        var commands = [];
        commands.push(command);
        RTSession.getLogger().debug("saved command with opCode = " + opCode);
    });
}