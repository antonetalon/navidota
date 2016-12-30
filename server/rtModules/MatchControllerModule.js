var Entities = require("EntitiesModule");
var Match = require("MatchModule");
var Position = require("PositionModule");
var Moving = require("MovingModule");
var InputControl = require("InputControlModule");
var TimeMachiene = require("TimeMachieneModule");
var Timer = require("TimerModule");

var matchStarted = false;
module.exports.GetMatchStarted = function() {
    return matchStarted;
}

module.exports.OnStartMatch = function() {
    Timer.Init();
    TimeMachiene.OnStartMatch();
    RTSession.getLogger().debug("before systems creation");
    var match = Match.CreateSystem();
    //RTSession.getLogger().debug("match created = " + JSON.stringify(match));
    var moving = Moving.CreateSystem();
    //RTSession.getLogger().debug("moving created = " + JSON.stringify(moving));
    var inputControl = InputControl.CreateSystem();
    //RTSession.getLogger().debug("inputControl created = " + JSON.stringify(inputControl));
    var systemsList = [match, moving, inputControl];
    Entities.Init(systemsList);
    matchStarted = true;
}

module.exports.Update = function(delta) {
    Timer.Update(delta)
    if (matchStarted)
        Entities.Update();
}