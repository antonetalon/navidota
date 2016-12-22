var CommandsController = require("CommandsControllerModule");
var MatchController = require("MatchControllerModule");

module.exports.OnStartSession = function() {
    CommandsController.RegisterCommands();
    RTSession.setInterval(GameLoopUpdate, GameLoopUpdateInterval);
}

var GameLoopUpdateInterval = 500;
module.exports.OnStartMatch = function() {
    MatchController.OnStartMatch();
}

function GameLoopUpdate() {
    CommandsController.ProcessCommands();
}