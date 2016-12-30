var Commands = require("CommandModule");
var CommandCodes = require("CommandCodesModule");
var CommandsController = require("CommandsControllerModule");
var GameController = require("GameControllerModule");

var EverybodyReadySent;
var ReadyPlayerIds;
var PlayersCountForMatch;
module.exports.Init = function() {
    RTSession.getLogger().debug("sync start system init called");
    EverybodyReadySent = false;
    ReadyPlayerIds = [];
    PlayersCountForMatch = 2;
    
     RTSession.onPacket(CommandCodes.CommandCodes.ReadyForMatch, function(packet){
        RTSession.getLogger().debug("received ready for match command ");
        var time = new Date().getTime();
        var sender = packet.getSender().getPeerId();
        var command = new Commands.Command(CommandCodes.CommandCodes.ReadyForMatch, time, sender, packet.getData());
        ProcessReadyForMatch(command);
    });
    //RTSession.getLogger().debug("array inited = " + ReadyPlayerIds.length);
}

function ProcessReadyForMatch(/*CommandModule.Command*/command) {
    RTSession.getLogger().debug("called ProcessReadyForMatch");
    SetReady(command.SendersPeer);
    if (EverybodyReady() && !EverybodyReadySent)
        SendAllPlayersReady();
}

function SetReady(playersPeer) {
    //RTSession.getLogger().debug("called setready");
    RTSession.getLogger().debug("ready length = " + ReadyPlayerIds.length);
    if (ReadyPlayerIds.indexOf(playersPeer)!=-1 || EverybodyReadySent)
        return;
    ReadyPlayerIds.push(playersPeer);
    RTSession.getLogger().debug("player ready peer="+playersPeer);
}

function EverybodyReady() {
    //RTSession.getLogger().debug("checking ready, needed count = "+PlayersCountForMatch+", connected = "+RTSession.getPlayers().length);
    //RTSession.getLogger().debug("checking ready, needed count = "+PlayersCountForMatch+", connected = "+RTSession.getPlayers().length + ", ready players = "+ReadyPlayers);
    if (RTSession.getPlayers().length<PlayersCountForMatch)
        return false;
    var notReadyExists = false;
    for (var i=0;i<RTSession.getPlayers().length;i++) {
        var peer = RTSession.getPlayers()[i].getPeerId();
        if (ReadyPlayerIds.indexOf(peer)==-1)
            notReadyExists = true;
    }
    return !notReadyExists;
}

function SendAllPlayersReady() {
    RTSession.getLogger().debug("sending everybody ready");
    EverybodyReadySent = true;
    CommandsController.Send(CommandCodes.CommandCodes.AllPlayersReady, null);
    GameController.OnStartMatch();
}