module.exports.Clone = function me(obj) {
    if (obj === null || typeof obj !== 'object')
        return obj; 
    //RTSession.getLogger().debug("cloning obj = " + JSON.stringify(obj));
    var temp;
    if (Array.isArray(obj))
        temp = [];
    else
        temp = {};
    for (var key in obj)
        temp[key] = me(obj[key]); 
    return temp;
}