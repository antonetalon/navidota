var Commands = require("CommandModule");
var CommandCodes = require("CommandCodesModule");
var CommandsController = require("CommandsControllerModule");
var GameController = require("GameControllerModule");

var EverybodyReadySent = false;
var ReadyPlayers = [];
var PlayersCountForMatch = 2;
module.exports.ProcessReadyForMatch = function(/*CommandModule.Command*/command) {
    SetReady(command.SendersPeer);
    if (EverybodyReady() && !EverybodyReadySent)
        SendAllPlayersReady();
}

function SetReady(playersPeer) {
    if (ReadyPlayers.indexOf(playersPeer)!=-1 || EverybodyReadySent)
        return;
    ReadyPlayers.push(playersPeer);
    //RTSession.getLogger().debug("player ready peer="+playersPeer);
}

function EverybodyReady() {
    if (RTSession.getPlayers().length<PlayersCountForMatch)
        return false;
    var notReadyExists = false;
    for (var i=0;i<RTSession.getPlayers().length;i++) {
        var peer = RTSession.getPlayers()[i].getPeerId();
        if (ReadyPlayers.indexOf(peer)==-1)
            notReadyExists = true;
    }
    if (!notReadyExists)
        RTSession.getLogger().debug("everybody ready");
    //else
    //    RTSession.getLogger().debug("not everybody ready");
    return !notReadyExists;
}

function SendAllPlayersReady() {
    EverybodyReadySent = true;
    CommandsController.Send(CommandCodes.CommandCodes.AllPlayersReady, null);
    GameController.OnStartMatch();
}