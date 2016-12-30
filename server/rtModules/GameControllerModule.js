var CommandsController = require("CommandsControllerModule");
var MatchController = require("MatchControllerModule");
var Entities = require("EntitiesModule");
var LagController = require("LagControllerModule");
var SyncStartSystem = require("SyncStartSystemModule");
var GameLoopUpdateInterval = 50;

module.exports.OnStartSession = function() {
    //RTSession.getLogger().debug("before gamecontroller.onstartsession");
    MatchController.OnStartSession();
    CommandsController.RegisterCommands();
    SyncStartSystem.Init();
    LagController.Init();
    RTSession.setInterval(GameLoopUpdate, GameLoopUpdateInterval);
    //RTSession.getLogger().debug("after gamecontroller.onstartsession");
}

module.exports.OnStartMatch = function() {
    //RTSession.getLogger().debug("before gamecontroller.OnStartMatch");
    MatchController.OnStartMatch();
    //RTSession.getLogger().debug("after gamecontroller.OnStartMatch");
}

function GameLoopUpdate() {
    CommandsController.ProcessCommands();
    if (MatchController.GetMatchStarted())
        MatchController.Update(GameLoopUpdateInterval*0.001);
}