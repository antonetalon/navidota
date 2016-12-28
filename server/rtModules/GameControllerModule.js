var CommandsController = require("CommandsControllerModule");
var MatchController = require("MatchControllerModule");
var Entities = require("EntitiesModule");
var GameLoopUpdateInterval = 50;

module.exports.OnStartSession = function() {
    CommandsController.RegisterCommands();
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