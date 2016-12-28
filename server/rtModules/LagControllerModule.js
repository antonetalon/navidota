var CommandCodes = require("CommandCodesModule");

module.exports.Init = function() {
    RTSession.onPacket(CommandCodes.CommandCodes.SyncTimeRequest, function(packet){
        var rtData = RTSession.newData().setNumber(1, packet.getData().getNumber(1)) // return the timestamp the server just got
                                    .setNumber(2, new Date().getTime()); // return the current time on the server
        RTSession.newPacket().setData(rtData).setOpCode(CommandCodes.CommandCodes.SyncTimeResponse).setTargetPeers(packet.getSender().getPeerId()).send();
    });
}