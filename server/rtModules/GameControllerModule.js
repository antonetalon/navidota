var CommandsController = require("CommandsControllerModule");
var MatchController = require("MatchControllerModule");
var Entities = require("EntitiesModule");
var LagController = require("LagControllerModule");
var GameLoopUpdateInterval = 50;

module.exports.OnStartSession = function() {
    CommandsController.RegisterCommands();
    LagController.Init();
    RTSession.setInterval(GameLoopUpdate, GameLoopUpdateInterval);
}

module.exports.OnStartMatch = function() {
    MatchController.OnStartMatch();
}

function GameLoopUpdate() {
    CommandsController.ProcessCommands();
    if (MatchController.GetMatchStarted())
        MatchController.Update();
}