var CommandsController = require("CommandsControllerModule");

var GameLoopUpdateInterval = 500;
module.exports.StartGameLoop = function() {
    RTSession.setInterval(GameLoopUpdate, GameLoopUpdateInterval);
}

function GameLoopUpdate() {
    CommandsController.ProcessCommands();
}