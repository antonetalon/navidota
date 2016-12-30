var depthInPast;
var deltaTime;
module.exports.Init = function() {
    depthInPast = 0;
    deltaTime = 0;
}
module.exports.GetDepthInPast = function() {
    //RTSession.getLogger().debug("called GetDepthInPast");
    // How far in past are control in.
    return depthInPast;
}
module.exports.GetDeltaTime = function() {
    // Current delta time. Usually it is update interval. When control is in past and fast forwarding, delta is max step.
    return deltaTime;
}
module.exports.Update = function(deltaTimeParam) {
    deltaTime = deltaTimeParam;
    depthInPast -= deltaTime;
    depthInPast = Math.max(0, depthInPast);
}
module.exports.GoToPast = function(deltaTime) {
    depthInPast += deltaTime;
}